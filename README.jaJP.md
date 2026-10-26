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
  - Sirius3、DXF/DWG、SVG、HPGL/PLT、Gerber/Excellon、G-code/NGC
  - SVG（`.svg`）ベクターインポート：`IDocument.ActImport` / `EntityFactory.CreateSVG`。編集可能な元曲線を保持し、塗り・線幅・端点・接続部を mm 単位の領域として生成します。丸い端点と角も反映します。
  - SVG ハッチ：`UI.Config.IsSvgImportHatchEnabled` で有効にします（既定 `false`）。`UI.Config.SvgImportHatchInterval` で間隔を調整します（既定 0.1 mm）。線/ポリゴンハッチに対応し、クリックとドラッグによる選択は表示された輪郭とハッチを基準にします。
  - ラスター画像および STL、OBJ、PLY、STP/STEP、3MF 3D モデル
  - 3MF（`.3mf`）メッシュインポート：`IDocument.ActImport` / `EntityFactory.CreateMesh`。ビルド内の複数の三角形メッシュモデルとコンポーネントを、相対配置とファイルの単位を反映して一つの mm 単位メッシュにまとめます。保存されたスライサーの加工経路とスライス結果はインポート対象に含まれません。
  - 3D プレビューで利用可能なメッシュの色と OBJ/PLY/3MF の拡散テクスチャを保持
  - G-code/NGC インポートはモーダル XYZ 座標、単位・作業原点、平面円弧・ヘリックス、ドウェル、レーザー ON/OFF 経路を保持します。`UI.Config.GCodeImportDialect` で CNC・Laser・Marlin 規則を選択し、`GCodeImportInfo.TryGet` で機器設定を適用せず元の F/S/T と行範囲を取得します。
  - 許容距離に基づくベクターパス結合と内容に基づく Gerber/Excellon 判定
- リモート通信と動的データ
  - マーカー制御とデータアクセス用の TCP/IP、Serial（RS-232）、WebSocket、MQTT
  - AI によるエディター登録、文書・エンティティ編集、シミュレーション、登録済みデバイス制御に対応する組み込み HTTP MCP サーバー
  - テキストおよびバーコード用のイベント、ファイル、オフセット、リンク、C# スクリプト変換
- ドキュメント、エディター、シミュレーション
  - レイヤー、ペン、グループ、ブロック、設定可能な Undo/Redo を備えた 4 ページ
  - WinForms と WPF の `SiriusEditorControl` / `SiriusMultiEditorControl` を Debug と Release の両方でサポート。1 つのドキュメントを複数ビューに表示可能
  - [フォントエディター](SpiralLab.Sirius3.UI/FontEditor/README.md)：WinForms/WPF の **Font Edit** から CXF/LFF/EO FNT/DOT グリフを独立編集。mm ベクトル、整数 DOT セル、BBox/メトリクス、Undo/Redo、DXF インポート、テキストプレビュー、インストール済み TTF 変換に対応。Debug/Release の両方に常に含まれます。
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
| 3D メッシュスライス | STL、OBJ、PLY、STEP、3MF | なし |
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
  - RTC6: 2026.8.17 v1.25.1
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

上記の Sirius3 NuGet パッケージを3つインストールします。以下の抜粋は基本的な流れを示し、デバイス生成、検証、イベント接続を `...` で省略しています。起動、読み込み、終了の各処理をホストの対応するイベントに配置します。

# [WinForms](#tab/winforms)

プロジェクト設定

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWindowsForms>true</UseWindowsForms>
    <ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>
</PropertyGroup>
```

Coreを初期化してエディターを作成し、初期化済みのデバイスを登録します。WinFormsのDPI初期化はこれらの処理より先に行います。下のHiDPI項目を参照してください。

```csharp
using SpiralLab.Sirius3.MCP;
using System.Windows.Forms;

// Startup: initialize Core and create the host UI.
SpiralLab.Sirius3.Core.Initialize();
var form = new Form
{
    AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F),
    AutoScaleMode = AutoScaleMode.Dpi
};
var editor = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl
{
    Dock = DockStyle.Fill
};
form.Controls.Add(editor);
...

// Load: create and initialize the scanner, laser, I/O, power meter and marker.
...
editor.RegisterDevices(rtc, laser, powerMeter, ..., marker);

