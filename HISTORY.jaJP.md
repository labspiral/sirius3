# Sirius3 バージョン履歴

## v1.13.0 (2026.8.31)

- 更新) SCANLAB RTC6 依存関係を Software Package 1.25.0 に更新し、初期化時にボードリビジョンに合うファームウェアを自動選択するとともに、`RtcRevision` では値 0 をリビジョン 1、値 1 をリビジョン 2 として確認できるようにしました
- 追加) `EntityNURBSSurface`、`EntityTorus`、`EntityPlane`、`EntityPyramid` の 3D サーフェスメッシュとファクトリメソッドを追加し、WinForms エディターで 3D メッシュとスプラインの作成・編集およびスライスのプレビューに対応しました
- 追加) 直線・円形の Sirius/GDI テキストと `EntityImageText` に Fixed 文字セル幅とグリフ幅合わせを追加し、空白や欠落グリフを含む文字位置を安定して維持できるようにしました
- 変更) 円形テキストは改行を同心円状の複数行として配置するようになり、テキストのベースラインと論理境界、および `EntityImageText` の透明余白計算を安定化しました
- 修正) 1D/2D バーコードの `Width` と `Height` を独立した最大加工境界として維持し、要求サイズを超えないようにエンコード行列を内部へ収めるようにしました
- 修正) `EntityPoint` と `EntityPoints` の複数パルス点マーキングで `EntityPen.IsPixelPulsesExit` 設定を RTC コマンドへ反映するようにしました
- 修正) RTC 2D 補正グリッドで dx/dy を空白またはカンマで区切って入力でき、誤差距離に応じたセル色を再び表示するようにしました

## v1.12.2 (2026.8.18)

- 追加) `EntityCircularSiriusText` エンティティを追加しました
- 修正) SiriusText の自動カーニングで、斜め方向を含むグリフ線分間の実際の最短距離から文字間隔を計算するようにしました

## v1.12.1 (2026.8.16)

- 追加) `EntitySiriusText.IsAutoKerning` により、Variable 文字間隔でキャッシュ済みフォント輪郭を基準に隣接グリフを自動カーニングできるようにしました
- 修正) Fixed 文字間隔で内容が変わっても文字セル位置と両端セルの余白を維持し、多言語フォントメトリックサンプルを設定でき、Unicode 17 の文字体系範囲に基づいてフォールバックを選択するよう改善しました
- 修正) `TextConverters.Link` のリンク先も `Link` を使用する場合、連続リンクをたどらず、エラーを記録してマーキングを停止するようにしました
- 変更) Gentec-EO 初期化時に `scaleIndex` が `null` の場合、オートスケールを有効にせず、デバイスの現在のスケールおよびオートスケール設定を維持するようにしました
- 修正) 空または不正なバーコード文字列ではエンコードエラーを記録し、以前の Data Matrix、QR、PDF417、Aztec、1D バーコード形状をエディターから消去するようにしました
- 追加) `ITransformable` が `OriginalIn/Out`、`ModelIn/Out`、`RealIn/Out` により、元形状、自身のモデル変換後、親変換を累積したワールド座標の経路始点・終点を提供するようにしました
- 変更) RTC 2D/3D 補正ダイアログ、共通検証エラー、カスタムメッセージボックスのボタンを Sirius3 リソースへ統合し、英語、韓国語、簡体字中国語、日本語、ドイツ語で表示するようにしました

## v1.11.14 (2026.8.7)

- 修正) OpenTK 3/4 がコンテキストに合った Polygon Mode を安全に読み書きし、Release のメモリアクセス障害と境界ボックス消失を防止
- 修正) ESC によるレーザー経路シミュレーション停止で PropertyGrid タイムアウトや仮想デバイスの重複 Abort が発生しないよう改善
- 修正) F5 加工開始確認をキャンセルした後に同じダイアログが再表示される問題
- 修正) PropertyGrid のローカライズ説明に関連設定、注意事項、設定順序を改行して再表示
- 修正) MultiBeam Both + JumpAndShoot で Token Release 待機を実 Jump と重ね、短い Jump のみを小さくまとめるよう改善
- 修正) Editor/TreeView ショートカットを安定化し、矢印キーは Tree ナビゲーションに維持
- 修正) マーキング中および Remote Desktop でも選択をロックしたまま Zoom/Pan を使用可能
- 変更) DXF、DWG、HPGL、PLT が共通の `Config.ImportMergeDistance` を使用
- 変更) Core/UI 設定ラベルの多言語化と DXF/DWG/Gerber の色保持または Pen 色への近似割当
- 追加) PropertyGrid を名前、カテゴリ、説明で検索。CTRL+F とワンクリック消去に対応

## v1.11.11 (2026.8.5)

