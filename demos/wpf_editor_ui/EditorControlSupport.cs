/*
 * 2026 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : Status text helpers for the public WPF editor source demo
 */

using System;
using System.Globalization;
using SpiralLab.Sirius3.Localization;
using SpiralLab.Sirius3.Remote;
using SpiralLab.Sirius3.Scanner.Rtc;

namespace Demos.Internal
{
    internal static class RemoteStatusLocalization
    {
        internal static string Format(RemoteControlModes mode, bool isConnected)
        {
            var modeText = MessageBoxLocalization.S(mode == RemoteControlModes.Local
                ? "SiriusEditor_RemoteLocal"
                : "SiriusEditor_RemoteRemote");
            if (mode == RemoteControlModes.Local)
                modeText = modeText.ToUpperInvariant();
            return isConnected
                ? MessageBoxLocalization.S("SiriusEditor_RemoteConnectedFormat", modeText)
                : modeText;
        }
    }

    internal static class EncoderStatusToolTip
    {
        internal static string Format(IRtcMoF rtcMoF, string description)
        {
            if (rtcMoF == null)
                return description ?? string.Empty;

            var speed = rtcMoF.MoFMode == RtcMoFModes.Rotary
                ? string.Format(CultureInfo.CurrentCulture, "ENC R: {0:0.0} °/s", rtcMoF.EncRotaryApproxSpeed)
                : string.Format(CultureInfo.CurrentCulture, "ENC X/Y: {0:0.0} / {1:0.0} mm/s", rtcMoF.EncXApproxSpeed, rtcMoF.EncYApproxSpeed);
            return string.IsNullOrEmpty(description) ? speed : description + Environment.NewLine + speed;
        }
    }
}

/*
 * 2026 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : Shared WPF editor commands for the public source demo
 */

