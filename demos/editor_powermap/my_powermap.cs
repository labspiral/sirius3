
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Scanner.Rtc;
using Microsoft.Extensions.Logging;
using SpiralLab.Sirius3.PowerMap;
using SpiralLab.Sirius3;

#if OPENTK3
using OpenTK;
using DVec2 = OpenTK.Vector2d;
using DVec3 = OpenTK.Vector3d;
using DVec4 = OpenTK.Vector4d;
using DMat3 = OpenTK.Matrix3d;
using DMat4 = OpenTK.Matrix4d;
#elif OPENTK4
using OpenTK.Mathematics;
using DVec2 = OpenTK.Mathematics.Vector2d;
using DVec3 = OpenTK.Mathematics.Vector3d;
using DVec4 = OpenTK.Mathematics.Vector4d;
using DMat3 = OpenTK.Mathematics.Matrix3d;
using DMat4 = OpenTK.Mathematics.Matrix4d;
#endif

namespace Demos
{
    /// <summary>
    /// MyPowerMap  
    /// </summary>
    public class MyPowerMap : PowerMapBase
    {
        bool isTerminated = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="index">Zero-based unique identifier. <para>0부터 시작하는 고유 식별자입니다.</para><para>从 0 开始的唯一标识符。</para></param>
        /// <param name="name">Descriptive name. <para>장치 이름입니다.</para><para>设备名称。</para></param>
        public MyPowerMap(int index, string name)
            : base(index, name)
        {
            this.IsReady = true;
            this.IsBusy = false;
            this.IsError = false;
        }