- 修正) RTC6 の Status/Analog I/O API、Ethernet 切断判定、Status Timer 終了競合
- 修正) syncAXIS Job 完了後の Busy 解除と設定エラー報告
- 修正) StreamParser の接続、再接続、受信中クリーンアップ
- 修正) Barcode Encoding の任意化、要求 Dot サイズ保持、Data Matrix Shape 切替時のサイズ安定化

## v1.11.10 (2026.8.1)

- 修正) バーコードの要求サイズ、加工経路、補助コード、Hatch、Dot Cell の整合性
- 修正) 3D Mesh Slice の速度、信頼性、破損メッシュ警告
- 修正) エンティティ形状を変更しない安定した AABB HitTest
- 修正) Hatch、ALC、Pen のリスト編集と Preview 更新
- 追加) 画面固定 Marker、Beam、減衰 Particle を使うレーザー経路 Simulation
- 修正) Vector/Gerber/Excellon の近接 Path 結合、内容判定、安全な非対応ファイル処理

## v1.11.0 (2026.7.27)

- 追加) `EntityStitchedImage` と `IView.StitchedImage`
- 追加) `IEntityCloneable`
- 修正) 一部エンティティの選択強調
- リファクタリング) OpenGL 描画を Renderer Layer へ分離し選択表示を改善
- 修正) PropertyGrid の範囲外入力を Min/Max に調整

## v1.10.14 (2026.7.10)

- 追加) `TextConverters.Link` によるリンク先プロパティのテキスト変換
- 修正) `Config.UnReDoSize` を超えた Redo を含む Undo/Redo 安定性
- 修正) Intel GPU での OpenGL 初期化
- 修正) `EntityBarcode1D_V2.QuiteZone` を左右余白として処理
- 追加) 並列タスク数を制限する `UI.Config.MaxDegreeOfParallelism`

## v1.10.11 (2026.7.1)

- リファクタリング) Text Entity の Kerning、Variable/Fixed 幅、外部 `.fnt` フォント
- 追加) 時間・周波数などを RTC 分解能へ変換する `Config.IsConvertToControllerResolution`
- 修正) BinaryKits ZPL の韓国語フォントと設定可能なフォールバック
- 修正) Remote text コマンドの複数データ処理
- 修正) サイズ 0 の ZoomFit、File Converter の行削除、Undo 後の編集例外

## v1.10.10 (2026.6.22)

- 追加) `EntityText` と `EntitySiriusText` の固定全体幅
- 追加) BinaryKits によるローカル ZPL 画像変換
- 追加) `OriginalDimension`、`ModelDimension`、`RealDimension`
- 追加) Hatch Alignment の None、Center、Fit
- 追加) GS1 区切り文字変換
- 修正) 複数 View の画像 Texture
- 修正) `EntityUniformGroup` に追加可能なオブジェクト制約
- 修正) 最大 99 x 99 点の 2D Scanner Calibration
- 修正) `RtcCalibrationLibrary` の測定座標逆変換

## v1.9.0 (2026.6.1)

- 追加) `.gcode` / `.ngc` の G-code Import
- 改善) `TextConverters.Offset` 拡張値と Remote text コマンド
- 更新) Ophir StarLab v4.00
- 修正) 外部 Entity 作成。`editor_entity_custom` を参照

## v1.8.6 (2026.5.14)

- 修正) `HatchFills` と Barcode Outline Cell の Hatch
- 追加) `IView.CheckerSize` による背景 Checker Grid サイズ
- 修正) Editor ショートカットの Undo と安定性

## v1.8.5 (2026.5.8)

- 追加) 対応する `IDocument.Act*` の Undo/Redo と履歴数設定
- 追加) Aztec、PLESSEY Barcode と Pixel Size 編集
- 追加) Dot、Circle、Cross、Grid を作る CreateGrid
- 追加) Femtosecond Laser 向け RTC6 Pulse Picking
- 修正) `IRtcStepper` の非同期初期化/待機
- 修正) DIO Analog 表示、MultiBeam Token 交換、SEMI OCR Raster 加工

## v1.8.1 (2026.4.22)

- 追加) Serial、TCP/IP、WebSocket、MQTT を使う `IRemote`
- 追加) 加工直前に Text を変更する C# Script
- 追加) SEMI OCR `.dot` Font
- 修正) `IDocument.FindByName` と MultiBeamControl Toggle

## v1.7.1 (2026.4.16)

- 更新) RTC6 Package v1.24.0
- 修正) Task ベースの非同期 `IMarker`
- 修正) Single/Multi Editor の MultiBeam 排他同期
- 追加) LogControl の Filter/Search
- 修正) View ごとの Shader 管理
- 修正) 3D A/B/C 係数の 16/20 bit 処理
- 修正) Memory Leak、Snapshot、C# Script 速度

## v1.6.1 (2026.4.9)