namespace Demos
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using SpiralLab.Sirius3.Document;
    using SpiralLab.Sirius3.Entity;
    using SpiralLab.Sirius3.Marker;
    using SpiralLab.Sirius3.UI;
    using SpiralLab.Sirius3.View;

    internal static class WPFCommands
    {
        internal static bool TryExecutePreviewShortcut(KeyEventArgs e, IDocument document, IView view, IMarker marker)
        {
            if (e == null || document == null)
                return false;
            return TryExecuteEditingShortcut(e, document, view, marker);
        }

        private static bool TryExecuteEditingShortcut(KeyEventArgs e, IDocument document, IView view, IMarker marker)
        {
            if (e.Handled || IsTextInput(e.OriginalSource) || IsTextInput(Keyboard.FocusedElement))
                return false;

            var modifiers = Keyboard.Modifiers;
            if (modifiers == ModifierKeys.None && e.Key == Key.Escape && document.IsSimulationWorking)
            {
                document.ActSimulateStop();
                return true;
            }

            var key = ToGLKey(e.Key == Key.System ? e.SystemKey : e.Key);
            if (!document.IsSimulationWorking && TryGetSimulationSpeed(key, GetKeyMods(), out var speed))
            {
                if (view == null || marker == null || marker.IsBusy)
                    return false;
                document.ActRegen();
                document.ActSimulateStart(view, document.Selected, marker, speed);
                return true;
            }

            if (view?.IsAllowEdit == false || marker?.IsBusy == true || document.IsSimulationWorking)
                return false;
            if (modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.Z: return document.ActUnDo();
                    case Key.Y: return document.ActReDo();
                    case Key.C: return document.ActCopy();
                    case Key.X: return document.ActCut();
                    case Key.V: return document.ActPaste(out var _);
                    case Key.A: return document.ActSelect(document.ActivePage?.ActiveLayer);
                    case Key.R: return ToggleRender(document);
                    case Key.M: return ToggleMark(document);
                }
            }
            return modifiers == ModifierKeys.None && e.Key == Key.Delete && document.ActRemove(document.Selected);
        }

        private static bool IsTextInput(object source)
        {
            var current = source as DependencyObject;
            while (current != null)
            {
                if (current is System.Windows.Controls.Primitives.TextBoxBase ||
                    current is System.Windows.Controls.PasswordBox ||
                    current is System.Windows.Controls.ComboBox comboBox && comboBox.IsEditable)
                {
                    return true;
                }
                current = current is Visual || current is Visual3D
                    ? VisualTreeHelper.GetParent(current)
                    : LogicalTreeHelper.GetParent(current);
            }
            return false;
        }

        private static bool TryGetSimulationSpeed(GLKeys key, KeyMods modifiers, out IDocument.SimulationSpeeds speed)
        {
            speed = IDocument.SimulationSpeeds.Fast;
            if (key != Config.KeyboardSimulationStart || modifiers == null)
                return false;
            switch (modifiers.Value)
            {
                case KeyMods.Bit.None:
                    return true;
                case KeyMods.Bit.Control:
                    speed = IDocument.SimulationSpeeds.Normal;
                    return true;
                case KeyMods.Bit.Control | KeyMods.Bit.Alt:
                    speed = IDocument.SimulationSpeeds.Slow;
                    return true;
                default:
                    return false;
            }
        }

        private static bool ToggleRender(IDocument document)
        {
            var entities = document.Selected?.Where(entity => entity is IRenderable).ToArray();
            if (entities == null || entities.Length == 0)
                return false;
            return document.ActPropertyChanged(entities, nameof(IRenderable.IsAllowRender), !((IRenderable)entities[0]).IsAllowRender);
        }

        private static bool ToggleMark(IDocument document)
        {
            var entities = document.Selected?.Where(entity => entity is IMarkerable).ToArray();
            if (entities == null || entities.Length == 0)
                return false;
            return document.ActPropertyChanged(entities, nameof(IMarkerable.IsAllowMark), !((IMarkerable)entities[0]).IsAllowMark);
        }

        private static KeyMods GetKeyMods()
        {
            var result = new KeyMods();
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) result.Add(KeyMods.Bit.Shift);
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) result.Add(KeyMods.Bit.Control);
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) result.Add(KeyMods.Bit.Alt);
            return result;
        }

        internal static GLKeys ToGLKey(Key key)
        {
            if (key >= Key.A && key <= Key.Z) return (GLKeys)((int)GLKeys.A + (key - Key.A));
            if (key >= Key.D0 && key <= Key.D9) return (GLKeys)((int)GLKeys.D0 + (key - Key.D0));
            if (key >= Key.F1 && key <= Key.F12) return (GLKeys)((int)GLKeys.F1 + (key - Key.F1));
            if (key >= Key.NumPad0 && key <= Key.NumPad9) return (GLKeys)((int)GLKeys.Keypad0 + (key - Key.NumPad0));
            switch (key)
            {
                case Key.Escape: return GLKeys.Escape;
                case Key.Enter: return GLKeys.Enter;
                case Key.Space: return GLKeys.Space;
                case Key.Back: return GLKeys.Backspace;
                case Key.Delete: return GLKeys.Delete;
                case Key.Left: return GLKeys.Left;
                case Key.Right: return GLKeys.Right;
                case Key.Up: return GLKeys.Up;
                case Key.Down: return GLKeys.Down;
                case Key.LeftShift: return GLKeys.ShiftLeft;
                case Key.RightShift: return GLKeys.ShiftRight;
                case Key.LeftCtrl: return GLKeys.ControlLeft;
                case Key.RightCtrl: return GLKeys.ControlRight;
                case Key.LeftAlt: return GLKeys.AltLeft;
                case Key.RightAlt: return GLKeys.AltRight;
                case Key.Tab: return GLKeys.Tab;
                case Key.CapsLock: return GLKeys.CapsLock;
                case Key.Scroll: return GLKeys.ScrollLock;
                case Key.NumLock: return GLKeys.NumLock;
                case Key.PrintScreen: return GLKeys.PrintScreen;
                case Key.Pause: return GLKeys.Pause;
                case Key.Insert: return GLKeys.Insert;
                case Key.Home: return GLKeys.Home;
                case Key.End: return GLKeys.End;
                case Key.PageUp: return GLKeys.PageUp;
                case Key.PageDown: return GLKeys.PageDown;
                case Key.OemSemicolon: return GLKeys.OemSemicolon;
                case Key.OemPlus: return GLKeys.OemPlus;
                case Key.OemComma: return GLKeys.OemComma;
                case Key.OemMinus: return GLKeys.OemMinus;
                case Key.OemPeriod: return GLKeys.OemPeriod;
                case Key.OemQuestion: return GLKeys.OemQuestion;
                case Key.OemTilde: return GLKeys.OemTilde;
                case Key.OemQuotes: return GLKeys.OemQuotes;
                case Key.OemBackslash: return GLKeys.OemBackslash;
                case Key.OemCloseBrackets: return GLKeys.OemCloseBrackets;
                case Key.OemOpenBrackets: return GLKeys.OemOpenBrackets;
                case Key.Add: return GLKeys.KeypadAdd;
                case Key.Subtract: return GLKeys.KeypadSubtract;
                case Key.Multiply: return GLKeys.KeypadMultiply;
                case Key.Divide: return GLKeys.KeypadDivide;
                case Key.Decimal: return GLKeys.KeypadDecimal;
                default: return GLKeys.Unknown;
            }
        }
    }
}