        /// <inheritdoc/>
        public override bool CtlMapping(string[] categories, double[] xWatts)
        {
            if (this.Scanner == null || this.Laser == null || this.PowerMeter == null)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start mapping power. assign scanner, laser and powermeter at first");
                this.NotifyMappingFailed();
                return false;
            }
            if (this.IsBusy)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start mapping power. it's busy running ...");
                this.NotifyMappingFailed();
                return false;
            }
            if (null == categories || 0 == categories.Length)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start mapping power. target categories is not valid");
                this.NotifyMappingFailed();
                return false;
            }
            foreach (var category in categories)
            {
                // For example, consider category as frequency
                if (!double.TryParse(category, out double hz))
                {
                    this.IsError = true;
                    this.IsReady = false;
                    Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start mapping power. target category is not valid hz: {category}");
                    this.NotifyMappingFailed();
                    return false;
                }
            }
            if (null == xWatts || xWatts.Length < 2)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start mapping power. target watts is not valid. counts= {xWatts.Length}");
                this.NotifyMappingFailed();
                return false;
            }
            if (PowerMeter.IsError)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start mapping power. invalid powermeter status");
                this.NotifyMappingFailed();
                return false;
            }
            if (Laser.IsBusy || Laser.IsError || !Laser.IsReady)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start mapping power. invalid laser status");
                this.NotifyMappingFailed();
                return false;
            }
            var rtc = Scanner as IRtc;
            if (rtc.CtlGetStatus(RtcStatus.Busy))
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start mapping power. rtc status is invalid (busy ?)");
                this.NotifyMappingFailed();
                return false;
            }
            var powerControl = Laser as ILaserPowerControl;
            if (null == powerControl)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start mapping power. laser is not support power control function");
                this.NotifyMappingFailed();
                return false;
            }

            Array.Sort(xWatts); //ascending sort
            return this.DoPowerMapping(categories, xWatts);
        }

        /// <summary>
        /// Internal routine to perform power mapping operation.
        /// <para>파워 매핑 작업을 수행하는 내부 루틴입니다.</para>
        /// <para>执行功率映射操作的内部例程。</para>
        /// </summary>
        /// <param name="categories">The array of category names (e.g., frequency values). <para>카테고리 이름 배열입니다(예: 주파수 값).</para></param>
        /// <param name="xWatts">The array of target output power in Watts (X-axis). <para>목표 출력 파워 배열(와트 단위, X축)입니다.</para></param>
        /// <returns><c>true</c> if the mapping operation was successful; otherwise, <c>false</c>.</returns>
        protected virtual bool DoPowerMapping(string[] categories, double[] xWatts)
        {
            bool success = true;
            var powerControl = Laser as ILaserPowerControl;

            Task.Run(() =>
            {
                this.IsBusy = true;
                this.IsReady = false;
                isTerminated = false;

                this.NotifyMappingStarted();
                var rtc = Scanner as IRtc;
                success &= rtc.CtlMoveTo(Location);
                Thread.Sleep(100);
                PowerMeter.CtlClear();

                double maxWatt = xWatts[xWatts.Length - 1];

                foreach (var category in categories)
                {
                    Logger.Log(LogLevel.Warning, $"powermap [{this.Index}]: trying to start mapping power at target category: {category}");
                    this.Clear(category);
                    success &= PowerMeter.CtlStart(category);
                    // For example, consider category as frequency
                    var hz = double.Parse(category);
                    success &= rtc.CtlFrequency(hz, 2);
                    var sw = Stopwatch.StartNew();
                    bool isPreHeated = false;

                    foreach (var targetWatt in xWatts)
                    {
                        if (this.isTerminated || rtc.CtlGetStatus(RtcStatus.Aborted) || Laser.IsError || PowerMeter.IsError)
                        {
                            success &= false;
                            break;
                        }
                        success &= powerControl.CtlPower(targetWatt, string.Empty); //set raw power without mapped power
                        if (!isTerminated)
                            success &= rtc.CtlLaserOn();

                        long delayTime = Config.PowerMapHoldTimeMs;
                        if (!isPreHeated)
                        {
                            delayTime = Config.PowerMapPreHeatTimeMs;
                            isPreHeated = true;
                        }
                        sw.Restart();
                        do
                        {
                            if (isTerminated)
                            {
                                success &= false;
                                break;
                            }
                            Thread.Sleep(50);
                        } while (sw.ElapsedMilliseconds < delayTime);
                        double detectedWatt = PowerMeter.MeasuredPower; //read last measured data 
                        success &= rtc.CtlLaserOff();
                        if (!success)
                            break;
                        if (isTerminated)
                            break;
                        double inRangeWatt = targetWatt * Config.PowerMapInRangeThreshold / 100.0f;
                        if (inRangeWatt > 0)
                        {
                            if (Math.Abs(targetWatt - detectedWatt) > inRangeWatt)
                            {
                                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: mapping out of range threshold: {Config.PowerMapInRangeThreshold:F1}%, target: {targetWatt:F3}W, detected: {detectedWatt:F3}W at category: {category}");
                                success &= false;
                                break;
                            }
                        }
                        success &= this.Update(category, targetWatt, detectedWatt);
                        Logger.Log(LogLevel.Information, $"powermap [{this.Index}]: mapping target: {targetWatt:F3}W, detected: {detectedWatt:F3}W at category: {category}");
                        NotifyMappingProgress(category, targetWatt);
                        if (!success)
                            break;
                    }
                    if (!success)
                        break;
                }
                success &= rtc.CtlLaserOff();
                success &= PowerMeter.CtlStop();

                rtc.CtlMoveTo(DVec2.Zero);
                this.IsBusy = false;
                if (success && !isTerminated)
                {
                    this.IsReady = true;
                    Logger.Log(LogLevel.Information, $"powermap [{this.Index}]: success to mapping power");
                    this.NotifyMappingFinished();
                }
                else
                {
                    this.IsError = true;
                    this.IsReady = false;
                    Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to mapping power");
                    this.NotifyMappingFailed();
                }
            });
            return success;
        }

        /// <inheritdoc/>
        public override bool CtlVerify(KeyValuePair<string, double>[] categoryAndYWatts)
        {
            if (this.Scanner == null || this.Laser == null || this.PowerMeter == null)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start verify power. assign scanner, laser and powermeter at first");
                this.NotifyVerifyFailed();
                return false;
            }
            var rtc = Scanner as IRtc;
            if (this.IsBusy)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start verify power. it's busy running ...");
                this.NotifyVerifyFailed();
                return false;
            }
            if (null == categoryAndYWatts || 0 == categoryAndYWatts.Length)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start verify power. target category is not valid");
                this.NotifyVerifyFailed();
                return false;
            }
            foreach (var kv in categoryAndYWatts)
            {
                // For example, consider category as frequency
                if (!double.TryParse(kv.Key, out double hz))
                {
                    this.IsError = true;
                    this.IsReady = false;
                    Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start verify power. target category is not valid hz: {kv.Key}");
                    this.NotifyVerifyFailed();
                    return false;
                }
            }
            if (PowerMeter.IsError)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start verify power. invalid powermeter status");
                this.NotifyVerifyFailed();
                return false;
            }
            if (Laser.IsBusy || Laser.IsError || !Laser.IsReady)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start verify power. invalid laser status");
                this.NotifyVerifyFailed();
                return false;
            }
            if (rtc.CtlGetStatus(RtcStatus.Busy))
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start verify power. rtc status is invalid (busy ?)");
                this.NotifyVerifyFailed();
                return false;
            }
            var powerControl = Laser as ILaserPowerControl;
            if (null == powerControl)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start verify power. laser is not support power control function");
                this.NotifyVerifyFailed();
                return false;
            }
            return this.DoPowerVerify(categoryAndYWatts);
        }

        /// <summary>
        /// Internal routine to perform power verification operation.
        /// <para>파워 검증 작업을 수행하는 내부 루틴입니다.</para>
        /// <para>执行功率验证操作的内部例程。</para>
        /// </summary>
        /// <param name="categoryAndYWatts">The array of key-value pairs where key is the category and value is the target power in Watts. <para>키가 카테고리이고 값이 목표 파워(와트)인 키-값 쌍의 배열입니다.</para></param>
        /// <returns><c>true</c> if the verification operation was successful; otherwise, <c>false</c>.</returns>
        protected virtual bool DoPowerVerify(KeyValuePair<string, double>[] categoryAndYWatts)
        {
            bool success = true;
            var powerControl = Laser as ILaserPowerControl;

            Task.Run(() =>
            {
                this.IsBusy = true;
                this.IsReady = false;
                isTerminated = false;

                this.NotifyVerifyStarted();
                var rtc = Scanner as IRtc;
                rtc.CtlMoveTo(Location);
                Thread.Sleep(100);
                PowerMeter.CtlClear();

                var sw = Stopwatch.StartNew();
                var oldIsEnableLookUp = this.IsLookUpEnable;
                this.IsLookUpEnable = true;
                bool isPreHeated = false;
                foreach (var kv in categoryAndYWatts)
                {
                    Logger.Log(LogLevel.Warning, $"powermap [{this.Index}]: trying to start power verify. target category: {kv.Key}");
                    string category = kv.Key;
                    success &= PowerMeter.CtlStart(category);
                    double targetWatt = kv.Value;
                    double detectedWatt = 0;
                    // For example, consider category as frequency
                    double hz = double.Parse(category);
                    success &= rtc.CtlFrequency(hz, 2);
                    if (powerControl.CtlPower(targetWatt, category))
                    {
                        success &= rtc.CtlLaserOn();
                        sw.Restart();
                        long delayTime = Config.PowerMapHoldTimeMs;
                        if (!isPreHeated)
                        {
                            delayTime = Config.PowerMapPreHeatTimeMs;
                            isPreHeated = true;
                        }
                        do
                        {
                            if (rtc.CtlGetStatus(RtcStatus.Aborted))
                            {
                                success &= false;
                                break;
                            }
                            Thread.Sleep(50);
                        } while (sw.ElapsedMilliseconds < delayTime);
                        detectedWatt = PowerMeter.MeasuredPower;
                        success &= rtc.CtlLaserOff();
                        if (success)
                        {
                            double inRangeWatt = targetWatt * Config.PowerMapInRangeThreshold / 100.0f;
                            if (Math.Abs(targetWatt - detectedWatt) < inRangeWatt)
                            {
                                Logger.Log(LogLevel.Information, $"powermap [{this.Index}]: verify in range target: {targetWatt:F3} - detected: {detectedWatt:F3}W < threshold: {Config.PowerMapInRangeThreshold}% at category: {category}");
                                this.NotifyVerifyProgress(category, targetWatt);
                            }
                            else
                            {
                                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: verify out of range threshold: {Config.PowerMapInRangeThreshold:F1}%, target: {targetWatt:F3}W, detected: {detectedWatt:F3}W at category: {category}");
                                success &= false;
                            }
                        }
                        if (!success)
                            break;
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to change target output power: {targetWatt:F3}W. target category: {kv.Key}");
                        success &= false;
                    }
                    if (!success)
                        break;
                }
                success &= rtc.CtlLaserOff();
                success &= PowerMeter.CtlStop();
                Scanner.CtlMoveTo(DVec2.Zero);
                this.IsLookUpEnable = oldIsEnableLookUp;
                this.IsBusy = false;
                if (success)
                {
                    this.IsReady = true;
                    this.NotifyVerifyFinished();
                }
                else
                {
                    this.IsError = true;
                    this.IsReady = false;
                    this.NotifyVerifyFailed();
                }
            });
            return success;
        }

        /// <inheritdoc/>
        public override bool CtlCompensate(KeyValuePair<string, double>[] categoryAndYWatts)
        {
            if (this.Scanner == null || this.Laser == null || this.PowerMeter == null)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start compensate power. assign scanner, laser and powermeter at first");
                this.NotifyCompensateFailed();
                return false;
            }
            if (this.IsBusy)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start compensate power. it's busy running ...");
                this.NotifyCompensateFailed();
                return false;
            }
            if (null == categoryAndYWatts || 0 == categoryAndYWatts.Length)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start compensate power. target category is not valid");
                this.NotifyCompensateFailed();
                return false;
            }
            foreach (var kv in categoryAndYWatts)
            {
                // For example, consider category as frequency
                if (!double.TryParse(kv.Key, out double hz))
                {
                    this.IsError = true;
                    this.IsReady = false;
                    Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start compensate power. target category is not valid hz: {kv.Key}");
                    this.NotifyCompensateFailed();
                    return false;
                }
            }
            if (PowerMeter.IsError)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start compensate power. invalid powermeter status");
                this.NotifyCompensateFailed();
                return false;
            }
            if (Laser.IsBusy || Laser.IsError || !Laser.IsReady)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start compensate power. invalid laser status");
                this.NotifyCompensateFailed();
                return false;
            }
            var rtc = Scanner as IRtc;
            if (rtc.CtlGetStatus(RtcStatus.Busy))
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start compensate power. rtc status is invalid (busy ?)");
                this.NotifyCompensateFailed();
                return false;
            }
            var powerControl = Laser as ILaserPowerControl;
            if (null == powerControl)
            {
                this.IsError = true;
                this.IsReady = false;
                Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to start compensate power. laser is not support power control function");
                this.NotifyCompensateFailed();
                return false;
            }
            return this.DoPowerCompensate(categoryAndYWatts);
        }

        /// <summary>
        /// Internal routine to perform power compensation operation.
        /// <para>파워 보정 작업을 수행하는 내부 루틴입니다.</para>
        /// <para>执行功率补偿操作的内部例程。</para>
        /// </summary>
        /// <param name="categoryAndYWatts">The array of key-value pairs where key is the category and value is the target power in Watts. <para>키가 카테고리이고 값이 목표 파워(와트)인 키-값 쌍의 배열입니다.</para></param>
        /// <returns><c>true</c> if the compensation operation was successful; otherwise, <c>false</c>.</returns>
        protected virtual bool DoPowerCompensate(KeyValuePair<string, double>[] categoryAndYWatts)
        {
            bool success = true;
            var powerControl = Laser as ILaserPowerControl;

            Task.Run(() =>
            {
                this.IsBusy = true;
                this.IsReady = false;
                isTerminated = false;

                this.NotifyCompensateStarted();
                var rtc = Scanner as IRtc;
                rtc.CtlMoveTo(Location);
                Thread.Sleep(100);
                PowerMeter.CtlClear();
                var sw = Stopwatch.StartNew();
                bool isPreHeated = false;
                var oldIsEnableLookUp = this.IsLookUpEnable;
                this.IsLookUpEnable = true;
                int retryCounts = 0;

                for (int i = 0; i < categoryAndYWatts.Length; i++)
                {
                    var kv = categoryAndYWatts[i];
                    string category = kv.Key;
                    double targetWatt = kv.Value;
                    Logger.Log(LogLevel.Warning, $"powermap [{this.Index}]: trying to start power compensate. target: {targetWatt:F3}W at category: {category}");
                    success &= PowerMeter.CtlStart(category);
                    double detectedWatt = 0;
                    // For example, consider category as frequency
                    double hz = double.Parse(category);
                    success &= rtc.CtlFrequency(hz, 2);
                    success &= powerControl.CtlPower(targetWatt, category);
                    if (success)
                    {
                        success &= rtc.CtlLaserOn();
                        long delayTime = Config.PowerMapHoldTimeMs;
                        if (!isPreHeated)
                        {
                            delayTime = Config.PowerMapPreHeatTimeMs;
                            isPreHeated = true;
                        }
                        sw.Restart();
                        do
                        {
                            if (rtc.CtlGetStatus(RtcStatus.Aborted))
                            {
                                success &= false;
                                break;
                            }
                            Thread.Sleep(50);
                        } while (sw.ElapsedMilliseconds < delayTime);
                        detectedWatt = PowerMeter.MeasuredPower;
                        success &= rtc.CtlLaserOff();
                        if (success)
                        {
                            double inRangeWatt = targetWatt * Config.PowerMapInRangeThreshold / 100.0f;
                            if (Math.Abs(targetWatt - detectedWatt) <= inRangeWatt)
                            {
                                retryCounts = 0;
                                Logger.Log(LogLevel.Information, $"powermap [{this.Index}]: compensate in range target: {targetWatt:F3} - detected: {detectedWatt:F3}W < threshold: {Config.PowerMapInRangeThreshold}% at category: {category}");
                                this.NotifyCompensateProgress(category, targetWatt);
                            }
                            else
                            {
                                // APC (Automatic Power Compensate) - Method 2: Adaptive Update
                                double outOfRangeWatt = targetWatt * Config.PowerMapOutOfRangeThreshold / 100.0f;
                                if (outOfRangeWatt > 0 && Math.Abs(targetWatt - detectedWatt) > outOfRangeWatt)
                                {
                                    Logger.Log(LogLevel.Information, $"powermap [{this.Index}]: compensate out of range target: {targetWatt:F3} - detected: {detectedWatt:F3}W < threshold: {Config.PowerMapOutOfRangeThreshold}% at category: {category}");
                                    success &= false;
                                }
                                else if (retryCounts >= Config.PowerMapCompensateRetryCounts)
                                {
                                    Logger.Log(LogLevel.Information, $"powermap [{this.Index}]: compensating but failed to retry target: {targetWatt:F3} - detected: {detectedWatt:F3}W < threshold: {Config.PowerMapInRangeThreshold}% at category: {category}");
                                    success &= false;
                                }
                                else
                                {
                                    // Refine map based on latest measurement
                                    success &= powerControl.PowerMap.LookUp(category, targetWatt, out var currentXWatt, out var leftXWatt, out var rightXWatt);
                                    if (!success)
                                        break;

                                    Logger.Log(LogLevel.Warning, $"powermap [{this.Index}]: compensate out of range. target: {targetWatt:F3}W, detected: {detectedWatt:F3}W (diff > {Config.PowerMapInRangeThreshold}%) at category: {category}. Retry {++retryCounts}/{Config.PowerMapCompensateRetryCounts}");

                                    Logger.Log(LogLevel.Information, $"powermap [{this.Index}]: adaptive update x: {currentXWatt:F3} -> y: {detectedWatt:F3}W at category: {category}");
                                    success &= powerControl.PowerMap.Update(category, currentXWatt, detectedWatt);
                                    this.NotifyMappingProgress(category, currentXWatt);

                                    // Retry current target with refined map
                                    if (success)
                                        i--;
                                }
                            }
                        }
                        if (!success)
                            break;
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, $"powermap [{this.Index}]: fail to change target output power: {targetWatt:F3}W. target category: {kv.Key}");
                        success &= false;
                    }
                    if (!success)
                        break;
                }
                success &= rtc.CtlLaserOff();
                success &= PowerMeter.CtlStop();
                rtc.CtlMoveTo(DVec2.Zero);
                this.IsBusy = false;
                this.IsLookUpEnable = oldIsEnableLookUp;
                if (success)
                {
                    this.IsReady = true;
                    this.NotifyCompensateFinished();
                }
                else
                {
                    this.IsError = true;
                    this.IsReady = false;
                    this.NotifyCompensateFailed();
                }
            });
            return success;
        }

        /// <inheritdoc/>
        public override bool CtlStop()
        {
            bool success = true;
            isTerminated = true;

            Logger.Log(LogLevel.Debug, $"powermap [{this.Index}]: trying to stop");
            var rtc = Scanner as IRtc;
            if (null != Scanner && rtc.IsBusy)
            {
                success &= rtc.CtlAbort();
                success &= rtc.CtlLaserOff();
            }
            if (null != Laser && Laser.IsBusy)
                success &= Laser.CtlAbort();

            return success;
        }

        /// <inheritdoc/>
        public override bool CtlReset()
        {
            bool success = true;
            this.IsReady = true;
            this.IsError = false;

            Logger.Log(LogLevel.Warning, $"powermap [{this.Index}]: trying to reset");
            var rtc = Scanner as IRtc;
            if (null != Scanner)
                success &= rtc.CtlReset();
            if (null != Laser)
                success &= Laser.CtlReset();

            isTerminated = false;
            return success;
        }
    }
}
