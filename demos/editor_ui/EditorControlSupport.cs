/*
 * 2026 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : Shared support for the public WinForms and WPF editor controls
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SpiralLab.Sirius3.Localization;
using SpiralLab.Sirius3.Remote;
using SpiralLab.Sirius3.Scanner.Rtc;

namespace Demos
{
    /// <summary>Refreshes Framework's cached DPI factor before a parent scales nested library containers.</summary>
    internal static class DpiContainerScale
    {
#if NETFRAMEWORK
        private static readonly ConditionalWeakTable<ContainerControl, Entry> entries = new ConditionalWeakTable<ContainerControl, Entry>();
        private static readonly System.Reflection.MethodInfo scaleChildren = typeof(ContainerControl).GetMethod(
            "FormDpiChanged", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        private static readonly System.Reflection.FieldInfo deviceDpi = typeof(Control).GetField(
            "deviceDpi", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        private static readonly System.Reflection.MethodInfo rescaleConstants = typeof(Control).GetMethod(
            "RescaleConstantsForDpi", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern uint GetDpiForWindow(IntPtr handle);
#endif

        internal static void Attach(ContainerControl control)
        {
            ToolStripDpi.AttachStatusStrips(control);
#if NETFRAMEWORK
            if (control is Form || control.AutoScaleMode != AutoScaleMode.Dpi || SpiralLab.Sirius3.UI.WinForms.EditorControl.IsDesigner()) return;
            if (!entries.TryGetValue(control, out _)) entries.Add(control, new Entry(control));
#endif
        }

#if NETFRAMEWORK
        private sealed class Entry
        {
            private readonly ContainerControl control;
            private int dpi;
            private bool suspended;
            private bool updatePending;
            private readonly System.Collections.Generic.List<FontState> fonts = new System.Collections.Generic.List<FontState>();

            internal Entry(ContainerControl control)
            {
                this.control = control;
                dpi = control.DeviceDpi;
                TrackFonts(control);
                control.HandleCreated += HandleCreated;
                control.HandleDestroyed += HandleDestroyed;
                control.VisibleChanged += QueueUpdate;
                control.DpiChangedBeforeParent += BeforeParent;
                control.DpiChangedAfterParent += AfterParent;
                control.Disposed += Disposed;
            }

            private void TrackFonts(Control owner)
            {
                fonts.Add(new FontState(owner));
                if (owner is ToolStrip strip)
                    foreach (ToolStripItem item in strip.Items) fonts.Add(new FontState(owner, item));
                owner.FontChanged += FontChanged;
                foreach (Control child in owner.Controls)
                    if (!(child is UserControl)) TrackFonts(child);
            }

            private void FontChanged(object sender, EventArgs args)
            {
                if (suspended || control.DeviceDpi != dpi || ((Control)sender).DeviceDpi != dpi) return;
                foreach (var state in fonts)
                    if (ReferenceEquals(state.Control, sender)) state.Capture();
            }

            private void HandleCreated(object sender, EventArgs args) => QueueUpdate(sender, args);

            private void QueueUpdate(object sender, EventArgs args)
            {
                if (updatePending || control.IsDisposed || control.Disposing || !control.IsHandleCreated) return;
                updatePending = true;
                control.BeginInvoke(new Action(UpdateAfterLayout));
            }

            private void UpdateAfterLayout()
            {
                updatePending = false;
                if (suspended || control.IsDisposed || control.Disposing || !control.IsHandleCreated
                    || control.AutoScaleMode != AutoScaleMode.Dpi) return;
                if (control.Visible && control.TopLevelControl != null && deviceDpi != null)
                {
                    // Synchronize the same cache/constants as Framework's OnHandleCreated,
                    // using this HWND's DPI and preserving child handles and GL contexts.
                    foreach (var state in fonts)
                    {
                        var child = state.Control;
                        if (child.IsDisposed || !child.IsHandleCreated || child.DeviceDpi == control.TopLevelControl.DeviceDpi) continue;
                        int oldDpi = child.DeviceDpi;
                        int nativeDpi = (int)GetDpiForWindow(child.Handle);
                        if (nativeDpi <= 0 || nativeDpi == oldDpi) continue;
                        deviceDpi.SetValue(child, nativeDpi);
                        rescaleConstants?.Invoke(child, new object[] { oldDpi, nativeDpi });
                    }
                }
                var currentDpi = control.DeviceDpi;
                var baseline = control.AutoScaleDimensions;
                if (currentDpi != dpi && baseline == new SizeF(dpi, dpi) && scaleChildren != null)
                {
                    // Hidden Framework controls can miss the parent's DPI messages.
                    // Their outer bounds are already scaled; only recover the missed children.
                    float factor = (float)currentDpi / dpi;
                    control.SuspendLayout();
                    suspended = true;
                    try
                    {
                        ResetScaleCache(baseline);
                        foreach (var state in fonts) state.ScaleUnchangedFont(factor);
                        control.AutoScaleDimensions = new SizeF(currentDpi, currentDpi);
                        scaleChildren.Invoke(control, new object[] { factor });
                    }
                    finally
                    {
                        suspended = false;
                        control.ResumeLayout(true);
                    }
                }
                if (control.AutoScaleDimensions != new SizeF(currentDpi, currentDpi)) return;
                dpi = currentDpi;
                foreach (var state in fonts) state.Capture();
                foreach (var state in fonts)
                    if (state.IsControl && state.Control is ToolStrip strip) ToolStripDpi.Refresh(strip);
            }

            private void ResetScaleCache(SizeF baseline)
            {
                control.AutoScaleMode = AutoScaleMode.Inherit;
                control.AutoScaleMode = AutoScaleMode.Dpi;
                control.AutoScaleDimensions = baseline;
            }

            private void BeforeParent(object sender, EventArgs args)
            {
                if (suspended || dpi == control.DeviceDpi || control.AutoScaleMode != AutoScaleMode.Dpi) return;
                var baseline = control.AutoScaleDimensions;
                control.SuspendLayout();
                suspended = true;
                // Framework caches CurrentAutoScaleDimensions on nested containers.
                // Changing mode invalidates that cache through public APIs. Keep the
                // old baseline until the parent has applied its single scaling pass.
                ResetScaleCache(baseline);
            }

            private void AfterParent(object sender, EventArgs args)
            {
                if (!suspended) { QueueUpdate(sender, args); return; }
                // Framework scales control fonts, but explicit ToolStripItem fonts can
                // remain unchanged even on a visible page. Do not scale changed copies again.
                float factor = (float)control.DeviceDpi / dpi;
                foreach (var state in fonts) state.ScaleUnchangedFont(factor);
                dpi = control.DeviceDpi;
                control.AutoScaleDimensions = new SizeF(dpi, dpi);
                suspended = false;
                control.ResumeLayout(true);
                foreach (var state in fonts) state.Capture();
                QueueUpdate(sender, args);
            }

            private void HandleDestroyed(object sender, EventArgs args)
            {
                updatePending = false;
                if (!suspended) return;
                suspended = false;
                control.ResumeLayout(false);
            }

            private void Disposed(object sender, EventArgs args)
            {
                control.HandleCreated -= HandleCreated;
                control.HandleDestroyed -= HandleDestroyed;
                control.VisibleChanged -= QueueUpdate;
                control.DpiChangedBeforeParent -= BeforeParent;
                control.DpiChangedAfterParent -= AfterParent;
                control.Disposed -= Disposed;
                foreach (var state in fonts)
                {
                    state.Control.FontChanged -= FontChanged;
                    state.Dispose();
                }
                fonts.Clear();
            }

            private sealed class FontState : IDisposable
            {
                internal readonly Control Control;
                private readonly ToolStripItem item;
                private Font previous;
                private Font owned;

                private Font Font => item?.Font ?? Control.Font;
                internal bool IsControl => item == null;

                internal FontState(Control control, ToolStripItem item = null)
                {
                    Control = control;
                    this.item = item;
                    Capture();
                }

                internal void Capture()
                {
                    previous = Font;
                    if (owned != null && !ReferenceEquals(previous, owned))
                    {
                        owned.Dispose();
                        owned = null;
                    }
                }

                internal void ScaleUnchangedFont(float factor)
                {
                    var owner = (object)item ?? Control;
                    if (Control.IsDisposed || (item != null && item.IsDisposed) || !ReferenceEquals(Font, previous)
                        || !System.ComponentModel.TypeDescriptor.GetProperties(owner)["Font"].ShouldSerializeValue(owner)) return;
                    var replacement = new Font(previous.FontFamily, previous.Size * factor, previous.Style,
                        previous.Unit, previous.GdiCharSet, previous.GdiVerticalFont);
                    var retired = owned;
                    owned = replacement;
                    if (item == null) Control.Font = replacement;
                    else item.Font = replacement;
                    retired?.Dispose();
                }

                public void Dispose() { owned?.Dispose(); owned = null; }
            }
        }
#endif
    }

    /// <summary>Preserves logical icon dimensions and borrowed resource images on library toolbars.</summary>
    internal static class ToolStripDpi
    {
        internal const int DefaultStatusHeight = 22;
        internal const int NavigationWidth = 40;
        private static readonly ConditionalWeakTable<ToolStrip, Entry> entries = new ConditionalWeakTable<ToolStrip, Entry>();

        internal static void Attach(ToolStrip strip, int logicalImageSize, Control owner = null, int logicalWidth = 0)
        {
            if (!entries.TryGetValue(strip, out _))
                entries.Add(strip, new Entry(strip, strip is StatusStrip ? 16 : logicalImageSize, owner, logicalWidth));
        }

        internal static void AttachStatusStrips(Control root)
        {
            if (SpiralLab.Sirius3.UI.WinForms.EditorControl.IsDesigner()) return;
            foreach (Control child in root.Controls)
            {
                if (child is UserControl) continue;
                if (child is StatusStrip status) Attach(status, 16);
                if (child.HasChildren) AttachStatusStrips(child);
            }
        }

        internal static void Refresh(ToolStrip strip)
        {
            if (entries.TryGetValue(strip, out var entry)) entry.Update(strip, EventArgs.Empty);
        }

        private static int Pixels(Control owner, int logicalSize)
            => (int)Math.Round(logicalSize * owner.DeviceDpi / 96d);

        private sealed class Entry
        {
            private readonly ToolStrip strip;
            private readonly int logicalImageSize;
            private readonly int logicalWidth;
            private readonly Control owner;
            private readonly Dictionary<ToolStripItem, ImageCopy> naturalImages = new Dictionary<ToolStripItem, ImageCopy>();
            private readonly Dictionary<ToolStripItem, Font> statusFonts = new Dictionary<ToolStripItem, Font>();
            private int statusHeight, statusLabelHeight, statusDpi;
            private bool updating;

            internal Entry(ToolStrip strip, int logicalImageSize, Control owner, int logicalWidth)
            {
                this.strip = strip;
                this.logicalImageSize = logicalImageSize;
                this.logicalWidth = logicalWidth;
                this.owner = owner;
                strip.HandleCreated += Update;
                strip.DpiChangedAfterParent += Update;
                strip.FontChanged += Update;
                if (strip is StatusStrip) strip.Layout += StatusLayout;
                else if (logicalWidth > 0) strip.Layout += NavigationLayout;
                strip.ItemAdded += ItemAdded;
                strip.ItemRemoved += ItemRemoved;
                strip.Disposed += Disposed;
                if (strip is ToolStripDropDown dropdown) dropdown.Opening += Opening;
                Update(strip, EventArgs.Empty);
            }

            private void Opening(object sender, System.ComponentModel.CancelEventArgs e) => Update(sender, e);
            private void ItemAdded(object sender, ToolStripItemEventArgs e) => Update(sender, e);

            private void ItemRemoved(object sender, ToolStripItemEventArgs e)
            {
                statusFonts.Remove(e.Item);
                if (!naturalImages.TryGetValue(e.Item, out var copy)) return;
                if (ReferenceEquals(e.Item.Image, copy.Scaled)) e.Item.Image = copy.Original;
                copy.Scaled?.Dispose();
                naturalImages.Remove(e.Item);
            }

            internal void Update(object sender, EventArgs e)
            {
                if (updating || strip.IsDisposed) return;
                updating = true;
                strip.SuspendLayout();
                try
                {
                    var metricsOwner = owner ?? strip;
                    int size = Pixels(strip is StatusStrip ? strip : metricsOwner, logicalImageSize);
                    strip.ImageScalingSize = new Size(size, size);
                    if (logicalWidth > 0)
                    {
                        strip.AutoSize = false;
                        strip.Width = Pixels(metricsOwner, logicalWidth);
                    }
                    if (strip is StatusStrip status) UpdateStatusMeasurements(status);
                    foreach (ToolStripItem item in strip.Items)
                    {
                        if (logicalWidth > 0 && item.Image != null && item.ImageScaling == ToolStripItemImageScaling.SizeToFit)
                        {
                            // Framework can retain a preferred image size from before parent scaling.
                            item.ImageScaling = ToolStripItemImageScaling.None;
                            item.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                        }
                        if (item is ToolStripDropDownItem dropDownItem)
                            Attach(dropDownItem.DropDown, logicalImageSize, metricsOwner);
                        UpdateNaturalImage(item, metricsOwner);
                    }
                }
                finally
                {
                    strip.ResumeLayout(true);
                    updating = false;
                }
            }

            private void NavigationLayout(object sender, LayoutEventArgs e)
            {
                var metricsOwner = owner ?? strip;
                int imageSize = Pixels(metricsOwner, logicalImageSize);
                if (strip.Width != Pixels(metricsOwner, logicalWidth) ||
                    strip.ImageScalingSize != new Size(imageSize, imageSize) ||
                    strip.Items.OfType<ToolStripButton>().Any(item => item.Image != null &&
                        item.ImageScaling == ToolStripItemImageScaling.SizeToFit &&
                        item.GetPreferredSize(Size.Empty).Height < imageSize)) Update(sender, e);
            }

            private void StatusLayout(object sender, LayoutEventArgs e)
            {
                if (updating || strip.IsDisposed) return;
                if (strip.Height != statusHeight || strip.DeviceDpi != statusDpi)
                {
                    Update(sender, e);
                    return;
                }
                foreach (ToolStripItem item in strip.Items)
                    if (!statusFonts.TryGetValue(item, out var font) || !ReferenceEquals(font, item.Font)
                        || (item is ToolStripStatusLabel && item.Height != statusLabelHeight))
                    {
                        Update(sender, e);
                        return;
                    }
            }

            private void UpdateStatusMeasurements(StatusStrip status)
            {
                // Own vertical status metrics; Framework retains horizontal layout and widths.
                // 상태바의 세로 치수만 관리하고 가로 배치와 너비는 Framework에 맡깁니다.
                int contentHeight = Pixels(status, 16);
                if (status.IsHandleCreated)
                    using (var graphics = status.CreateGraphics())
                        foreach (ToolStripItem item in status.Items)
                            contentHeight = Math.Max(contentHeight, TextRenderer.MeasureText(graphics, "Ag", item.Font,
                                new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine).Height);

                status.AutoSize = false;
                status.Padding = new Padding(Pixels(status, 1), Pixels(status, 1),
                    Pixels(status, status.SizingGrip ? 14 : 1), 0);
                status.Height = Math.Max(Pixels(status, DefaultStatusHeight), contentHeight + Pixels(status, 6));
                int labelHeight = status.Height - status.Padding.Vertical - Pixels(status, 3) - Pixels(status, 2);
                foreach (ToolStripItem item in status.Items)
                {
                    statusFonts[item] = item.Font;
                    if (!(item is ToolStripStatusLabel label)) continue;
                    label.Margin = new Padding(label.Margin.Left, Pixels(status, 3), label.Margin.Right, Pixels(status, 2));
                    label.Padding = new Padding(label.Padding.Left, 0, label.Padding.Right, 0);
                    label.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                    label.Height = labelHeight;
                }
                statusHeight = status.Height;
                statusLabelHeight = labelHeight;
                statusDpi = status.DeviceDpi;
            }

            private void UpdateNaturalImage(ToolStripItem item, Control metricsOwner)
            {
                if (item.ImageScaling != ToolStripItemImageScaling.None || item.Image == null) return;
                if (!naturalImages.TryGetValue(item, out var copy))
                {
                    copy = new ImageCopy(item.Image);
                    naturalImages.Add(item, copy);
                }
                else if (!ReferenceEquals(item.Image, copy.Original) && !ReferenceEquals(item.Image, copy.Scaled))
                {
                    copy.Scaled?.Dispose();
                    copy = new ImageCopy(item.Image);
                    naturalImages[item] = copy;
                }
                var size = new Size(Pixels(metricsOwner, copy.Original.Width),
                    Pixels(metricsOwner, copy.Original.Height));
                if (copy.Scaled != null && copy.Scaled.Size == size) return;
                var previous = copy.Scaled;
                copy.Scaled = null;
                if (size == copy.Original.Size) item.Image = copy.Original;
                else
                {
                    copy.Scaled = DpiImage.CreateCopy(copy.Original, size, item.ImageTransparentColor);
                    item.Image = copy.Scaled;
                }
                previous?.Dispose();
            }

            private void Disposed(object sender, EventArgs e)
            {
                statusFonts.Clear();
                foreach (var copy in naturalImages.Values) copy.Scaled?.Dispose();
                naturalImages.Clear();
            }
        }

        private sealed class ImageCopy
        {
            internal readonly Image Original;
            internal Bitmap Scaled;
            internal ImageCopy(Image original) => Original = original;
        }
    }

    /// <summary>Scales borrowed button images from their original pixel dimensions without altering control bounds.</summary>
    internal static class ButtonImageDpi
    {
        private static readonly ConditionalWeakTable<ButtonBase, Entry> entries = new ConditionalWeakTable<ButtonBase, Entry>();

        internal static void Attach(Control root)
        {
            if (SpiralLab.Sirius3.UI.WinForms.EditorControl.IsDesigner()) return;
            AttachChildren(root);
        }

        private static void AttachChildren(Control root)
        {
            foreach (Control child in root.Controls)
            {
                // Nested user controls own their DPI images, including controls from another assembly.
                if (child is UserControl) continue;
                if (child is ButtonBase button && button.ImageList == null && button.Image != null && !entries.TryGetValue(button, out _))
                    entries.Add(button, new Entry(button));
                if (child.HasChildren) AttachChildren(child);
            }
        }

        private sealed class Entry
        {
            private readonly ButtonBase button;
            private Image original;
            private Bitmap scaled;
            private bool updating;

            internal Entry(ButtonBase button)
            {
                this.button = button;
                original = button.Image;
                button.HandleCreated += Update;
                button.DpiChangedAfterParent += Update;
                button.Invalidated += Invalidated;
                button.Disposed += Disposed;
                Update(null, EventArgs.Empty);
            }

            private void Invalidated(object sender, InvalidateEventArgs e) => Update(sender, EventArgs.Empty);

            private void Update(object sender, EventArgs e)
            {
                if (updating || button.IsDisposed || button.Disposing || button.ImageList != null) return;
                updating = true;
                try
                {
                    if (!ReferenceEquals(button.Image, original) && (scaled == null || !ReferenceEquals(button.Image, scaled)))
                    {
                        original = button.Image;
                        scaled?.Dispose();
                        scaled = null;
                    }
                    if (original == null) return;
                    var size = new Size(Math.Max(1, (int)Math.Round(original.Width * button.DeviceDpi / 96d)),
                        Math.Max(1, (int)Math.Round(original.Height * button.DeviceDpi / 96d)));
                    if (size == original.Size && ReferenceEquals(button.Image, original)) return;
                    if (scaled != null && scaled.Size == size && ReferenceEquals(button.Image, scaled)) return;
                    var replacement = size == original.Size ? null : DpiImage.CreateCopy(original, size, Color.Empty);
                    var previous = scaled;
                    scaled = replacement;
                    button.Image = scaled ?? original;
                    previous?.Dispose();
                }
                finally { updating = false; }
            }

            private void Disposed(object sender, EventArgs e)
            {
                button.HandleCreated -= Update;
                button.DpiChangedAfterParent -= Update;
                button.Invalidated -= Invalidated;
                button.Disposed -= Disposed;
                scaled?.Dispose();
                scaled = null;
                original = null;
            }
        }
    }

    /// <summary>Measures tab captions and rebuilds tab icons from retained 96-DPI sources.</summary>
    internal static class TabControlDpi
    {
        private static readonly ConditionalWeakTable<TabControl, Entry> entries = new ConditionalWeakTable<TabControl, Entry>();

        internal static void Attach(Control root)
        {
            if (SpiralLab.Sirius3.UI.WinForms.EditorControl.IsDesigner()) return;
            foreach (Control child in root.Controls)
            {
                // Nested user controls attach their own tab support.
                if (child is UserControl) continue;
                if (child is TabControl tab && !entries.TryGetValue(tab, out _))
                    entries.Add(tab, new Entry(tab));
                if (child.HasChildren) Attach(child);
            }
        }

        private static int Pixels(TabControl tab, int logicalSize)
            => (int)Math.Round(logicalSize * tab.DeviceDpi / 96d);

        private static Size MeasureText(TabControl tab, string text)
        {
            using (var graphics = tab.CreateGraphics())
                return TextRenderer.MeasureText(graphics, text, tab.Font,
                    new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);
        }

        private sealed class Entry
        {
            private readonly TabControl tab;
            private readonly Size logicalItemSize;
            private readonly Point logicalPadding;
            private readonly ImageList original;
            private readonly Bitmap[] sources;
            private readonly string[] keys;
            private readonly Size logicalImageSize;
            private ImageList scaled;
            private bool updating;
            private bool updatePending;
            private int measuredDpi;
            private Font measuredFont;
            private ImageList measuredImages;
            private Size measuredImageSize;
            private TabSizeMode measuredSizeMode;
            private Size requestedItemSize;
            private int measuredPageCount;

            internal Entry(TabControl tab)
            {
                this.tab = tab;
                logicalItemSize = tab.ItemSize;
                logicalPadding = tab.Padding;
                original = tab.ImageList;
                if (original != null)
                {
                    logicalImageSize = original.ImageSize;
                    sources = new Bitmap[original.Images.Count];
                    for (int i = 0; i < sources.Length; i++)
                        using (var image = original.Images[i]) sources[i] = new Bitmap(image);
                    keys = original.Images.Keys.Cast<string>().ToArray();
                }
                tab.HandleCreated += HandleCreated;
                tab.DpiChangedAfterParent += QueueUpdate;
                tab.FontChanged += QueueUpdate;
                tab.Invalidated += Invalidated;
                tab.ControlAdded += PageAdded;
                tab.ControlRemoved += PageRemoved;
                tab.Disposed += Disposed;
                foreach (TabPage page in tab.TabPages) page.TextChanged += QueueUpdate;
            }

            private void Invalidated(object sender, InvalidateEventArgs e)
            {
                if (measuredDpi != tab.DeviceDpi || !ReferenceEquals(measuredFont, tab.Font)
                    || !ReferenceEquals(measuredImages, tab.ImageList)
                    || measuredImageSize != (tab.ImageList?.ImageSize ?? Size.Empty)
                    || measuredSizeMode != tab.SizeMode
                    || measuredPageCount != tab.TabCount
                    || (tab.SizeMode == TabSizeMode.Fixed && tab.ItemSize != requestedItemSize))
                    QueueUpdate(sender, EventArgs.Empty);
            }
            private void HandleCreated(object sender, EventArgs e) => QueueUpdate(sender, e);

            private void QueueUpdate(object sender, EventArgs e)
            {
                if (updatePending || updating || tab.IsDisposed || tab.Disposing || !tab.IsHandleCreated) return;
                updatePending = true;
                // Native tab size/image changes trigger parent layout. Defer them until
                // the parent's DPI scaling and native page restoration have finished.
                tab.BeginInvoke(new Action(() =>
                {
                    updatePending = false;
                    Update(tab, EventArgs.Empty);
                }));
            }
            private void PageAdded(object sender, ControlEventArgs e)
            {
                if (e.Control is TabPage page) page.TextChanged += QueueUpdate;
                QueueUpdate(sender, EventArgs.Empty);
            }
            private void PageRemoved(object sender, ControlEventArgs e)
            {
                if (e.Control is TabPage page) page.TextChanged -= QueueUpdate;
                QueueUpdate(sender, EventArgs.Empty);
            }

            private void Update(object sender, EventArgs e)
            {
                if (updating || !tab.IsHandleCreated || tab.IsDisposed || tab.Disposing) return;
                int updateDpi = tab.DeviceDpi;
                updating = true;
                try
                {
                    if (original != null && (ReferenceEquals(tab.ImageList, original) || ReferenceEquals(tab.ImageList, scaled)))
                    {
                        var size = new Size(Math.Max(1, Math.Min(256, Pixels(tab, logicalImageSize.Width))),
                            Math.Max(1, Math.Min(256, Pixels(tab, logicalImageSize.Height))));
                        if (scaled == null || scaled.ImageSize != size)
                        {
                            var replacement = new ImageList { ImageSize = size, ColorDepth = ColorDepth.Depth32Bit, TransparentColor = original.TransparentColor };
                            var copies = new List<Bitmap>();
                            try
                            {
                                for (int i = 0; i < sources.Length; i++)
                                {
                                    var copy = DpiImage.CreateCopy(sources[i], size, original.TransparentColor);
                                    copies.Add(copy);
                                    replacement.Images.Add(keys[i], copy);
                                }
                                // ImageList copies lazily; realize it before disposing source copies.
                                var handle = replacement.Handle;
                                var previous = scaled;
                                scaled = replacement;
                                tab.ImageList = scaled;
                                previous?.Dispose();
                            }
                            catch { replacement.Dispose(); throw; }
                            finally { foreach (var copy in copies) copy.Dispose(); }
                        }
                    }
                    tab.Padding = new Point(Pixels(tab, logicalPadding.X),
                        Pixels(tab, logicalPadding.Y));
                    int width = Pixels(tab, logicalItemSize.Width);
                    int height = Pixels(tab, logicalItemSize.Height);
                    foreach (TabPage page in tab.TabPages)
                    {
                        var text = MeasureText(tab, page.Text);
                        bool hasImage = tab.ImageList != null && (tab.ImageList.Images.ContainsKey(page.ImageKey)
                            || (page.ImageIndex >= 0 && page.ImageIndex < tab.ImageList.Images.Count));
                        int imageWidth = hasImage ? tab.ImageList.ImageSize.Width : 0;
                        int imageHeight = hasImage ? tab.ImageList.ImageSize.Height : 0;
                        width = Math.Max(width, text.Width + imageWidth + tab.Padding.X * 2
                            + Pixels(tab, hasImage ? 6 : 2));
                        height = Math.Max(height, Math.Max(text.Height, imageHeight) + tab.Padding.Y * 2
                            + Pixels(tab, 2));
                    }
                    var itemSize = new Size(width, height);
                    if (tab.ItemSize != itemSize) tab.ItemSize = itemSize;
                    measuredDpi = tab.DeviceDpi;
                    measuredFont = tab.Font;
                    measuredImages = tab.ImageList;
                    measuredImageSize = tab.ImageList?.ImageSize ?? Size.Empty;
                    measuredSizeMode = tab.SizeMode;
                    requestedItemSize = itemSize;
                    measuredPageCount = tab.TabCount;
                }
                finally { updating = false; }
                // Padding can recreate the handle at a newer native DPI while this
                // update still owns images measured before that recreation.
                if (tab.DeviceDpi != updateDpi) QueueUpdate(tab, EventArgs.Empty);
            }

            private void Disposed(object sender, EventArgs e)
            {
                tab.HandleCreated -= HandleCreated;
                tab.DpiChangedAfterParent -= QueueUpdate;
                tab.FontChanged -= QueueUpdate;
                tab.Invalidated -= Invalidated;
                tab.ControlAdded -= PageAdded;
                tab.ControlRemoved -= PageRemoved;
                tab.Disposed -= Disposed;
                foreach (TabPage page in tab.TabPages) page.TextChanged -= QueueUpdate;
                scaled?.Dispose();
                if (sources != null) foreach (var source in sources) source.Dispose();
            }
        }
    }

    internal static class DpiImage
    {
        internal static Bitmap CreateCopy(Image original, Size size, Color transparentColor)
        {
            var copy = new Bitmap(size.Width, size.Height);
            try
            {
                using (var graphics = Graphics.FromImage(copy))
                using (var source = new Bitmap(original))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    if (transparentColor != Color.Empty) source.MakeTransparent(transparentColor);
                    graphics.DrawImage(source, new Rectangle(Point.Empty, size));
                }
                return copy;
            }
            catch { copy.Dispose(); throw; }
        }
    }


    internal static class ChartInteractionHelper
    {
        internal static int StyleSize(Chart chart, int logicalPixels)
        {
#if NETFRAMEWORK
            return (int)Math.Round(logicalPixels * chart.DeviceDpi / 96d);
#else
            return logicalPixels;
#endif
        }

        internal const double ZoomFactor = 0.8;

        internal static bool TryBeginInvoke(Control control, MethodInvoker action)
        {
            if (null == control || null == action || control.IsDisposed || control.Disposing || !control.IsHandleCreated)
                return false;

            try
            {
                control.BeginInvoke(action);
                return true;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        internal static bool TryGetPlotBounds(Chart chart, out RectangleF plotBounds)
        {
            plotBounds = RectangleF.Empty;
            if (null == chart || 0 == chart.ChartAreas.Count)
                return false;

            var chartArea = chart.ChartAreas[0];
            double xMinimum = chartArea.AxisX.ScaleView.ViewMinimum;
            double xMaximum = chartArea.AxisX.ScaleView.ViewMaximum;
            double yMinimum = chartArea.AxisY.ScaleView.ViewMinimum;
            double yMaximum = chartArea.AxisY.ScaleView.ViewMaximum;
            if (!IsFinitePositive(xMaximum - xMinimum) || !IsFinitePositive(yMaximum - yMinimum))
                return false;

            try
            {
                float x1 = (float)chartArea.AxisX.ValueToPixelPosition(xMinimum);
                float x2 = (float)chartArea.AxisX.ValueToPixelPosition(xMaximum);
                float y1 = (float)chartArea.AxisY.ValueToPixelPosition(yMinimum);
                float y2 = (float)chartArea.AxisY.ValueToPixelPosition(yMaximum);
                plotBounds = RectangleF.FromLTRB(
                    Math.Min(x1, x2),
                    Math.Min(y1, y2),
                    Math.Max(x1, x2),
                    Math.Max(y1, y2));
                return plotBounds.Width > 0 && plotBounds.Height > 0;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        internal static void ZoomAxis(Axis axis, double anchor, bool zoomIn, double fullMinimum, double fullMaximum)
        {
            double viewMinimum = axis.ScaleView.ViewMinimum;
            double viewMaximum = axis.ScaleView.ViewMaximum;
            double viewRange = viewMaximum - viewMinimum;
            double fullRange = fullMaximum - fullMinimum;
            if (!IsFinite(anchor) || !IsFinitePositive(viewRange) || !IsFinitePositive(fullRange))
                return;

            double targetRange = viewRange * (zoomIn ? ZoomFactor : 1.0 / ZoomFactor);
            if (!zoomIn && targetRange >= fullRange)
            {
                axis.ScaleView.ZoomReset(0);
                return;
            }

            double anchorRatio = (anchor - viewMinimum) / viewRange;
            anchorRatio = Math.Max(0, Math.Min(1, anchorRatio));
            double targetMinimum = anchor - targetRange * anchorRatio;
            double targetMaximum = targetMinimum + targetRange;

            if (targetMinimum < fullMinimum)
            {
                targetMaximum += fullMinimum - targetMinimum;
                targetMinimum = fullMinimum;
            }
            if (targetMaximum > fullMaximum)
            {
                targetMinimum -= targetMaximum - fullMaximum;
                targetMaximum = fullMaximum;
            }

            if (targetMaximum > targetMinimum)
                axis.ScaleView.Zoom(targetMinimum, targetMaximum);
        }

        internal static bool TryGetNearestPoint(
            Chart chart,
            Point mouseLocation,
            double hoverRadius,
            out Series nearestSeries,
            out DataPoint nearestPoint)
        {
            nearestSeries = null;
            nearestPoint = null;
            if (!IsFinitePositive(hoverRadius) ||
                !TryGetPlotBounds(chart, out var plotBounds) ||
                !plotBounds.Contains(mouseLocation))
            {
                return false;
            }

            var chartArea = chart.ChartAreas[0];
            double nearestDistanceSquared = hoverRadius * hoverRadius;

            try
            {
                double mouseXValue = chartArea.AxisX.PixelPositionToValue(mouseLocation.X);
                if (!IsFinite(mouseXValue))
                    return false;

                foreach (Series series in chart.Series)
                {
                    if (!series.Enabled || !string.Equals(series.ChartArea, chartArea.Name, StringComparison.Ordinal))
                        continue;

                    int firstRightIndex = FindFirstPointAtOrAfter(series.Points, mouseXValue);
                    for (int i = firstRightIndex - 1; i >= 0; i--)
                    {
                        var point = series.Points[i];
                        if (!TryGetPointPixelPosition(chartArea, point, out double pointX, out double pointY))
                            continue;

                        if (pointX < mouseLocation.X - hoverRadius)
                            break;

                        UpdateNearestPoint(
                            series,
                            point,
                            pointX,
                            pointY,
                            mouseLocation,
                            ref nearestDistanceSquared,
                            ref nearestSeries,
                            ref nearestPoint);
                    }

                    for (int i = firstRightIndex; i < series.Points.Count; i++)
                    {
                        var point = series.Points[i];
                        if (!TryGetPointPixelPosition(chartArea, point, out double pointX, out double pointY))
                            continue;

                        if (pointX > mouseLocation.X + hoverRadius)
                            break;

                        UpdateNearestPoint(
                            series,
                            point,
                            pointX,
                            pointY,
                            mouseLocation,
                            ref nearestDistanceSquared,
                            ref nearestSeries,
                            ref nearestPoint);
                    }
                }
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            return null != nearestPoint;
        }

        private static int FindFirstPointAtOrAfter(DataPointCollection points, double xValue)
        {
            int low = 0;
            int high = points.Count;
            while (low < high)
            {
                int middle = low + (high - low) / 2;
                if (points[middle].XValue < xValue)
                    low = middle + 1;
                else
                    high = middle;
            }
            return low;
        }

        private static bool TryGetPointPixelPosition(
            ChartArea chartArea,
            DataPoint point,
            out double pointX,
            out double pointY)
        {
            pointX = double.NaN;
            pointY = double.NaN;
            if (null == point || point.IsEmpty || 0 == point.YValues.Length ||
                !IsFinite(point.XValue) || !IsFinite(point.YValues[0]))
            {
                return false;
            }

            pointX = chartArea.AxisX.ValueToPixelPosition(point.XValue);
            pointY = chartArea.AxisY.ValueToPixelPosition(point.YValues[0]);
            return IsFinite(pointX) && IsFinite(pointY);
        }

        private static void UpdateNearestPoint(
            Series series,
            DataPoint point,
            double pointX,
            double pointY,
            Point mouseLocation,
            ref double nearestDistanceSquared,
            ref Series nearestSeries,
            ref DataPoint nearestPoint)
        {
            double deltaX = pointX - mouseLocation.X;
            double deltaY = pointY - mouseLocation.Y;
            double distanceSquared = deltaX * deltaX + deltaY * deltaY;
            if (distanceSquared > nearestDistanceSquared)
                return;

            nearestDistanceSquared = distanceSquared;
            nearestSeries = series;
            nearestPoint = point;
        }

        internal static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        internal static bool IsFinitePositive(double value)
        {
            return IsFinite(value) && value > 0;
        }
    }

    internal static class EditorControlDispatch
    {
        internal static void Run(Control control, MethodInvoker update)
        {
            if (control.IsDisposed || control.Disposing || !control.IsHandleCreated)
                return;

            if (control.InvokeRequired)
            {
                ChartInteractionHelper.TryBeginInvoke(control, () => Run(control, update));
                return;
            }

            if (control.IsDisposed || control.Disposing || !control.IsHandleCreated)
                return;

            update();
        }
    }
}

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
            return string.IsNullOrEmpty(description)
                ? speed
                : description + Environment.NewLine + speed;
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