/*
 * 2026 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : Image helpers used by the public WPF editor source demo
 */

namespace Demos
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Resources;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Extensions.Logging;

    internal static class WPFWindowStyleHelper
    {
        private static readonly ILogger Log = SpiralLab.Sirius3.Logger.CreateLogger(typeof(WPFWindowStyleHelper).FullName);
        private static readonly object ImageSync = new object();
        private static readonly Dictionary<string, BitmapImage> Images = new Dictionary<string, BitmapImage>(StringComparer.Ordinal);

        internal static global::System.Windows.Controls.Image CreateImage(string resourceName, double size, bool required = false)
        {
            var image = new global::System.Windows.Controls.Image { Width = size, Height = size, Stretch = Stretch.Uniform };
            try
            {
                image.Source = new BitmapImage(new Uri($"/SpiralLab.Sirius3.UI;component/Resources/{resourceName}", UriKind.Relative));
            }
            catch (Exception exception)
            {
                Log.Log(required ? LogLevel.Error : LogLevel.Debug, exception, "WPF demo image {ResourceName} could not be loaded.", resourceName);
            }
            if (image.Source == null)
            {
                image.ToolTip = required ? $"Required image is missing: {resourceName}" : null;
                image.Visibility = required ? Visibility.Visible : Visibility.Collapsed;
            }
            return image;
        }

        internal static BitmapImage LoadWinFormsImageListImage(Type controlType, string resourceName, int imageIndex, bool required = false)
        {
            if (controlType == null) throw new ArgumentNullException(nameof(controlType));
            if (string.IsNullOrWhiteSpace(resourceName)) throw new ArgumentException("A WinForms image-list resource name is required.", nameof(resourceName));
            if (imageIndex < 0) throw new ArgumentOutOfRangeException(nameof(imageIndex));

            var cacheKey = $"{controlType.AssemblyQualifiedName}|{resourceName}|{imageIndex}";
            lock (ImageSync)
            {
                if (Images.TryGetValue(cacheKey, out var cached))
                    return cached;
                try
                {
                    var resources = new ComponentResourceManager(controlType);
                    var stream = resources.GetObject(resourceName, CultureInfo.CurrentUICulture) as global::System.Windows.Forms.ImageListStreamer;
                    using (var imageList = stream == null ? null : new global::System.Windows.Forms.ImageList { ImageStream = stream })
                    {
                        var result = imageList == null || imageIndex >= imageList.Images.Count
                            ? null
                            : ConvertDrawingImage(imageList.Images[imageIndex]);
                        if (result != null)
                            Images[cacheKey] = result;
                        else
                            Log.Log(required ? LogLevel.Error : LogLevel.Debug, "WPF demo image-list entry {ImageIndex} was not found.", imageIndex);
                        return result;
                    }
                }
                catch (Exception exception)
                {
                    Log.Log(required ? LogLevel.Error : LogLevel.Debug, exception, "WPF demo image-list entry {ImageIndex} could not be loaded.", imageIndex);
                    return LoadSerializedWinFormsPng(controlType, resourceName);
                }
            }
        }

        private static BitmapImage ConvertDrawingImage(System.Drawing.Image drawingImage)
        {
            if (drawingImage == null) return null;
            using (var stream = new MemoryStream())
            {
                drawingImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;
                return LoadBitmap(stream);
            }
        }

        private static BitmapImage LoadSerializedWinFormsPng(Type controlType, string resourceName)
        {
            using (var stream = controlType.Assembly.GetManifestResourceStream(controlType.FullName + ".resources"))
            {
                if (stream == null) return null;
                using (var reader = new ResourceReader(stream))
                {
                    reader.GetResourceData(resourceName, out var typeName, out var data);
                    if (data == null || data.Length == 0 || typeName?.IndexOf("System.Drawing.Bitmap", StringComparison.Ordinal) < 0)
                        return null;
                    var start = FindBytes(data, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, 0);
                    var end = FindBytes(data, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 }, Math.Max(0, start));
                    if (start < 0 || end < start) return null;
                    using (var imageStream = new MemoryStream(data, start, end + 12 - start, false))
                        return LoadBitmap(imageStream);
                }
            }
        }

        private static BitmapImage LoadBitmap(Stream stream)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            if (bitmap.CanFreeze) bitmap.Freeze();
            return bitmap;
        }

        private static int FindBytes(byte[] source, byte[] value, int start)
        {
            for (var index = Math.Max(0, start); index <= source.Length - value.Length; index++)
            {
                var matches = true;
                for (var offset = 0; offset < value.Length; offset++)
                    if (source[index + offset] != value[offset]) { matches = false; break; }
                if (matches) return index;
            }
            return -1;
        }
    }
}

