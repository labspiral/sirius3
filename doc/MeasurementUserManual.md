# RTC real-time measurement and data analysis (Measurement & Plot)

> Reference version: Sirius3 1.12.3 (public Release features)


## 1. Overview

RTC4, RTC5, and RTC6 controllers can sample internal signals—such as scanner position, laser state, and error signals—in synchronization with scanner motion.
It provides the internal measurement buffer (Internal Measurement Buffer) function, which analyzes tracking errors and processes during super precision processing.
Quality verification (QA) is essential, etc.


## 2. Measurement Channels

You can select the signals to record through the MeasurementChannels column.
Each channel has a primary data (Raw) and a human-readable unit.

   - The main channel:
       - LaserOn: Laser output signal state (0 or 1)
       - SampleX, SampleY, SampleZ: Scanner's command coordinates (mm)
       - StatusAX, StatusAY: the actual feedback location of the scanner mirror (with iDRIVE, mm)
       - SampleAX_Corr, SampleAY_Corr: the output coordinates with correction applied
       - MarkSpeed: Current processing speed (mm/s)
       - Enc0Counter, Enc1Counter: MoF (Marking on the Fly)
       - ExtAO1, ExtAO2 / ExtDO: Analog/Digital Output Status

## 3. List Command (IRtcMeasurement)

Order to measure the data of a specific range within the processing list.

  ListMeasurementBegin(frequency, channels)
   - Description: Start the measurement.
   - The variable:
       - Frequency: sampling frequency (Hz). up to 100kHz (10μs resolution).
       - Channels: Channels to be recorded (RTC4: up to two, RTC5: four, RTC6: eight)

  ListMeasurementEnd()
   - Description: Closing the measurement.


## 4. Data storage and visualization (RtcMeasurementHelper)

Save the recorded data as a file or extract it as a graph.

The Data Saving (Save)
   - RtcMeasurementHelper.Save(fileName, rtcMeasurement)
   - The recorded data is stored in a .txt file. Time and the values converted into the physical units of each channel (mm, V, etc.) are recorded.

The Graph Plot (Plot)
   - RtcMeasurementHelper.Plot(fileName, plotMode, title)
   - Plot Modes (Plot Modes):
       1. TimeChart (0): X-Score is the value of the channels selected and X-Score is the value of the channels selected.
       2. PositionChart (1): The X-ray is SampleX, the X-ray is using SampleY to draw the laser processing path.
It is the light of the light and the light of the light and the light of the light and the light of the light.


## 5. Automatic processing in MarkerRtc

MarkerRtc class has the ability to automatically process measurement data after processing is completed.

IsMeasurementPlot properties
   - Description: After the processing is completed, determine whether the measurement data will be automatically drawn out into the graph.
   - Method of action (Plot = true):
       1. The processing list must include EntityMeasurementBegin and EntityMeasurementEnd.
       2. Once the processing is completed (NotifyEnded), check the measurement sessions stored in the sessionQueue within MarkerRtc.
       3. When the NotifyPlot() method is called, the Plot() window will automatically pop up for all the sessions stored.


## 6. Example Code (C#)


Basic list methods.

    1 var rtcMeasurement = rtc as IRtcMeasurement;
    2 var channels = new[] { MeasurementChannels.SampleX, MeasurementChannels.SampleY, MeasurementChannels.LaserOn };
    3
    4 rtc.ListBegin();
5 rtcMeasurement.ListMeasurementBegin(10000, channels); // 10kHz sampling start
    6
7 // Processing entities...
    8 rtc.ListJumpTo(new DVec2(0, 0));
    9 rtc.ListMarkTo(new DVec2(10, 10));
   10
11 rtcMeasurement.ListMeasurementEnd(); // End measurement
   12 rtc.ListEnd();
   13 rtc.ListExecute(true);
   14
15 // Results storage and graph output
   16 string filePath = "measurement_result.txt";
   17 RtcMeasurementHelper.Save(filePath, rtcMeasurement);
   18 RtcMeasurementHelper.Plot(filePath, PlotModes.PositionChart, "Laser Path Analysis");

When using MarkerRtc (UI environment)

1 // MarkerRtc setup
2 markerRtc.IsMeasurementPlot = true; // Automatic graph output after processing is activated
   3
4 // Processing start (internally call NotifyPlot after LayerWork)
   5 await markerRtc.StartAsync();


## 7. Cautions

   - Sampling frequency: Too high frequency (100kHz) can exceed the RTC internal buffer size limit during long-time measurements.
   - PositionChart required channels: To use PositionChart mode, the measurement channel must contain SampleX, SampleY, and LaserOn.
   - iDRIVE Temperature: CtlGetTemperatureStateValues allows you to real-time check the galvanic and server board temperatures of the scanner (only for the iDRIVE scanner)
