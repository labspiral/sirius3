# System Log Monitoring & Logger Events User Manual

> Reference version: Sirius3 1.12.3 (public Release features)


## 1. Overview

`LogControl` collects Sirius3 system events, errors, warnings, and diagnostic messages in real time and displays them in the application UI.
It safely collects logs in a multi-cred environment, providing color distinction and filtration functions to increase additivity.

## 2. Main UI Features (UI Features)

- View of the list (ListView):
  - Time: Log occurrence time (HH:mm:ss.fff unit).
  - Level: Debug, Information, Warning, Error, Critical.
  - Message: The details occurred. (more messages supported)
- Filtering and searching:
  - Category Filter: You can only select logs for a specific level (e.g. See only Error).
  - Text Search (Search): Filters only logs that contain a specific keyword in the message content in real time (acts when entering more than 2 letters)
- The manipulation button:
  - Clear: Empty all the current screen logs and waiting logs.
  - Open Folder: Open the hard disk folder where the log file is actually stored.
  - Ctrl + C: Copy the selected log items into the clipboard as a text in the tab separator form.

## 3. Logger Event Architecture

The log system in Sirius3 follows the 'Pub-Sub' model.

- Events occur: When you call `Logger.Log(LogLevel, "message")` anywhere within the framework, logs are generated.
- Event subscription: `LogControl` subscribes to the `SpiralLab.Sirius3.Config.OnLogged` event at the time of loading.
  - `OnLogged` Delegate Format: `Action<LogLevel, string>`
- Thread Safety (Thread Safety):
  - Processing markers or hardware control thread operate separately from the UI thread.
  - Log messages will be saved immediately in `ConcurrentQueue` safely.
  - The UI Update Timer (100ms cycle) empty the curve from the UI thread and renews the screen, allowing a stable output without cross-trade exception.

## 4. Log Level and Color Guide (Log Levels)

- Error / Critical (Red): situations where system stops, hardware errors, communication failures, etc. require immediate action.
- Warning (Yellow): notice during processing, potential problems, unusual settings notifications.
- Information (Default): Normal status information, such as normal work start/out, changes to settings.
- Debug (Default): detailed tracking information for development and problem analysis.

## 5. The main setup parameters (Configurations)

The behavior of the logs is determined by the following properties of the `SpiralLab.Sirius3.Config` class.
- MaxLogItems: the maximum number of logs to keep on the screen. when exceeding it will be automatically deleted from the oldest logs.
- LogPath: The path where log files are physically recorded.

## 6. Cautions

- Big logs occur: If more than a thousand logs per second occur continuously, it can affect the UI rendering performance, so manage your log level with the urgent need of information.
- Search mode: If the search word is entered, a new log may not appear on the screen if it does not meet the conditions. To view all logs, empty the search window.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
