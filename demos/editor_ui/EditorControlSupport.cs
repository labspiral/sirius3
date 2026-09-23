/*
 * 2026 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : Shared support for the public WinForms and WPF editor controls
 */

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SpiralLab.Sirius3.Localization;
using SpiralLab.Sirius3.Remote;
using SpiralLab.Sirius3.Scanner.Rtc;

namespace Demos
{
    internal static class ChartInteractionHelper
    {
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