- 追加) `ViewerControl` と One Document, Multiple Views
- 修正) `RtcCalibrationLibrary` による Beam Tilt、XY、Z=0 Focus、A/B/C、Stretch の 3D 校正
- 変更) `RtcCorrection3D`、`KZScale`、`ZOffset` を CalibrationLibrary / MatrixStack に置換
- 追加) `EntityPoint`、`EntityBarcode1D_V2`、Vertex List File I/O

## v1.5.4 (2026.4.2)

- 修正) `SiriusEditorControl` と `SiriusMultiEditorControl` の Design Time 例外

## v1.5.3 (2026.3.31)

- 追加) `IView` 接続設定と `IPowerMeter.MeasureUnits`
- 修正) RTC6 Auto Delays の通知と関連 Pen Property 表示
- 修正) Barcode Dot 順序、MultiBeam Token、2D/3D 補正の内部 Matrix 対応

## v1.5.2 (2026.3.27)

- 追加) RTC5/RTC6 の `IRtcStepper` と StepperControl
- 追加) RTC Serial Port の `IRtcSerialComm` と SerialCommControl
- 改善) 3 軸および McBSP 対応 MoF Extension
- リファクタリング) Offset、Marker、Scanner、Laser Control
- 変更) MatrixStack から BaseMatrix を削除し `IRtc.CtlMatrix` / `ListMatrix` に統合
- 修正) PowerMeter、外部 `.sirius3` Import、Layer Pen 色表示
- 追加) Editor の Paste Array

## v1.4.1 (2026.3.10)

- 追加) Web およびローカル API ドキュメント
- 追加) PropertyGrid の右ドラッグ値編集
- 修正) RTC6 Pulse Wait、EntityPen 表示、4 Page の既定有効化

## v1.4.0 (2026.3.3)

- 追加) .NET 9.0-Windows / .NET 10.0-Windows
- 追加) 外部 Laser Sync Signal による Pulse Count
- 追加) 実験的 `IRtcMultiBeam`、`EntityPoints`、`IRtcIO`
- 修正) Ext16 Entity の ushort Mask と Design Time 安定性
- 変更) License/Option 不足時の 30 分 Evaluation Mode

## v1.3.2 (2026.2.20)

- 修正) ALC Extended Mode の組み合わせ
- 追加) SCANAhead Spot Distance
- 追加) `IRtcMoF` Encoder Error、Virtual Field、Filter
- 名称変更) `IRtcWaitID` を `IRtcInterrupt` に変更

## v1.3.1 (2026.2.9)

- 追加) `IRtcSCANAhead`、SCANahead Pen Property、Position/Trajectory Acknowledge Limit
- 追加) `IRtcWaitID`

## v1.3.0 (2026.2.5)

- 追加) Vertex Editor、`SiriusMultiEditorControl`、`EntityLayerPen`
- 置換) gnuplot を内蔵 Plot に変更
- 追加) DWG/DXF 用 ODA Converter
- 変更) 3D を基本機能、syncAXIS を License Option に変更

## v1.2.7 (2026.1.26)

- 追加) Variable Polygon Delay / Jump Delay
- 修正) Skywriting LaserOnShift、Arc 分解、Contour Close、Pen Color、Simulation Entity Remove

## v1.2.6 (2026.1.20)

- 追加) Ellipse、RampFactor、Hatch Repeat
- 修正) Pen Value 表示、PowerMap Compensation、複数 Bounding Box Preview

## v1.2.5 (2026.1.15)

- 追加) Contour Offset、Sub-Entity Hit Mode、追加 HitTest
- 修正) 詳細 HitTest と Empty Ungroup
- 更新) ZXing 0.16.11、Clipper2 2.0.0

## v1.2.4 (2026.1.7)

- 追加) Render/Mark Shortcut、FreeVariable Event、GridCloud Interval
- 修正) Gerber Parse、詳細 HitTest、Ungroup 例外

## v1.0.1 (2025.12.22)

- 追加) CHM ドキュメント、Contour Expand、Gentec-EO PowerMeter
- 更新) Ophir StarLab v3.93
- 修正) Hatch Joint、Pen Search、Marker Log

## v0.9.3 (2025.12.5)

- 追加) ZoomFit と `TextConverters.Offset`
- 修正) Gerber Import と Scanner Pen 適用
- 名称変更) Scanner Pen を Entity Pen に変更

## v0.9.2 (2025.11.25)

- 追加) Block / BlockInsert 変換
- 名称変更) `EntityGroup` を `EntityMixedGroup` に変更
- 修正) Ungroup、Group Performance、Gerber Load、Large Document Save

## v0.9.1 (2025.11.18)

- 追加) Dependencies Package の gnuplot と Uniform Group
- 修正) Uniform Group Render、Memory Leak、Spline、Large Tree
- 変更) `Core.Initialize` Signature

## v0.8.2 (2025.11.11)

- 修正) HPGL Parse、Scanner Pen、`ActNew` 後の Pen 更新

## v0.8.0 (2025.11.7)

- 開発者プレビュー

## v0.1 (2025.03.06)

- 初回リリース
