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
            var statusText = isConnected
                ? MessageBoxLocalization.S("SiriusEditor_RemoteConnectedFormat", modeText)
                : modeText;
            return statusText.ToUpperInvariant();
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

        internal static bool TryExecutePreviewShortcut(global::System.Windows.Input.KeyEventArgs e, IDocument document, IView view, IMarker marker)
        {
            if (e == null || document == null)
                return false;
            return TryExecuteEditingShortcut(e, document, view, marker);
        }

        internal static bool TryExecuteEditingShortcut(global::System.Windows.Input.KeyEventArgs e, IDocument document, IView view, IMarker marker = null)
        {
            if (e == null || e.Handled || document == null ||
                IsTextInput(e.OriginalSource) || IsTextInput(Keyboard.FocusedElement))
                return false;
            var modifiers = Keyboard.Modifiers;
            if (modifiers == ModifierKeys.None && e.Key == Key.Escape && document.IsSimulationWorking)
            {
                document.ActSimulateStop();
                return true;
            }

            var key = NormKey(e);
            if (!document.IsSimulationWorking &&
                TryGetSimulationSpeed(key.Key, key.Mods, out var speed))
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
            if (modifiers == ModifierKeys.None && e.Key == Key.Delete)
                return document.ActRemove(document.Selected);
            return false;
        }

        private static bool IsTextInput(object source)
        {
            var current = source as DependencyObject;
            while (current != null)
            {
                if (current is global::System.Windows.Controls.Primitives.TextBoxBase ||
                    current is global::System.Windows.Controls.PasswordBox ||
                    current is global::System.Windows.Controls.ComboBox comboBox && comboBox.IsEditable)
                    return true;
                current = current is Visual || current is Visual3D
                    ? VisualTreeHelper.GetParent(current)
                    : LogicalTreeHelper.GetParent(current);
            }
            return false;
        }

        internal static bool TryGetSimulationSpeed(GLKeys key, KeyMods modifiers, out IDocument.SimulationSpeeds speed)
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
            var nextValue = !((IRenderable)entities[0]).IsAllowRender;
            return document.ActPropertyChanged(entities, nameof(IRenderable.IsAllowRender), nextValue);
        }

        private static bool ToggleMark(IDocument document)
        {
            var entities = document.Selected?.Where(entity => entity is IMarkerable).ToArray();
            if (entities == null || entities.Length == 0)
                return false;
            var nextValue = !((IMarkerable)entities[0]).IsAllowMark;
            return document.ActPropertyChanged(entities, nameof(IMarkerable.IsAllowMark), nextValue);
        }

        internal static KeyEvt NormKey(KeyEventArgs e)
        {
            return new KeyEvt
            {
                Key = MapKey(e.Key == Key.System ? e.SystemKey : e.Key),
                IsDown = true,
                Mods = GetKeyMods(),
                Char = null
            };
        }

        private static KeyMods GetKeyMods()
        {
            var r = new KeyMods();
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) r.Add(KeyMods.Bit.Shift);
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) r.Add(KeyMods.Bit.Control);
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) r.Add(KeyMods.Bit.Alt);
            return r;
        }

        private static GLKeys MapKey(Key k)
        {
            if (k >= Key.A && k <= Key.Z) return (GLKeys)((int)GLKeys.A + (k - Key.A));
            if (k >= Key.D0 && k <= Key.D9) return (GLKeys)((int)GLKeys.D0 + (k - Key.D0));
            if (k >= Key.F1 && k <= Key.F12) return (GLKeys)((int)GLKeys.F1 + (k - Key.F1));
            if (k >= Key.NumPad0 && k <= Key.NumPad9) return (GLKeys)((int)GLKeys.Keypad0 + (k - Key.NumPad0));
            return k switch
            {
                Key.Escape => GLKeys.Escape,
                Key.Enter => GLKeys.Enter,
                Key.Space => GLKeys.Space,
                Key.Back => GLKeys.Backspace,
                Key.Delete => GLKeys.Delete,
                Key.Left => GLKeys.Left,
                Key.Right => GLKeys.Right,
                Key.Up => GLKeys.Up,
                Key.Down => GLKeys.Down,
                Key.LeftShift => GLKeys.ShiftLeft,
                Key.RightShift => GLKeys.ShiftRight,
                Key.LeftCtrl => GLKeys.ControlLeft,
                Key.RightCtrl => GLKeys.ControlRight,
                Key.LeftAlt => GLKeys.AltLeft,
                Key.RightAlt => GLKeys.AltRight,
                Key.Tab => GLKeys.Tab,
                Key.CapsLock => GLKeys.CapsLock,
                Key.Scroll => GLKeys.ScrollLock,
                Key.NumLock => GLKeys.NumLock,
                Key.PrintScreen => GLKeys.PrintScreen,
                Key.Pause => GLKeys.Pause,
                Key.Insert => GLKeys.Insert,
                Key.Home => GLKeys.Home,
                Key.End => GLKeys.End,
                Key.PageUp => GLKeys.PageUp,
                Key.PageDown => GLKeys.PageDown,
                Key.OemSemicolon => GLKeys.OemSemicolon,
                Key.OemPlus => GLKeys.OemPlus,
                Key.OemComma => GLKeys.OemComma,
                Key.OemMinus => GLKeys.OemMinus,
                Key.OemPeriod => GLKeys.OemPeriod,
                Key.OemQuestion => GLKeys.OemQuestion,
                Key.OemTilde => GLKeys.OemTilde,
                Key.OemQuotes => GLKeys.OemQuotes,
                Key.OemBackslash => GLKeys.OemBackslash,
                Key.OemCloseBrackets => GLKeys.OemCloseBrackets,
                Key.OemOpenBrackets => GLKeys.OemOpenBrackets,
                Key.Add => GLKeys.KeypadAdd,
                Key.Subtract => GLKeys.KeypadSubtract,
                Key.Multiply => GLKeys.KeypadMultiply,
                Key.Divide => GLKeys.KeypadDivide,
                Key.Decimal => GLKeys.KeypadDecimal,
                _ => GLKeys.Unknown
            };
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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.IO;
    using System.Linq;
    using System.Resources;
    using Microsoft.Extensions.Logging;

    internal static class WPFWindowStyleHelper
    {
        private static readonly object WinFormsImageSync = new object();
        private static readonly System.Collections.Generic.Dictionary<string, BitmapImage> WinFormsImages =
            new System.Collections.Generic.Dictionary<string, BitmapImage>(StringComparer.Ordinal);
        private static readonly System.Collections.Generic.HashSet<string> MissingImages =
            new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static ResourceSet generatedResources;
        private static bool generatedResourcesInitialized;

        internal static BitmapImage LoadBitmap(string resourceName, bool required = false)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                return null;

            var cacheKey = $"wpf|{resourceName}";
            lock (WinFormsImageSync)
            {
                if (WinFormsImages.TryGetValue(cacheKey, out var cached))
                    return cached;
                if (MissingImages.Contains(cacheKey))
                    return null;

                try
                {
                    var bitmap = LoadGeneratedResourceBitmap(resourceName);
                    if (bitmap != null)
                    {
                        WinFormsImages[cacheKey] = bitmap;
                        return bitmap;
                    }
                }
                catch (Exception ex)
                {
                    SpiralLab.Sirius3.Logger.Log(required ? LogLevel.Error : LogLevel.Debug, ex,
                        $"wpf: {(required ? "required" : "optional")} image '{resourceName}' was not loaded from generated resources");
                    MissingImages.Add(cacheKey);
                    return null;
                }

                MissingImages.Add(cacheKey);
                SpiralLab.Sirius3.Logger.Log(required ? LogLevel.Error : LogLevel.Debug,
                    $"wpf: {(required ? "required" : "optional")} image '{resourceName}' was not found in generated resources");
                return null;
            }
        }

        private static BitmapImage LoadGeneratedResourceBitmap(string resourceName)
        {
            var normalizedName = resourceName.Replace('\\', '/');
            var requestedKey = "resources/" + normalizedName;
            var escapedKey = "resources/" + string.Join("/", normalizedName.Split('/').Select(Uri.EscapeDataString));
            var resources = GetGeneratedResources();
            if (resources == null)
                return null;
            var value = resources.GetObject(requestedKey, true) ?? resources.GetObject(escapedKey, true);
            if (!(value is Stream imageData))
                return null;
            using (var copy = new MemoryStream())
            {
                imageData.Position = 0;
                imageData.CopyTo(copy);
                copy.Position = 0;
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = copy;
                bitmap.EndInit();
                if (bitmap.CanFreeze)
                    bitmap.Freeze();
                return bitmap;
            }
        }

        private static ResourceSet GetGeneratedResources()
        {
            if (generatedResourcesInitialized)
                return generatedResources;
            generatedResourcesInitialized = true;

            var assembly = typeof(SpiralLab.Sirius3.UI.WPF.EditorControl).Assembly;
            var manifestName = assembly.GetManifestResourceNames().FirstOrDefault(name =>
                name.EndsWith(".g.resources", StringComparison.OrdinalIgnoreCase));
            if (manifestName == null)
                return null;
            var manifest = assembly.GetManifestResourceStream(manifestName);
            if (manifest == null)
                return null;
            generatedResources = new ResourceSet(manifest);
            return generatedResources;
        }

        internal static Image CreateImage(string resourceName, double size, bool required = false)
        {
            var image = new Image { Width = size, Height = size, Stretch = Stretch.Uniform };
            image.Source = LoadBitmap(resourceName, required);
            if (image.Source == null)
            {
                image.ToolTip = required ? $"Required image is missing: {resourceName}" : null;
                image.Visibility = required ? Visibility.Visible : Visibility.Collapsed;
            }
            return image;
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