// Optional MCP: start after device registration (Remote license).
var server = MCPFactory.CreateServer(0, "MCP", editor);
server.AccessMode = MCPAccessMode.All;
editor.MCPServer = server;
await server.Start();
...
```

非同期終了ではUIイベントループを維持し、リソース解放の完了を待ちます。

```csharp
// FormClosing / Window.Closing: await cleanup on the owning UI thread.
var document = editor.Document;
await editor.DisposeDevices();
editor.Dispose();
document?.Dispose();
...

// Application exit: after the UI lifetime ends.
SpiralLab.Sirius3.Core.Cleanup();
```

# [WPF](#tab/wpf)

プロジェクト設定

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWPF>true</UseWPF>
    <ApplicationManifest>app.manifest</ApplicationManifest>
</PropertyGroup>
```

WPFエディターの作成前にCoreを初期化し、Windowの読み込み時に初期化済みデバイスを登録します。

```csharp
using SpiralLab.Sirius3.MCP;
using System.Windows;

// Startup: initialize Core and create the host UI.
SpiralLab.Sirius3.Core.Initialize();
var editor = new SpiralLab.Sirius3.UI.WPF.SiriusEditorControl();
var window = new Window { Content = editor };
...

// Loaded: create and initialize the scanner, laser, I/O, power meter and marker.
...
editor.RegisterDevices(rtc, laser, powerMeter, ..., marker);

// Optional MCP: start after device registration (Remote license).
var server = MCPFactory.CreateServer(0, "MCP", editor);
server.AccessMode = MCPAccessMode.All;
editor.MCPServer = server;
await server.Start();
...
```

非同期終了ではUIイベントループを維持し、リソース解放の完了を待ちます。

```csharp
// FormClosing / Window.Closing: await cleanup on the owning UI thread.
var document = editor.Document;
await editor.DisposeDevices();
editor.Dispose();
document?.Dispose();
...

// Application exit: after the UI lifetime ends.
SpiralLab.Sirius3.Core.Cleanup();
```

`DisposeDevices()` は登録済みMCPサーバーとRemoteの非同期解放を待ってからデバイスを解放します。エディターとDocumentの解放後、アプリケーション終了時に `Core.Cleanup()` を呼び出します。終了イベントの重複防止とエラー処理は詳細例を参照してください。

完全な例：[WinFormsガイド（英語）](https://github.com/labspiral/sirius3/blob/main/doc/WinForms.md)、[WPFガイド（英語）](https://github.com/labspiral/sirius3/blob/main/doc/WPF.md)、[デモプロジェクト](DEMOS.jaJP.md)。

---

## WinForms HiDPI

最初に DLL/パッケージを更新します。既存の `DpiUnaware` アプリは従来の動作を維持できます。モニターごとの拡大率を利用する場合は、コントロール作成前に使用側 EXE を設定します。DLL はプロセスの DPI モードを変更しません。既存のホスト Form は `AutoScaleMode.Font`、フォント、Designer の基準値を維持できます。独自の固定ピクセル配置と手動スケーリングは別途確認してください。

手動 `Main` を使用する .NET 8/9/10 では、`EnableVisualStyles` と `Core.Initialize` より前に Per-Monitor V2 を選択します:

```csharp
#if NET8_0_OR_GREATER
Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
#endif
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
```

.NET Framework 4.8.1 では、EXE の既存 app.config に以下を統合し、バインドリダイレクトを保持します。manifest に Windows 10 互換性を宣言し、コントロール作成前に `EnableVisualStyles` を呼び出します。競合する unaware DPI 宣言と `EnableWindowsFormsHighDpiAutoResizing=false` は削除します。

```xml
<System.Windows.Forms.ApplicationConfigurationSection>
  <add key="DpiAwareness" value="PerMonitorV2" />
</System.Windows.Forms.ApplicationConfigurationSection>
```

`ApplicationConfiguration.Initialize()` を使用する場合は EXE プロジェクトで `<ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>` を設定し、初期化経路を一つにします。フォントサイズは pt のままです。アプリ所有の ImageList は保持した元画像から DPI ごとに再生成し、画像追加前に `ImageSize`/`ColorDepth` を設定します。キーとインデックスを維持し、参照をすべて置換してから所有する旧リストだけを破棄します。WPF の 96 は論理単位の基準なので、配置に DPI 倍率を再度掛けないでください。

[Microsoft: .NET Framework HiDPI](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support-in-windows-forms), [WinForms automatic scaling](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/forms/autoscale).

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
