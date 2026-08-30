# Sirius3

SCANLAB 制御、デバイス連携、ジオメトリ処理、OpenGL 可視化、ドキュメント編集、シミュレーション、マーキング実行を統合した Windows/.NET 向け精密レーザー加工プラットフォームです。

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
  - テキストおよびバーコード用のイベント、ファイル、オフセット、リンク、C# スクリプト変換
- ドキュメント、エディター、シミュレーション
  - レイヤー、ペン、グループ、ブロック、設定可能な Undo/Redo を備えた 4 ページ
  - 安定した WinForms コントロール。1 つのドキュメントを複数ビューに表示可能
  - 画面固定サイズのマーカー、ビーム効果、任意の粒子を使ったリアルタイム加工経路表示
  - カメラおよび検査向けのグリッド型スティッチ画像表示
- オープンアーキテクチャ
  - エディター、エンティティ、マーカー、スキャナー、レーザー、パワーメーター、リモートの拡張可能なインターフェイス

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
| 更新 | NuGet Package Manager | 手動 |

![sirius3_hatch](https://spirallab.co.kr/sirius3/sirius3_hatch.png)
![sirius3_pod](https://spirallab.co.kr/sirius3/sirius3_pod.png)
![sirius3_slicer](https://spirallab.co.kr/sirius3/sirius3_slicer.png)
![sirius3_syncaxis](https://spirallab.co.kr/sirius3/sirius3_syncaxis.png)

## パッケージ / DLL

- `SpiralLab.Sirius3.Dependencies` - SCANLAB RTC4/5/6、syncAXIS ランタイム、フォント、サンプルデータ
- `SpiralLab.Sirius3` - スキャナー、レーザー、パワーメーターなどの HAL
- `SpiralLab.Sirius3.UI` - エンティティ、ジオメトリ処理、OpenGL レンダリング、WinForms コントロール

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
  - ターゲット別 Microsoft.Extensions.Logging 8.0.1 / 9.0.15 / 10.0.7
  - Microsoft.Extensions.Logging.Abstractions 8.0.3 / 9.0.15 / 10.0.7
  - Newtonsoft.Json 13.0.4

## パッケージのインストール

次の NuGet パッケージを参照してください。

- `SpiralLab.Sirius3.Dependencies`
- `SpiralLab.Sirius3`
- `SpiralLab.Sirius3.UI`

## クイックスタート

プロジェクトで WinForms と対応ターゲットを有効にします。

```xml
<PropertyGroup>
  <OutputType>WinExe</OutputType>
  <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
  <UseWindowsForms>true</UseWindowsForms>
</PropertyGroup>
```

`SpiralLab.Sirius3.Core` を初期化し、スキャナー、レーザー、パワーメーター、I/O、マーカーを生成・初期化して `SiriusEditorControl.RegisterDevices(...)` に登録します。終了時は `DisposeDevices()` を呼び出し、その後 `Core.Cleanup()` を実行してください。完全なコンパイル可能サンプルは [英語 README の Quick Start](README.md#quick-start) を参照してください。

## デモプログラム

- 説明: [DEMOS.jaJP.md](DEMOS.jaJP.md)
- サンプル: https://github.com/labspiral/sirius3/tree/main/demos

## ライセンス

- 商用利用にはライセンスの購入が必要です。
- ライセンスは RTC インスタンス数と次のオプションで構成されます。
  - MoF: 外部エンコーダーを使用する移動体加工
  - MultiBeam: レーザー光源 1 台、AOM 2 台、スキャンヘッド 2 台
  - syncAXIS: スキャンヘッドと ACS ステージを同期する XL-SCAN
  - Remote: Socket、Serial、WebSocket、MQTT によるレシピ、加工、データ制御
- [LICENSE.jaJP.txt](LICENSE.jaJP.txt) および [THIRD-PARTY-NOTICES.jaJP.txt](THIRD-PARTY-NOTICES.jaJP.txt) を参照してください。
- 連絡先: hcchoi@spirallab.co.kr | https://spirallab.co.kr

> ライセンスキーがない場合は、1 回の実行につき 30 分に制限された評価モードで動作します。

## バージョン履歴

- [HISTORY.jaJP.md](HISTORY.jaJP.md)

## API ドキュメント

- https://spirallab.co.kr/sirius3/doc