namespace Demos
{
    /// <summary>
    /// Keeps only the latest value while one asynchronous UI update is pending.
    /// </summary>
    /// <typeparam name="T">Type of value consumed by the UI update.</typeparam>
    internal sealed class LatestValueDispatcher<T> : IDisposable
    {
        private readonly object syncRoot = new object();
        private readonly Action<Action> dispatch;
        private readonly Action<T> apply;
        private readonly Func<T, bool> acceptValue;
        private T latestValue;
        private bool pending;
        private bool disposed;
        private long generation;

        internal LatestValueDispatcher(Action<Action> dispatch, Action<T> apply, Func<T, bool> acceptValue = null)
        {
            this.dispatch = dispatch ?? throw new ArgumentNullException(nameof(dispatch));
            this.apply = apply ?? throw new ArgumentNullException(nameof(apply));
            this.acceptValue = acceptValue;
        }

        internal bool IsPending
        {
            get
            {
                lock (syncRoot)
                    return pending;
            }
        }

        internal void Post(T value)
        {
            long scheduledGeneration;
            lock (syncRoot)
            {
                // Reject stale sources before they can overwrite a pending current value.
                // 이전 소스가 대기 중인 현재 값을 덮어쓰기 전에 제외합니다.
                if (disposed || (acceptValue != null && !acceptValue(value)))
                    return;

                latestValue = value;
                if (pending)
                    return;
                pending = true;
                scheduledGeneration = ++generation;
            }

            try
            {
                dispatch(() => Drain(scheduledGeneration));
            }
            catch
            {
                lock (syncRoot)
                {
                    if (generation == scheduledGeneration)
                    {
                        pending = false;
                        latestValue = default;
                    }
                }
                throw;
            }
        }

        internal void Reset()
        {
            lock (syncRoot)
            {
                if (disposed)
                    return;
                generation++;
                pending = false;
                latestValue = default;
            }
        }

        private void Drain(long scheduledGeneration)
        {
            T value;
            lock (syncRoot)
            {
                if (generation != scheduledGeneration)
                    return;
                if (disposed)
                {
                    pending = false;
                    latestValue = default;
                    return;
                }

                value = latestValue;
                latestValue = default;
                pending = false;
            }
            apply(value);
        }

        public void Dispose()
        {
            lock (syncRoot)
            {
                disposed = true;
                generation++;
                pending = false;
                latestValue = default;
            }
        }
    }
}

namespace Demos
{
    // Finish independent cleanup steps before reporting failures to the caller.
    internal sealed class WPFCleanup
    {
        private readonly List<Exception> failures = new List<Exception>();
        private readonly List<IDisposable> disposed = new List<IDisposable>();

        internal void Run(Action action)
        {
            try { action(); }
            catch (Exception exception) { failures.Add(exception); }
        }

        internal void Dispose(params IDisposable[] resources)
        {
            foreach (var resource in resources)
            {
                if (resource == null || disposed.Any(value => ReferenceEquals(value, resource)))
                    continue;
                disposed.Add(resource);
                Run(resource.Dispose);
            }
        }

        internal void ThrowIfFailed()
        {
            if (failures.Count > 0)
                throw new AggregateException("WPF cleanup failed.", failures);
        }
    }
}

namespace Demos
{
    using Border = global::System.Windows.Controls.Border;
    using TextBlock = global::System.Windows.Controls.TextBlock;

    internal static class WPFStatusIndicator
    {
        internal static void ApplyReady(Border border, bool active) =>
            Apply(border, active ? "Sirius.Brush.StatusReadyActive" : "Sirius.Brush.StatusReadyInactive",
                active ? "Sirius.Brush.StatusActiveText" : "Sirius.Brush.StatusInactiveText");

        internal static void ApplyBusy(Border border, bool active, int tick)
        {
            var bright = active && (tick & 1) == 0;
            Apply(border, bright ? "Sirius.Brush.StatusBusyActive" : "Sirius.Brush.StatusBusyInactive",
                bright ? "Sirius.Brush.StatusActiveText" : "Sirius.Brush.StatusInactiveText");
        }

        internal static void ApplyError(Border border, bool active) =>
            Apply(border, active ? "Sirius.Brush.StatusErrorActive" : "Sirius.Brush.StatusErrorInactive",
                "Sirius.Brush.StatusInactiveText");

        private static void Apply(Border border, string backgroundKey, string foregroundKey)
        {
            if (border == null) return;
            border.Opacity = 1;
            border.SetResourceReference(Border.BackgroundProperty, backgroundKey);
            if (border.Child is TextBlock text)
                text.SetResourceReference(TextBlock.ForegroundProperty, foregroundKey);
        }
    }
}
