# Sirius3

SCANLAB 制御、デバイス連携、ジオメトリ処理、OpenGL 可視化、ドキュメント編集、組み込み MCP サーバーを利用した AI プロンプト、シミュレーション、マーキング実行を統合した Windows/.NET 向け精密レーザー加工プラットフォームです。

[English](README.md) | [한국어](README.koKR.md) | [简体中文](README.zhCN.md) | 日本語 | [Deutsch](README.deDE.md)

![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_logo.png)

---

## 主な特長

![sirius3_logo1](https://spirallab.co.kr/sirius3/sirius3_logo1.png)
![sirius3_editor](https://spirallab.co.kr/sirius3/sirius3_editor.png)

- SCANLAB RTC コントローラー
  - RTC4 / RTC4e / RTC5 / RTC6 / RTC6e
  - XL-SCAN（syncAXIS による RTC6 + ACS）
- 計測とプロファイリング
  - スキャナー軌跡および出力信号の記録とグラフ表示
  - 加工経路のリアルタイムシミュレーション
- 高度なマーキング機能
  - 可変 Polygon Delay / Jump Delay
  - セカンドヘッド、3D
  - MoF（Marking on the Fly）および MoF Extension
  - Sky Writing Mode 1/2/3/4
  - SCANAhead による Auto Delays
  - MultiBeam（レーザー光源 1 台 + AOM 2 台 + スキャンヘッド 2 台）
- ALC（Automatic Laser Control）/ Pulse on Demand
  - Defined Vector と Ramp
  - 指令速度または実速度への依存
  - エンコーダー速度への依存
  - 距離とスケール係数を用いる位置依存テーブル
  - SCANAhead、Encoder Speed Addition、Inverse Speed Correction、Backward Transformation、SDC + Skywriting の組み合わせ
- スキャナーフィールド補正
  - 2D 補正
  - 傾き、フォーカス、A/B/C 係数、Stretch Factor の 3D 補正
- レーザー出力制御
  - 周波数、デューティ比、アナログ出力、デジタル出力
  - AdvancedOptoWave、Coherent、IPG、JPT、Photonics Industry、Spectra Physics などを統合
- パワーメーターと PowerMap
  - Coherent PowerMax、Thorlabs（OPM）、Ophir（StarLab）
  - PowerMap に基づく出力補償
- レンダリングとジオメトリ処理
  - 1 台の正投影カメラと 5 台の透視カメラを備えた OpenGL 3.3+ 2D/3D レンダラー
  - 点、線、ラインストリップ、三角形のヒットテストを高速化する AABB
  - 開閉輪郭診断を備えたトポロジー対応 3D メッシュスライサー
  - 外形、入れ子領域、連結バーコードセルに対応する winding ベースの複数ハッチ
- エンティティ、テキスト、バーコード
  - 点、線、円弧、ポリライン、三角形、矩形、スパイラル、トレパン、スプライン
  - 立方体、球、円柱、円錐、メッシュ、レイヤー、グループ、ブロック、ブロック挿入
  - Text、SiriusText、ImageText、Circular Text、リンクテキスト、ZPL エンティティ
  - Outline、Hatch、Dot セル加工に対応する 1D、QR、DataMatrix、PDF417、Aztec バーコード
- ファイルインポートと相互運用
  - Sirius3、DXF/DWG、HPGL/PLT、Gerber/Excellon、G-code/NGC
  - ラスター画像および STL、OBJ、PLY、STP/STEP 3D モデル
  - 許容距離に基づくベクターパス結合と内容に基づく Gerber/Excellon 判定
- リモート通信と動的データ
  - マーカー制御とデータアクセス用の TCP/IP、Serial（RS-232）、WebSocket、MQTT
  - AI によるエディター登録、文書・エンティティ編集、シミュレーション、登録済みデバイス制御に対応する組み込み HTTP MCP サーバー
  - テキストおよびバーコード用のイベント、ファイル、オフセット、リンク、C# スクリプト変換
- ドキュメント、エディター、シミュレーション
  - レイヤー、ペン、グループ、ブロック、設定可能な Undo/Redo を備えた 4 ページ
  - WinForms と WPF の `SiriusEditorControl` / `SiriusMultiEditorControl` を Debug と Release の両方でサポート。1 つのドキュメントを複数ビューに表示可能
  - 画面固定サイズのマーカー、ビーム効果、任意の粒子を使ったリアルタイム加工経路表示
  - カメラおよび検査向けのグリッド型スティッチ画像表示
- オープンアーキテクチャ
  - エディター、エンティティ、マーカー、スキャナー、レーザー、パワーメーター、リモートの拡張可能なインターフェイス
  - AI によるエディター登録、文書・エンティティ編集、デバイス制御に対応する組み込み MCP サーバー

## Sirius2 との主な違い

| 機能 | SIRIUS3 | SIRIUS2 |
|:--|:--|:--|
| ドキュメントページ | 4 ページ | 単一ドキュメント |
| カメラ | 内蔵 6 台 | 透視カメラ |
| レンダリング | GPU アクセラレーション OpenGL シェーダー | 内蔵シェーダー |
| ヒットテスト | AABB 高速化 | 低速 |
| ハッチ | winding 対応複数ハッチ | 単一ハッチ |
| 3D メッシュスライス | STL、OBJ、PLY、STEP | なし |
| Gerber / Excellon | 内容判定インポート | なし |
| ペン | Entity と Layer のペン | Entity ペンのみ |
| MCP | 対応 | なし |
| 更新 | NuGet Package Manager | 手動 |

![sirius3_hatch](https://spirallab.co.kr/sirius3/sirius3_hatch.png)
![sirius3_pod](https://spirallab.co.kr/sirius3/sirius3_pod.png)
![sirius3_slicer](https://spirallab.co.kr/sirius3/sirius3_slicer.png)
![sirius3_syncaxis](https://spirallab.co.kr/sirius3/sirius3_syncaxis.png)

## パッケージ / DLL

- `SpiralLab.Sirius3.Dependencies` - SCANLAB RTC4/5/6、syncAXIS ランタイム、フォント、サンプルデータ
- `SpiralLab.Sirius3` - スキャナー、レーザー、パワーメーターなどの HAL
- `SpiralLab.Sirius3.UI` - エンティティ、ジオメトリ処理、OpenGL レンダリング、WinForms/WPF コントロール、組み込み MCP 連携

NuGet Package Manager で簡単にインストールおよび更新できます。

## 対象プラットフォーム

- `net481`
- `net8.0-windows`
- `net9.0-windows`
- `net10.0-windows`

## システム要件

- Windows 10/11（x64）
- OpenGL 3.3 以上をサポートする GPU/ドライバー（最新ドライバーを強く推奨）
- SCANLAB ドライバー/ランタイム
- Visual Studio 2022 以降

## 依存関係

- SCANLAB
  - RTC4: v2023.11.02
  - RTC5: v2024.09.27
  - RTC6: 2026.6.19 v1.25.0
  - syncAXIS: v1.8.2（2023.03.09）
- .NET
  - `net481`: OpenTK 3.3.3
  - 新しいターゲット: OpenTK / OpenTK.Mathematics 4.9.4
  - すべてのターゲットで Microsoft.Extensions.Logging 10.0.10
  - すべてのターゲットで Microsoft.Extensions.Logging.Abstractions 10.0.10
  - すべてのターゲットで Microsoft.Extensions.Logging.Debug 10.0.10
  - Newtonsoft.Json 13.0.4
## パッケージのインストール

次の NuGet パッケージを参照してください。

- `SpiralLab.Sirius3.Dependencies`
- `SpiralLab.Sirius3`
- `SpiralLab.Sirius3.UI`

## クイックスタート

# [WinForms](#tab/winforms)

プロジェクト設定:

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWindowsForms>true</UseWindowsForms>
</PropertyGroup>

<PropertyGroup Condition="'$(TargetFramework)'=='net481'">
	<DefineConstants>$(DefineConstants);OPENTK3</DefineConstants>
</PropertyGroup>
<PropertyGroup Condition="'$(TargetFramework)'!='net481'">
	<DefineConstants>$(DefineConstants);OPENTK4</DefineConstants>
</PropertyGroup>

<ItemGroup Condition="'$(TargetFramework)'=='net481'">
	<PackageReference Include="OpenTK" Version="3.3.3" />
</ItemGroup>
<ItemGroup Condition="'$(TargetFramework)'!='net481'">
	<PackageReference Include="OpenTK" Version="4.9.4" />
	<PackageReference Include="OpenTK.Mathematics" Version="4.9.4" />
</ItemGroup>

<ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.10" />
    <PackageReference Include="SpiralLab.Sirius3.Dependencies" Version="1.*" />
    <PackageReference Include="SpiralLab.Sirius3" Version="1.*" />
    <PackageReference Include="SpiralLab.Sirius3.UI" Version="1.*" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
</ItemGroup>
```

サンプルコード:

```csharp
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using SpiralLab.Sirius3.MCP;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.PowerMap;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.Scanner.Rtc;

#if OPENTK3
    using OpenTK;
    using DVec3 = OpenTK.Vector3d;
#elif OPENTK4
    using OpenTK.Mathematics;
    using DVec3 = OpenTK.Mathematics.Vector3d;
#endif

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Sirius3 ライブラリを初期化
        SpiralLab.Sirius3.Core.Initialize();

        // WinForms を作成
        CreateAndExecuteMainForm();
    }

    static void CreateAndExecuteMainForm()
    {
        // 動的フォームを作成して SiriusEditorControl を追加
        Form dynamicForm = new Form();
        dynamicForm.SuspendLayout();
        dynamicForm.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        dynamicForm.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        dynamicForm.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        dynamicForm.Text = "DEMO - (c)SpiralLab";
        dynamicForm.Size = new Size(1600, 1200);
        dynamicForm.StartPosition = FormStartPosition.CenterScreen;
        var editorControl = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
        IMCPServer mcpServer = null;
        editorControl.Dock = DockStyle.Fill;
        dynamicForm.Controls.Add(editorControl);
        dynamicForm.ResumeLayout(false);

        dynamicForm.Load += (s, e) =>
        {
            // デバイスを生成・初期化して EditorControl に登録
            bool success = true;

            // スキャナー制御
            string correctionFile = "cor_1to1.ct5";
            string correctionPath = Path.Combine(SpiralLab.Sirius3.Config.CorrectionPath, correctionFile);
            const double fov = 100.0;
            var kfactor = Math.Pow(2, 20) / fov;
            var index = 0;
            var rtc = ScannerFactory.CreateRtc5(index, kfactor, LaserModes.Yag1, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionPath);
            success &= rtc.Initialize();
            rtc.CtlFrequency(50 * 1000, 2);
            rtc.CtlSpeed(100, 100);

            // デジタル I/O 制御
            var dIExt1 = IOFactory.CreateInputExtension1(rtc); success &= dIExt1.Initialize();
            var dOExt1 = IOFactory.CreateOutputExtension1(rtc); success &= dOExt1.Initialize();
            var dOExt2 = IOFactory.CreateOutputExtension2(rtc); success &= dOExt2.Initialize();
            var dILaserPort = IOFactory.CreateInputLaserPort(rtc); success &= dILaserPort.Initialize();
            var dOLaserPort = IOFactory.CreateOutputLaserPort(rtc); success &= dOLaserPort.Initialize();

            // パワーメーター制御
            double laserMaxPower = 20;
            var powerMeter = PowerMeterFactory.CreateVirtual(index, laserMaxPower);
            //var powerMeter = PowerMeterFactory.CreateCoherentPowerMax(index, 4);
            //var powerMeter = PowerMeterFactory.CreateGentecEO(index, 3, scaleIndex: null);
            success &= powerMeter.Initialize();

            // レーザー制御
            var laser = LaserFactory.CreateVirtualDutyCycle(index, laserMaxPower, 0, 100);
            //var laser = LaserFactory.Create ...
            success &= laser.Initialize();
            laser.Scanner = rtc;

            // パワーマップ
            var powerMap = PowerMapFactory.CreateDefault(index, "default");
            powerMap.Reset1to1("10000", laserMaxPower);
            laser.PowerMap = powerMap;

            // マーカー
            var marker = MarkerFactory.CreateRtc(index);
            //var marker = MarkerFactory.CreateRtcFast(index);
            //var marker = MarkerFactory.CreateSyncAxis(index);
            success &= marker.Initialize();

            Debug.Assert(success);

            // デバイスを登録
            editorControl.RegisterDevices(rtc, laser, powerMeter, dIExt1, dILaserPort, dOExt1, dOExt2, dOLaserPort, marker);

            // 認証付き HTTP MCP サーバーを起動
            mcpServer = MCPFactory.CreateServer(0, "MCP", editorControl);
            mcpServer.AccessMode = MCPAccessMode.All;
            if (!mcpServer.Start().GetAwaiter().GetResult())
                throw new InvalidOperationException("Failed to start the MCP server.");
            dynamicForm.Text = $"{dynamicForm.Text} + MCP ({mcpServer.Options.HttpEndpoint})";
        };

        dynamicForm.FormClosing += (s, e) =>
        {
            var dlgResult = MessageBox.Show(dynamicForm, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dlgResult != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            // エディター、文書、デバイスより先に MCP サーバーを解放
            mcpServer?.Dispose();
            mcpServer = null;

            // デバイスを解放
            editorControl.DisposeDevices();

            // 文書を解放
            editorControl.Document?.Dispose();

            // Sirius3 ライブラリをクリーンアップ
            SpiralLab.Sirius3.Core.Cleanup();
        };

        Application.Run(dynamicForm);
    }
}
```

# [WPF](#tab/wpf)

WPF プロジェクトでは次の設定を使用します。

```xml
<PropertyGroup>
  <OutputType>WinExe</OutputType>
  <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
  <UseWPF>true</UseWPF>
  <UseWindowsForms>true</UseWindowsForms>
</PropertyGroup>
```

`Program.cs`:

```csharp
using System;
using System.Windows;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.MCP;
using SpiralLab.Sirius3.PowerMap;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.UI.WPF;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var app = new Application();
        SiriusEditorControl editor = null;
        IMCPServer mcpServer = null;
        var coreInitialized = false;
        var editorDisposed = false;

        void DisposeEditor()
        {
            if (editorDisposed || editor == null)
                return;
            editorDisposed = true;

            var document = editor.Document;
            try { mcpServer?.Dispose(); }
            finally
            {
                mcpServer = null;
                try { editor.DisposeDevices(); }
                finally
                {
                    try { editor.Dispose(); }
                    finally { document?.Dispose(); }
                }
            }
        }

        try
        {
            SpiralLab.Sirius3.Core.Initialize();
            coreInitialized = true;
            WPFThemeManager.Initialize();

            editor = new SiriusEditorControl();

            const int index = 0;
            const double fieldSize = 100.0;
            const double laserMaxPower = 20.0;
            var scanner = ScannerFactory.CreateVirtual(
                index,
                Math.Pow(2, 20) / fieldSize,
                LaserModes.Yag1,
                RtcSignalLevels.ActiveHigh,
                RtcSignalLevels.ActiveHigh,
                null);
            var dIExt1 = IOFactory.CreateInputExtension1(scanner);
            var dILaserPort = IOFactory.CreateInputLaserPort(scanner);
            var dOExt1 = IOFactory.CreateOutputExtension1(scanner);
            var dOExt2 = IOFactory.CreateOutputExtension2(scanner);
            var dOLaserPort = IOFactory.CreateOutputLaserPort(scanner);
            var powerMeter = PowerMeterFactory.CreateVirtual(index, laserMaxPower);
            var laser = LaserFactory.CreateVirtualDutyCycle(index, laserMaxPower, 0, 100);
            var powerMap = PowerMapFactory.CreateDefault(index, "default");
            var marker = MarkerFactory.CreateVirtual(index);

            powerMap.Reset1to1("10000", laserMaxPower);
            laser.Scanner = scanner;
            laser.PowerMap = powerMap;

            var success = scanner.Initialize();
            success &= scanner.CtlFrequency(50 * 1000, 2);
            success &= scanner.CtlSpeed(100, 100);
            success &= dIExt1.Initialize();
            success &= dILaserPort.Initialize();
            success &= dOExt1.Initialize();
            success &= dOExt2.Initialize();
            success &= dOLaserPort.Initialize();
            success &= powerMeter.Initialize();
            success &= laser.Initialize();
            success &= marker.Initialize();

            editor.RegisterDevices(
                scanner, laser, powerMeter,
                dIExt1, dILaserPort,
                dOExt1, dOExt2, dOLaserPort,
                marker);

            if (!success)
                throw new InvalidOperationException("仮想デバイスの初期化に失敗しました。");

            var window = new Window
            {
                Title = "Sirius3 WPF Editor",
                Width = 1400,
                Height = 900,
                Content = editor,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };

            mcpServer = MCPFactory.CreateServer(0, "MCP", editor);
            mcpServer.AccessMode = MCPAccessMode.All;
            if (!mcpServer.Start().GetAwaiter().GetResult())
                throw new InvalidOperationException("Failed to start the MCP server.");
            window.Title = $"{window.Title} + MCP ({mcpServer.Options.HttpEndpoint})";

            // WPF ウィンドウと GL コンテキストが有効な Closing 時に MCP サーバーとエディターを破棄します。
            window.Closing += (_, __) => DisposeEditor();
            app.Run(window);
        }
        finally
        {
            try { DisposeEditor(); }
            finally
            {
                if (coreInitialized)
                    SpiralLab.Sirius3.Core.Cleanup();
            }
        }
    }
}
```

---

## デモプログラム

- 説明: [DEMOS.jaJP.md](DEMOS.jaJP.md)
- サンプル: https://github.com/labspiral/sirius3/tree/main/demos

## ライセンス

- 商用利用にはライセンスの購入が必要です。
- ライセンスは RTC インスタンス数と次のオプションで構成されます。
  - MoF: 外部エンコーダーを使用する移動体加工
  - MultiBeam: レーザー光源 1 台、AOM 2 台、スキャンヘッド 2 台
  - syncAXIS: スキャンヘッドと ACS ステージを同期する XL-SCAN
  - Remote: Socket、Serial、WebSocket、MQTT によるレシピ、加工、データ制御に加え、AI による文書・エンティティ編集と登録済みデバイス制御のための組み込み MCP サーバー機能を含みます
- [LICENSE.jaJP.txt](LICENSE.jaJP.txt) および [THIRD-PARTY-NOTICES.jaJP.txt](THIRD-PARTY-NOTICES.jaJP.txt) を参照してください。
- 連絡先: hcchoi@spirallab.co.kr | https://spirallab.co.kr

> ライセンスキーがない場合は、1 回の実行につき 30 分に制限された評価モードで動作します。

## バージョン履歴

- [HISTORY.jaJP.md](HISTORY.jaJP.md)

## API ドキュメント

- https://spirallab.co.kr/sirius3/doc
