# デモプログラム一覧

## beginner

Sirius3 ライブラリとスキャナー、レーザーなどの各種デバイスを初期化し、UI に接続します。

## console_document

コンソール環境で各種デバイス、マーカー、ドキュメントを使用する方法を示します。

## console_syncaxis_setup

RTC6 ベースの XL-SCAN を syncAXIS で低レベル設定・診断します。`syncAXISConfig.xml` の読み込み、シミュレーション/ハードウェア切替、Follow/Unfollow、移動、校正、レーザー遅延およびシステム遅延テストを扱います。

## editor_automatic_laser_control

Defined Vector、指令/実速度、Spot Distance Control に基づく Automatic Laser Control（ALC）を実演し、出力データを計測してグラフ表示します。

## editor_barcode

各種 2D バーコード、セル構成、および同じデータを持つテキストを生成します。表示と加工条件は Entity Pen で設定します。

## editor_barcode_textconvert

イベント、C# スクリプト、外部ファイル、位置オフセットを使い、加工直前にバーコードデータを動的に変更します。

## editor_dio

RTC 拡張ポートのデジタル入出力を使用します。ピン名、Ready/Processing/Error 出力、外部 Start 信号を扱います。

## editor_document

ドキュメントの生成・交換と、複数の Editor/Viewer で 1 つのドキュメントを表示する方法を示します。

## editor_entity

Sirius3 の加工データおよびベクターデータを生成してページへ追加し、Block、Insert、Group の再利用方法を示します。

## editor_entity_custom

独自のひし形、フィデューシャル、ドリル穴エンティティを実装し、プロパティ編集、再生成、描画、複製、ハッチ、マーキングを示します。

## editor_fieldcorrection_2d

一定間隔のパターンを測定した誤差から新しい 2D 補正ファイルを生成し、テーブルへのロードと選択を実演します。

## editor_fieldcorrection_3d

`RtcCalibrationLibrary` を使用して、ビーム傾き、Z=0 フォーカス、A/B/C フォーカス係数、Z ボリュームの Stretch 補正を順番に実行します。

## editor_fieldcorrection_3d_pointscloud

3D 点群から Z 高さ補正ファイルを生成し、2D XY 加工データを 3D メッシュ表面へ投影します。

## editor_hardjump

通常の MicroVector Jump と 10 µs 1 サイクルの Hard Jump を比較します。Hard Jump は高速ですがスキャナー負荷が大きいため、仕様に合わせて使用します。

## editor_hatch

閉じた輪郭に Line/Polygon Hatch を生成し、経路を最適化して、ハッチごとに異なる Entity Pen を割り当てます。

## editor_hatch_clip

指定領域をクリップしてハッチパターンを生成します。

## editor_interrupt

RTC リストバッファ実行中に割り込みを発生させ、アプリケーション処理後に実行可能状態のリストを再開します。

## editor_laser_ui

独自の `ILaser` デバイスと専用 UI を `OnCreateLaserUI` で統合する方法を示します。

## editor_marker

`MarkerRtc`、`MarkerRtcFast`、`MarkerSyncAxis` の公開サンプルコードで、加工手順を用途に合わせて変更できます。

## editor_measurement_skywriting_wobbel

Skywriting Mode、Wobble 形状、および出力データを可視化する計測機能を組み合わせます。

## editor_mof_interrupt

MoF とリスト割り込みを組み合わせ、エンコーダー位置を待って追従を開始し、加工完了後に追従を終了します。

## editor_mof_offsets

仮想フィールド上の最大 1,000 オフセット位置で同じパターンを加工し、RTC6 + excelliSCAN の SCANAhead 設定も示します。

## editor_mof_trigger

外部トリガーで MoF を開始し、加工直前に動的バーコードを生成し、Free Variable で加工回数を積算します。

## editor_mof_xy

2 軸 MoF でオブジェクトごとに個別のエンコーダー待機条件を作成します。

## editor_mof_xy_raster

画像、1D/2D バーコード、ImageText の各ラスター行をエンコーダー位置に同期し、スキャナーフィールド中心付近で加工します。

## editor_multibeam

1 台のレーザーを 2 台の AOM とスキャンヘッドへ分配し、一方の Jump 中に他方が加工してレーザー利用率を高めます。

## editor_multibeam2

1 台のレーザーを共有する 2 つの RTC MultiBeam インスタンスを操作し、配線確認、Head 1/Head 2/Both、Ready/Start/Stop/Reset を制御します。

## editor_multiple

2 つのデバイスシステム、2 つの Editor、2 つの Document で異なるデータを並列加工します。

## editor_multiple2

2 つのデバイスシステムが 1 つの Editor と同じ Document を加工します。

## editor_offset

同じデータを複数位置で繰り返し加工し、dx/dy/dz、Z 回転、Scale を個別に設定します。

## editor_pen

Layer Pen と Entity Pen で加工条件を変更し、Marker Event で適用処理を上書きします。

## editor_pen_multiple

新規 Pen の既定値を上書きし、オブジェクトごとに異なる Entity Pen を割り当てます。

## editor_points_sync_pulses_count

点の LASER ON 時間をレーザー光源の SYNC OUT パルスに同期し、その他の同期信号も示します。

## editor_powermap

要求出力と実測出力の差を補償する PowerMap を生成・検証・使用します。`my_powermap.cs` に変更可能な処理手順があります。

## editor_remote

Serial、TCP/IP、WebSocket、MQTT で外部接続し、Marker、Offset、Object 値を読み書きします。サンプルは WebSocket を使用します。

## editor_scanahead

RTC6 SCANAhead と Auto Delays を使用し、Jump/Polygon/Mark/Laser の遅延を自動計算して Entity Pen の項目表示を連動させます。

## editor_scanahead_sdc

Layer Pen で SCANAhead と ALC を組み合わせ、Entity Pen で Spot Distance Control を設定します。

## editor_script

マーキング直前にスクリプトでエンティティデータを更新します。`.script`、C# ソース、DLL からの保存、復元、読み込み、コンパイルを示します。

## editor_slicer

3D メッシュを読み込み、指定 Z 平面で輪郭へスライスし、その領域にハッチを生成します。

## editor_steppermotor

RTC カードのステッピングモーター拡張ポートを制御します。

## editor_stitchedimage

カメラグリッド、解像度、視野から `EntityStitchedImage` を作成し、タイルごとの画像取得、表示、クリア、再構築を示します。

## editor_syncaxis

ACS Motion Control、excelliSCAN または intelliSCAN iV、RTC6 を組み合わせた XL-SCAN で、同期制御と Motion Decomposition を実演します。

## editor_ui

`SiriusEditorControl` と `SiriusMultiEditorControl` の公開 UI コードを使用して独自の WinForms UI を設計します。

## editor_viewer

1 つの Document を Editor と Viewer に同時接続する「One Document, Multiple Views」を示します。

## editor_zpl

ローカル BinaryKits レンダラーで `EntityImageZPL` を生成し、ラベルサイズ、印字密度、Unicode ZPL、`^CW` フォント割当、フォールバックフォントを示します。
