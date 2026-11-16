# Lịch sử phiên bản Sirius3

## v1.17.2 (2026.12.x)

- added) Thêm tài nguyên tiếng Việt và tiếng Tây Ban Nha
- updated) Bổ sung mô tả chi tiết cho các bước hiệu chuẩn 3D

## v1.17.1 (2026.11.11)

- Thêm) `Rtc6SyncAxis`: màn hình thiết lập
- Thêm) `Rtc6Ethernet`: giám sát watchdog
- Sửa) `IDocument.ActClip`: tùy chọn `ClipOptions.IsIncludeOnce`
  - Dữ liệu dùng chung tại biên chỉ được tạo trong một vùng cắt

## v1.17.0 (2026.10.26)

- Cập nhật) Cải thiện tài liệu nhà phát triển
- Thêm) `IGrippable`
  - Tạo đối tượng tương tác và sửa đỉnh, vị trí, kích thước, góc xoay
  - Di chuyển bám lưới bằng tổ hợp Ctrl, Alt và Shift
- Thêm) `FontEditorControl`
  - Tạo và sửa tệp phông chữ cho `EntitySiriusText`
- Thêm) `EntityImagePDF`
  - Nhập PDF qua `IDocument.ActImport`
- Sửa) `EntityMixedGroup`, `EntityUniformGroup`, `EntityGerber`
  - Kết xuất nhanh hơn khi sửa đối tượng con trong đối tượng tổng hợp
- Sửa) `MCPServerOptions.AllowedFileRoots`
  - Truy cập tệp MCP mặc định cho phép mọi đường dẫn mà tiến trình chủ có thể truy cập

## v1.16.1 (2026.10.12)

- Cập nhật) Cập nhật thành phần SCANLAB RTC6 lên Software Package 1.25.1
- Thêm) Nhập tệp .3MF: 'IDocument.ActImport'
- Thêm) Nhập tệp .SVG: 'IDocument.ActImport'
- Thêm) `IPowerMap` cho phép chọn nội suy tuyến tính hoặc đường cong phi tuyến mới (PCHIP): 'Config.PowerMapInterpolationMode'
- Thêm) `IClippable` chia dữ liệu vùng lớn thành các vùng chữ nhật bằng cắt vùng: 'IDocument.ActClip'
- Sửa) Sửa thứ tự truyền mặt nạ trong `ListWriteMaskExtDO16If`

## v1.15.0 (2026.10.1)

- Thêm) Hỗ trợ HiDPI trong WinForms
- Sửa) Hỗ trợ giải phóng tài nguyên bất đồng bộ cho IRemote và IMCPServer
- Thêm) Kết xuất kết cấu cho lưới 3D

## v1.14.4 (2026.9.24)

- Thêm) Hỗ trợ MCP (Model Context Protocol)
  - Máy chủ MCP để ứng dụng AI đăng ký trình chỉnh sửa, sửa tài liệu, đối tượng và điều khiển thiết bị
- Thêm) Hỗ trợ WPF
  - Trình chỉnh sửa WPF (`SiriusEditorControl`, `SiriusMultiEditorControl`)
  - Bản xem trước
- Cập nhật) Thống nhất `Microsoft.Extensions.Logging`, `Microsoft.Extensions.Logging.Abstractions` và `Microsoft.Extensions.Logging.Debug` ở 10.0.10 trên mọi framework đích
- Sửa) Mở rộng hỗ trợ đầu ra đa ngôn ngữ
- Sửa) Cải thiện nhật ký ngoại lệ để có ngăn xếp lời gọi
- Sửa) Sửa lỗi khởi tạo `Rtc6SyncAxis`

## v1.13.0 (2026.8.31)

- Cập nhật) Cập nhật SCANLAB RTC6 lên Software Package 1.25.0; tự chọn firmware theo phiên bản thẻ khi khởi tạo, công bố `RtcRevision` bằng 0 cho Revision 1 và 1 cho Revision 2
- Thêm) Thêm lưới bề mặt 3D `EntityNURBSSurface`, `EntityTorus`, `EntityPlane`, `EntityPyramid` cùng hàm factory; hỗ trợ tạo/sửa lưới, spline 3D và xem trước lát cắt trong WinForms
- Thêm) Thêm độ rộng ký tự Fixed và khớp độ rộng glyph cho văn bản Sirius/GDI thẳng, theo vòng tròn và `EntityImageText`, giữ vị trí cho khoảng trắng và glyph thiếu
- Đổi) Bố trí xuống dòng văn bản tròn thành các vòng đồng tâm; ổn định đường cơ sở, giới hạn logic và cải thiện lề trong suốt của `EntityImageText`
- Sửa) Mã vạch 1D/2D giữ `Width`, `Height` làm giới hạn gia công tối đa độc lập và đặt ma trận mã hóa bên trong, không vượt kích thước yêu cầu
- Sửa) Đánh dấu điểm nhiều xung bằng `EntityPoint`, `EntityPoints` áp dụng `EntityPen.IsPixelPulsesExit` vào lệnh RTC
- Sửa) Lưới hiệu chỉnh RTC 2D nhận dx/dy phân cách bằng khoảng trắng hoặc dấu phẩy và tô màu ô theo khoảng cách sai số

## v1.12.2 (2026.8.18)

- Thêm) Đối tượng `EntityCircularSiriusText`
- Sửa) AutoKerning của SiriusText tính khoảng cách chữ từ khoảng cách ngắn nhất thực giữa đoạn glyph, gồm khoảng cách chéo

## v1.12.1 (2026.8.16)

- Thêm) `EntitySiriusText.IsAutoKerning` hỗ trợ căn khoảng cách glyph liền kề từ đường bao phông đã lưu đệm trong chế độ Variable
- Sửa) Chế độ khoảng cách Fixed giữ vị trí ký tự, lề hai đầu khi nội dung đổi, hỗ trợ mẫu số đo đa ngôn ngữ có thể cấu hình và chọn dự phòng theo dải chữ Unicode 17
- Sửa) Nếu đích của `TextConverters.Link` cũng dùng `Link`, đánh dấu ghi lỗi và dừng, không đi theo chuỗi liên kết liên tiếp
- Đổi) Khởi tạo Gentec-EO giữ thang đo và thiết lập tự động hiện tại khi `scaleIndex` là `null`, thay vì bật tự động thang đo
- Sửa) Văn bản mã vạch trống hoặc không hợp lệ ghi lỗi mã hóa và xóa hình Data Matrix, QR, PDF417, Aztec hoặc 1D cũ khỏi trình chỉnh sửa
- Thêm) `ITransformable` cung cấp điểm đầu/cuối đường dẫn trong tọa độ nguồn, mô hình riêng và thế giới tích lũy theo cha qua `OriginalIn/Out`, `ModelIn/Out`, `RealIn/Out`
- Đổi) Hộp thoại hiệu chỉnh RTC 2D/3D, lỗi kiểm tra dùng chung và nút thông báo tùy chỉnh dùng tài nguyên Sirius3 bằng tiếng Anh, Hàn, Trung giản thể, Nhật và Đức

## v1.11.14 (2026.8.7)

- Sửa) OpenTK 3 và 4 đọc, khôi phục chế độ đa giác an toàn theo loại ngữ cảnh, ngăn lỗi truy cập bộ nhớ trong Release, mất khung bao và khẳng định debug sai của OpenTK 3
- Sửa) Mô phỏng đường laser dừng kịp thời bằng ESC, không bị chờ PropertyGrid, hủy RTC ảo lặp lại khi dọn dẹp hoặc báo lỗi cho lần hủy laser ảo bình thường
- Sửa) Hủy xác nhận bắt đầu F5 từ vùng biên tập hoặc cây đang có tiêu điểm không còn mở lại cùng hộp thoại
- Sửa) Mô tả PropertyGrid giữ thiết lập liên quan, lưu ý và thứ tự cấu hình trên các dòng riêng thay vì mất khi bản địa hóa
- Sửa) Trong MultiBeam Both với JumpAndShoot, chờ nhả token chồng lấp với bước nhảy thực; chỉ bước nhảy ngắn ít tốn chi phí được gom thành lô nhỏ
- Sửa) Phím tắt trình chỉnh sửa và cây, gồm CTRL+R, CTRL+M, camera, mô phỏng, bộ đánh dấu hoạt động ổn định, đồng thời giữ mọi tổ hợp phím mũi tên cho điều hướng cây
- Sửa) Giữ thu phóng, dịch chuyển trình chỉnh sửa khi đánh dấu và dùng desktop từ xa, trong khi khóa chọn đối tượng
- Đổi) Nhập đường DXF, DWG, HPGL, PLT dùng chung khoảng cách gộp đầu mút có thể cấu hình `Config.ImportMergeDistance`
- Đổi) Nhãn cấu hình Core/UI theo ngôn ngữ đã chọn; nhập DXF, DWG, Gerber có thể giữ màu nguồn hoặc ánh xạ đến màu bút đối tượng gần nhất
- Thêm) Tìm thuộc tính theo tên, nhóm hoặc mô tả, đặt tiêu điểm bằng CTRL+F và xóa bằng một lần nhấp

## v1.11.11 (2026.8.5)

- Sửa) RTC6 đọc trạng thái và I/O tương tự qua API bộ điều khiển đúng, phát hiện đúng lỗi kết nối Ethernet và tắt mà không tranh chấp với bộ định thời trạng thái
- Sửa) Tác vụ syncAXIS giải phóng trạng thái bận khi hoàn tất và báo lỗi cấu hình nhất quán
- Sửa) Ổn định kết nối, kết nối lại và đóng StreamParser, dọn an toàn tác vụ nhận đang chờ
- Sửa) Cho phép bỏ mã hóa mã vạch, giữ kích thước điểm yêu cầu và cung cấp riêng kích thước ma trận thực; đổi dạng Data Matrix không làm giảm kích thước đã đặt

## v1.11.10 (2026.8.1)

- Sửa) Mã vạch nằm trong kích thước yêu cầu; đường gia công, mã bổ sung, tô và mô phỏng ô chấm được căn khớp chính xác
- Sửa) Cắt lớp lưới 3D nhanh, ổn định hơn và cảnh báo rõ cho lưới hỏng hoặc không đầy đủ
- Sửa) Kiểm tra lựa chọn AABB ổn định, nhanh hơn mà không đổi hình học đối tượng
- Sửa) Trình chỉnh sửa hatch, ALC, bút và danh sách cập nhật giá trị, xem trước ổn định hơn
- Thêm) Mô phỏng đường laser dùng dấu chỉ kích thước cố định, tia nhìn thấy và hạt mờ dần nhẹ
- Sửa) Nhập vector, Gerber, Excellon tự nối đường gần nhau, nhận dạng nội dung và bỏ qua tệp không hỗ trợ an toàn

## v1.11.0 (2026.7.27)

- Thêm) Thêm đối tượng EntityStitchedImage
    - Có thể dùng dưới dạng IView.StitchedImage
- Thêm) Hỗ trợ giao diện IEntityCloneable
- Sửa) Sửa lỗi một số đối tượng không được tô đậm khi chọn
- refactor) Tách kết xuất OpenGL của đối tượng và cải thiện dấu chỉ lựa chọn
    - Chuyển lời gọi OpenGL sang lớp kết xuất
- Sửa) Giá trị nhập trong PropertyGrid
    - Giá trị vượt phạm vi được giới hạn giữa nhỏ nhất, lớn nhất thay vì hiển thị cảnh báo

## v1.10.14 (2026.7.10)

- Thêm) TextConverters.Link
    - TextConverter lấy giá trị thuộc tính của đối tượng liên kết theo tên LinkEntity và chuyển thành văn bản.
- Sửa) Cải thiện ổn định Hoàn tác và Làm lại.
    - Sửa ngoại lệ khi Làm lại vượt giới hạn Config.UnReDoSize.
- Sửa) Cải thiện ổn định khởi tạo OpenGL
    - Lỗi «Không phản hồi» khi dùng GPU Intel
- Sửa) EntityBarcode1D_V2
    - Giá trị «QuiteZone» được xử lý thành lề trái và phải
- Thêm) UI.Config.MaxDegreeOfParallelism
    - Hỗ trợ giới hạn số tác vụ song song tối đa (mặc định bằng 50% số bộ xử lý logic)

## v1.10.11 (2026.7.1)

- Tái cấu trúc) Văn bản
    - EntityText, EntityImageText, EntityCircularText: áp dụng kerning và căn giữa khi dùng «Fixed»
    - EntitySiriusText: căn giữa khi dùng «Fixed» và hỗ trợ phông ngoài dạng nhị phân .fnt
    - EntityImageText: đặt tổng chiều rộng bằng TargetWidthPixels, hỗ trợ chiều rộng biến đổi và cố định
    - EntityCircularText: hỗ trợ chiều rộng biến đổi và cố định
- Thêm) Config.IsConvertToControllerResolution
    - Xác định giá trị EntityPen và EntityLayerPen (thời gian, tần số, v.v.) có được hiển thị sau khi đổi theo độ phân giải thực của RTC hay không
    - False: mặc định, hiển thị nguyên giá trị người dùng nhập
    - True: đổi giá trị theo độ phân giải điều khiển RTC
- Sửa) EntityImageZPL
    - Hỗ trợ phông tiếng Hàn khi chuyển đổi bằng BinaryKits
    - Đổi phông chuyển đổi qua Config.ZPLBinaryKitsFonts
- Sửa) Remote
    - Sửa xử lý nhiều mục dữ liệu của lệnh text
    - Ví dụ 1) text|1|Text_1|ABCD123;
    - Ví dụ 2) text|2|Text_1|ABCD1|Text_2|ABCD2|Text_1|ABCD3|Text_2|ABCD4;
- Sửa) Lỗi
    - Sửa ZoomFit không hoạt động với đối tượng kích thước 0
    - Sửa dòng dữ liệu đầu không bị xóa khi dùng TextConverters.File
    - Sửa ngoại lệ khi chỉnh thuộc tính sau Hoàn tác

## v1.10.10 (2026.6.22)

- Thêm: Hỗ trợ cố định toàn bộ chiều rộng đối tượng văn bản
    - Đối tượng: EntityText, EntitySiriusText
    - Thêm thuộc tính «Target width»
    - Khi đặt 0, tạo văn bản ở cỡ phông tối ưu như trước;
    - với giá trị lớn hơn 0, tự điều chỉnh tỷ lệ theo `Target width`.
- Thêm) Chuyển đổi ảnh ZPL cục bộ.
    - Đối tượng `EntityImageZPL`.
    - Trước đây: chuyển đổi trực tuyến qua dịch vụ Labelry.
    - Thay đổi: chuyển đổi ngoại tuyến bằng thư viện BinaryKits.
    - Mặc định: chuyển sang BinaryKits;
    - Có thể đổi dịch vụ tạo ảnh qua UI.Config.ZPLService;
- Thêm) Hỗ trợ chuyển đổi toàn bộ kích thước;
    - OriginalDimension: xuất kích thước đối tượng nguồn;
    - ModelDimension: hỗ trợ đổi kích thước (rộng, cao, sâu) trong không gian cục bộ;
    - RealDimension: kích thước (rộng, cao, sâu) trong không gian thế giới (Real) sau khi tích lũy ModelMatrix của mọi đối tượng cha
- Thêm) Hỗ trợ căn chỉnh hatch
    - Thêm Alignment cho HatchLine
    - None: không căn chỉnh
    - Center (mặc định): căn giữa
    - Fit: tính lại và điều chỉnh khoảng cách đồng đều
- Thêm) Hỗ trợ định dạng GS1
    - Chuyển đổi dấu phân cách &lt;GS&gt; và (,) trong GS1
- Sửa) Kết xuất kết cấu ảnh trên nhiều khung nhìn
    - Đối tượng: EntityImage, EntityImageText, EntityImageZPL
    - Sửa lỗi không hiển thị kết cấu khi dùng nhiều khung nhìn
- Sửa) EntityUniformGroup
    - Thêm ràng buộc đối tượng có thể đổi thành nhóm đồng nhất
    - Cấm thêm đối tượng điều khiển hoặc đối tượng chứa ITextConvertible hay IHatch
- Sửa) Hiệu chuẩn máy quét 2D
    - Hỗ trợ tối đa 99x99 điểm hiệu chuẩn khi dùng RtcCorrection2D
- Sửa) Hiệu chuẩn RtcCalibrationLibrary
    - Tự biến đổi ngược tọa độ đo khi dùng MatrixPrimaryInternal
    - Tự tính dữ liệu nguồn khi sử dụng máy quét đã xoay

## v1.9.0 (2026.6.1)

- Thêm) Hỗ trợ nhập G-code
    - Phần mở rộng: tệp .gcode hoặc .ngc
- Sửa) Cải thiện TextConverters.Offset của TextConverter
    - Trước đây: dùng ExtensionData của Offset làm văn bản chuyển đổi
    - Thay đổi: nếu ExtensionData của Offset có dạng «Entity1|Value1|Entity2|Value2;...», TextConverter phân tích và dùng khóa, giá trị tương ứng
    - Thêm) Remote
        - Thêm lệnh text
        - Định dạng: text|count|Name1|Text1|Name2|Text2|...;
- Cập nhật) Ophir StarLab v4.00
- Sửa) Hỗ trợ tạo đối tượng ngoài
    - Xem dự án demo editor_entity_custom

## v1.8.6 (2026.5.14)

- Sửa) Tô
    - Thêm tùy chọn HatchFills cho hatch đường
    - Cải thiện: hatch áp dụng đúng khi dùng kiểu ô Outline cho mã vạch
- Thêm) Kích thước lưới nền caro
    - Hỗ trợ đặt kích thước qua IView.CheckerSize
- Sửa) Hoàn tác, Làm lại
    - Hỗ trợ Hoàn tác khi dùng phím tắt trong EditorControl
    - Cải thiện ổn định

## v1.8.5 (2026.5.8)

- Thêm) Hỗ trợ Undo và Redo
    - Hỗ trợ ActUndo và ActRedo trong IDocument
    - Chỉ có hiệu lực với một số hàm mang tên IDocument.Act
    - Có thể tắt qua Config.IsUnReDoEnable
    - Số mục lịch sử có thể đổi bằng Config.UnReDoSize (mặc định: 30)
- Thêm) Mã vạch
    - Thêm mã vạch Aztec 2D
    - Thêm mã vạch PLESSEY 1D
    - Hỗ trợ sửa kích thước pixel (Dimension)
- Thêm) Form CreateGrid
    - Hỗ trợ tạo điểm, hình tròn, dấu cộng và mẫu lưới
- Thêm) Rtc6
    - Thêm hỗ trợ chọn xung cho laser femtosecond
- Sửa) IRtcStepper
    - Thêm xử lý bất đồng bộ cho khởi tạo và hàm chờ
- Sửa) Form DIO
    - Sửa hiển thị giá trị đầu ra tương tự
- Sửa) MultiBeam
    - Đổi khoảng nhảy/trao đổi token khi gia công raster
    - Trước đây: trao đổi token khi nhảy ListRasterLine ở cuối mỗi dòng
    - Thay đổi: trao đổi token khi nhảy ListRasterPixel giữa từng pixel
- Sửa) Phông semi ocr
    - Áp dụng phương thức gia công raster

## v1.8.1 (2026.4.22)

- Thêm) Remote
    - Thêm giao diện IRemote để đổi công thức, truy vấn/sửa thuộc tính, gửi lệnh bắt đầu, dừng, đặt lại và thiết lập độ lệch gia công qua giao tiếp ngoài
    - Hỗ trợ giao tiếp nối tiếp
    - Hỗ trợ giao tiếp TCP/IP
    - Hỗ trợ giao tiếp WebSocket
    - Hỗ trợ giao tiếp MQTT
- Thêm) Script
    - Hỗ trợ sửa dữ liệu văn bản thời gian thực trong gia công bằng tệp script C# ngoài
    - Kết hợp với TextConverter.SimpleScript
    - Có thể dùng script C# tự viết trong thư mục Script
    - ScriptInstance tại IMarker
- Thêm) Phông SEMI OCR
    - Thêm tệp phông .dot
    - Hỗ trợ phông điểm bằng SiriusText
- Sửa) IDocument
    - Sửa lỗi tìm kiếm FindByName
- Sửa) MultiBeamControl
    - Sửa trạng thái bật/tắt của nút

## v1.7.1 (2026.4.16)

- Cập nhật) Gói RTC6 v1.24.0
    - Ngày phát hành: 31 tháng 3 năm 2026
- Sửa) IMarker
    - Hỗ trợ xử lý bất đồng bộ (thay đổi mã nguồn mở)
    - Tái cấu trúc dùng task thay cho thread và triển khai bằng kế thừa
- Sửa) IRtcMultiBeam
    - Hoàn tất xác minh điều khiển đồng bộ độc quyền giữa các RTC
    - Hoàn tất xác minh SiriusEditorControl
        - Hỗ trợ 2 bộ dữ liệu gia công và 2 tổ hợp bút khác nhau
    - Hoàn tất xác minh SiriusMultiEditorControl
        - 1 bộ dữ liệu gia công giống nhau + 1 tổ hợp bút khác nhau
- Thêm) LogControl
    - Lọc và tìm kiếm thông báo nhật ký;
- Sửa) Shader;
    - Đối tượng không được kết xuất trong View khi chạy trong môi trường console;
    - Quản lý Shader theo từng đích trong nhiều khung nhìn;
- Sửa) Correction 3D
    - Xử lý độ phân giải 16 bit và 20 bit cho các hệ số A, B và C
    - Cải thiện thao tác dữ liệu bằng Correction3DRtcForm
- Sửa) Rò rỉ bộ nhớ
- Sửa) Lỗi lưu ảnh khung nhìn bằng Snapshot
- Sửa) Tăng tốc thực thi tập lệnh C#

## v1.6.1 (2026.4.9)

- Thêm) ViewerControl
    - Đã thêm điều khiển người dùng
    - Kết xuất đồng thời một tài liệu trong trình xem và trình biên tập
    - Bỏ giới hạn kết nối 1:1 giữa Document và View
    - Tạo và sửa Document từ bên ngoài
- Sửa) IRtc3D
    - Cải thiện quy trình hiệu chuẩn 3D dựa trên RtcCalibrationLibrary
        - 1. Hiệu chuẩn độ nghiêng chùm tia: RtcCalibrationLibrary.BeamTiltCalibration
        - 2. Hiệu chỉnh trường 2D: RtcCalibrationLibrary.XyCalibration
        - 3. Hiệu chuẩn tiêu điểm tại z=0: RtcCalibrationLibrary.FocusCalibrationAtZ0
        - 4. Hiệu chuẩn tiêu điểm cho các hệ số A, B, C: RtcCalibrationLibrary.FocusCalibrationCoeffABC
        - 5. Hiệu chuẩn giãn thể tích Z: RtcCalibrationLibrary.StretchCalibration
    - Bỏ RtcCorrection3D: thay bằng RtcCalibrationLibrary;
    - Bỏ KZScale: thay bằng bù tiêu điểm trong RtcCalibrationLibrary;
    - Bỏ ZOffset: thay bằng tịnh tiến Z trong MatrixStack;
- Thêm) Đối tượng EntityPoint;
- Thêm) Đối tượng EntityBarcode1D_V2
    - Hỗ trợ nhiều kiểu ô tương tự mã vạch 2D:
    - Có thể kết hợp điểm, đường, nét gạch, v.v.:
- Thêm) Mở và lưu danh sách đỉnh cho các đối tượng sau:
    - EntityPoints:
    - EntityPolyline2D:
    - EntityPolyline3D:
    - Điều khiển người dùng OffsetControl

## v1.5.4 (2026.4.2)

- Sửa) Bản sửa khẩn cấp
    - Ngoại lệ khi tạo SiriusEditorControl trong chế độ thiết kế
    - Ngoại lệ khi tạo SiriusMultiEditorControl trong chế độ thiết kế

## v1.5.3 (2026.3.31)

- Sửa) IDocument
    - Thêm cấu hình kết nối IView
- Thêm) IPowerMeter
    - Thêm MeasureUnits cho chế độ đo công suất và năng lượng
- Sửa) Rtc6
    - Thông báo sự kiện khi thuộc tính IsActivateAutoDelays thay đổi
    - Các mục trong EntityPen và EntityLayerPen được hiển thị khi IsActivateAutoDelays thay đổi
- Sửa) EntityBarcode2D
    - Đổi thứ tự đánh dấu qua EntityPen khi xử lý CellDot
- Sửa) IRtcMultiBeam
    - Hoàn tất xác minh xử lý token độc quyền
- Sửa) IRtcCorrection2D, IRtcCorrection3D
    - Hỗ trợ xử lý dữ liệu thô bằng ma trận nội bộ (xoay, v.v.) được thiết lập trong đầu quét

## v1.5.2 (2026.3.27)

- Thêm) Hỗ trợ điều khiển động cơ bước
    - Điều khiển động cơ bước bên ngoài qua cổng động cơ bước trên RTC5, RTC6
    - Thêm giao diện IRtcStepper
    - Thêm giao diện điều khiển StepperControl
    - Hỗ trợ di chuyển tuyệt đối và tương đối cho động cơ bước
- Thêm) Giao tiếp nối tiếp
    - Thêm giao tiếp nối tiếp qua cổng RS232 trên RTC5, RTC6
    - Thêm giao diện IRtcSerialComm
    - Thêm giao diện điều khiển SerialCommControl
    - Theo dõi dữ liệu nhị phân đã gửi và nhận trong thẻ Laser
    - Thêm sự kiện OnSerialReceived
- Thêm) Cải tiến Fly Extension
    - Cải tiến phần mở rộng Marking on the fly cho RTC6
    - Tái cấu trúc giao diện IRtcMoFExtension
    - Hỗ trợ tổ hợp 3 trục (X, Y, Z hoặc trục quay)
    - Thêm hỗ trợ giao tiếp McBSP
- Sửa) Tái cấu trúc giao diện điều khiển người dùng
    - OffsetControl
    - MarkerControl
    - ScannerControl
    - LaserControl
- Sửa) Ngăn xếp ma trận
    - Bỏ BaseMatrix khỏi MatrixStack
    - Hỗ trợ tích hợp qua IRtc.CtlMatrix và ListMatrix
- Sửa) PowerMeter
    - Sửa lỗi đọc giá trị công suất từ thiết bị CoherentPowerMax và GentecEO
- Sửa) SiriusEditorControl
    - Nhập tệp .sirius3 bên ngoài rồi thêm hoặc gộp thành lớp trong tài liệu hiện tại
    - Cây hiển thị việc sử dụng màu bút lớp trên nút lớp
- Thêm) Dán theo mảng trong điều khiển biên tập

## v1.4.1 (2026.3.10)

- Thêm) Cung cấp tài liệu qua máy chủ web
    - Trang web trực tuyến: https://spirallab.co.kr/sirius3/doc
    - Hoặc giải nén sirius3\doc\sirius3_doc_{version}.zip rồi chạy tệp lệnh 'start_doc.bat'
- Thêm) Chỉnh giá trị bằng chuột
    - Tăng hoặc giảm giá trị bằng cách giữ nút chuột phải và kéo sang trái hoặc phải trong PropertyGrid
- Sửa) Rtc6
    - Khi dùng ListLaserOn, tín hiệu SYNC OUT từ nguồn laser ngoài được đưa vào để đếm xung nhưng thời gian chờ bị xử lý dài gấp 10 lần.
- Sửa) EntityPen
    - Các giá trị Power, PowerPercentage và PowerMapCategory không hiển thị
- Sửa) SiriusEditorControl
    - Cả 4 trang đều dùng được theo mặc định
    - WaferMap và Substratemap bị vô hiệu hóa

## v1.4.0 (2026.3.3)

- Thêm) Môi trường phát triển .NET 9.0-Windows, .NET 10.0-Windows
- Thêm) Xuất theo số xung bằng tín hiệu đồng bộ từ nguồn laser ngoài
    - Đưa tín hiệu đồng bộ ngoài vào DIGITAL IN1 trên đầu nối LASER
    - IRtc.ListLaserOn(thời gian chờ, số xung, thoát khi đủ số xung)
    - Thiết lập bằng PixelPulses và IsPixelPulsesExit của bút EntityPen
        - 0: Xuất LASERON trong thời lượng điểm ảnh như trước
        - 1~65535: Xuất LASERON trong thời lượng điểm ảnh đồng thời chờ tín hiệu đồng bộ ngoài đủ số xung đã chỉ định
        - Khi dùng IsPixelPulsesExit, kết thúc ngay khi số tín hiệu đồng bộ ngoài đạt PixelPulses và chuyển sang lệnh danh sách tiếp theo
- Thêm) Giao diện IRtcMultiBeam (thử nghiệm)
    - Hệ thống nhiều chùm tia dùng một nguồn laser + hai RTC + hai bộ điều khiển RF AOM
    - Rtc6MultiBeam
- Thêm) EntityPoints
    - Hỗ trợ tối ưu đường đi ngắn nhất
- Thêm) Giao diện IRtcIO
- Sửa) EntityWaitDataExt16Cond, EntityWaitDataExt16EdgeCond, EntityWriteDataExt16, EntityWriteDataExt16Cond
    - Mặt nạ bit dùng kiểu ushort thay cho chuỗi
- Sửa) Điều khiển SiriusEditorControl
    - Ngoại lệ khi thêm trong chế độ thiết kế
    - Sửa ngoại lệ khi tạo điều khiển trong mã do OpenGL chưa được khởi tạo
    - Bỏ mã buộc Document thực hiện ActNew khi tải điều khiển
- Sửa) Giấy phép
    - Khi vượt số thực thể tối đa được phép hoặc thiếu tùy chọn
    - Trước: Không sử dụng được
    - Sau: Kích hoạt chế độ dùng thử 30 phút

## v1.3.2 (2026.2.20)

- Sửa) Hỗ trợ Extended Mode trong Automatic Laser Control
    - Cho phép kết hợp các tín hiệu Actual Velocity + Encoder + SCANAhead + Inverse Speed Correction + Backward Transformation + SDC + SkyWriting
    - Thêm cấu hình tổ hợp Extended Mode trong danh sách PoD của thuộc tính EntityLayerPen
    - Thêm EntityPoD
- Sửa) EntityPen
    - Thêm cấu hình khoảng cách điểm cho chức năng SDC trong SpotDistanceSCANa.
- Thêm) IRtcMoF
    - Thêm sự kiện báo lỗi tín hiệu bộ mã hóa: IRtcMoF.OnEncoderSignalError
    - Hỗ trợ sự kiện khi ra khỏi trường ảnh ảo: IRtcMoF.OnOutOfVirtualImageField
    - Hỗ trợ thiết lập bộ lọc tín hiệu bộ mã hóa (chỉ RTC6)
        - Dùng CtlMoFEncoderFilter để lấy trung bình cộng tín hiệu khi có nhiễu hoặc hỗ trợ tốc độ trên 4MHz
    - Khi truy vấn bộ mã hóa, có thể lấy riêng vị trí tuyệt đối và tương đối.
    - Đổi đối số của sự kiện OnEncoderChanged
- Sửa) Đổi tên IRtcWaitID thành IRtcInterrupt

## v1.3.1 (2026.2.9)

- Thêm) Giao diện IRtcSCANAhead
    - Thêm các mục riêng cho SCANAhead (Corner, End, Acc Scale) vào EntityPen
    - Hỗ trợ thiết lập Position (hoặc Trajectory) Acknowledge Limit (mặc định: 0.28% toàn dải vị trí)
    - Khi dùng RTC6 + SCANAhead, hoạt động như Trajectory ACK Limit
- Thêm) Giao diện IRtcWaitID

## v1.3.0 (2026.2.5)

- Thêm) EntityPolyline2D và EntityPolyline3D
    - Trình biên tập danh sách đỉnh
- Thêm) Điều khiển SiriusMultiEditorControl
    - Hỗ trợ gia công một tài liệu trên nhiều thiết bị
- Thêm) EntityLayerPen
    - Thêm giao diện chỉnh giá trị bút và hỗ trợ người dùng
- Thay thế) Bỏ gnuplot và dùng chức năng biểu đồ tích hợp
- Sửa) Biểu mẫu điều khiển jog máy quét không chính xác
- Thêm) Hỗ trợ chương trình chuyển đổi ODA
    - Cho phép dùng thêm bộ chuyển đổi ODA khi xử lý tệp .dwg và .dxf
    - Người dùng cần cài riêng bộ chuyển đổi ODA (https://www.opendesign.com/guestfiles/oda_file_converter)
- Giấy phép) Thay đổi chính sách giấy phép
    - Bỏ tùy chọn 3D và đưa vào chức năng cơ bản
    - Thực thể syncAXIS chuyển thành chức năng tùy chọn

## v1.2.7 (2026.1.26)

- Thêm) Thêm Variable Delays vào EntityLayerPen
    - Variable polygon delay: Đặt độ trễ đa giác biến thiên theo góc gấp (mặc định: bật)
    - Variable jump delay: Đặt độ trễ bước nhảy biến thiên theo khoảng cách nhảy
- Sửa) RTC7
    - Giá trị LaserOnShift cho Skywriting không chính xác
- Sửa) Config.IsMarkArcsIntoLines
    - True: Cung (EntityArc) và đa tuyến (EntityPolyline2D) được phân rã thành đường thẳng (ListMarkTo)
    - False: Cung (EntityArc) và đa tuyến (EntityPolyline2D) được xử lý bằng ListArcTo
- Sửa) Contour
   - IsClosed được tính sai khi trích xuất đường bao
- Sửa) Cho phép chỉnh Config.EntityPenColors và Config.LayerPenColors
- Sửa) ActRemove thất bại với đối tượng mô phỏng

## v1.2.6 (2026.1.20)

- Thêm) Thực thể elip
- Thêm) EntityLine, EntityArc, EntityPolyline2D
	- Thêm thuộc tính RampFactor để hỗ trợ Automatic laser control (defined vector)
- Thêm) IHatch.HatchRepeats để lặp nét gạch
- Sửa) Giá trị EntityPen, EntityLayerPen hiển thị không chính xác
- Sửa) Quy trình CtlCompensate của PowerMap
	- Trước: Đo lại phạm vi trái và phải
	- Sau: Cập nhật ngay dữ liệu đã đo
- Sửa) IMarker.Preview
   - Trước: Hiển thị một hình chữ nhật bao quanh các đối tượng đã chọn
   - Sau: Hiển thị từng hình chữ nhật bao của mọi đối tượng đã chọn

## v1.2.5 (2026.1.15)

- Thêm) ClipHelper cho giao cắt
- Thêm) Nhấn phím cách để kích hoạt chế độ chọn đối tượng con
- Sửa) Cải thiện phép thử giao tia cho IHitTestable
   - Config.RayHitTestPixelSize: Kiểm tra chọn với khoảng cách ngưỡng động
- Sửa) IMarker
   - Đánh dấu đệ quy đối tượng con khi dùng MarkTargets.Selected
- Cập nhật) zxing v0.16.11
- Cập nhật) clipper2 v.2.0.0

## v1.2.4 (2026.1.7)

- Thêm) Phím tắt
   - CTRL + R: Bật/tắt cho phép kết xuất
   - CTRL + M: Bật/tắt cho phép đánh dấu
   - Đổi phông hoặc màu nút khi bật/tắt quyền kết xuất hoặc đánh dấu
- Thêm) Sự kiện IRtcFreeVariable.OnFreeVariableChanged
   - Phát sinh khi giá trị FreeVariable thay đổi
- Thêm) Config.GridCloudInterval
   - Dùng khi gọi IDocument.ActGridCloud
- Sửa) Tăng tốc phân tích tệp Gerber
- Sửa) Kiểm tra chọn cung cấp thông tin chi tiết hơn
   - IDocument.SubHitEntities
- Thêm) Hàm ActHitTest khác
- Sửa) Ngoại lệ khi thực hiện ActUngroup trên nút rỗng

## v1.0.1 (2025.12.22)

- Thêm) Tệp tài liệu .chm
- Thêm) ActExpand
   - Mở rộng hoặc thu hẹp đường bao theo khoảng cách
- Thêm) Hỗ trợ thiết bị đo công suất Gentec-EO
- Cập nhật) PowerMeterOphir bằng StarLab v3.93
- Sửa) Enum kiểu nối nét gạch
- Sửa) IDocument.FindByLayerUsedPenColors
- Sửa) Thêm thông báo nhật ký cho Marker.EntityWork

## v0.9.3 (2025.12.5)

- Thêm) Thu phóng vừa khung
   - Nhấp đúp chuột trong cây
   - Sau khi mở tệp
- Thêm) TextConverters.Offset mới
   - Dùng cùng Offset.ExtensionData
- Sửa) Tệp Gerber
   - Thêm) Tùy chọn UI.Config.IsGerberWithUniformGroup để kết xuất nhanh hơn
   - Sửa) Tùy chọn UI.Config.IsGerberTessellation cho lỗi chia lưới
- Đổi tên) Scanner pen thành entity pen

## v0.9.2 (2025.11.25)

- Thêm) Chuyển thành khối và chèn khối trong menu
- Đổi tên) EntityGroup thành EntityMixedGroup
- Sửa) Lỗi ActUngroup
- Sửa) Cải thiện hiệu năng ActMixedGroup, ActUniformGroup
- Sửa) Cải thiện thời gian tải khi nhập tệp Gerber
- Sửa) Ngoại lệ tràn ngăn xếp khi lưu tệp

## v0.9.1 (2025.11.18)

- Thêm) Đưa chương trình 'gnuplot' vào gói Spirallab.Sirius3.Dependencies
- Thêm) Nút tạo nhóm đồng nhất trong trình biên tập
- Sửa) Lỗi kết xuất EntityUniformGroup
- Sửa) Rò rỉ bộ nhớ
- Sửa) Đỉnh spline không chính xác
- Sửa) Hết bộ nhớ khi tạo quá nhiều nút
- Đổi) Chữ ký Core.Initialize

## v0.8.2 (2025.11.11)

- Sửa) Lỗi phân tích định dạng HPGL
- Sửa) Bút máy quét không được áp dụng
- Sửa) Làm mới đối tượng bút máy quét/lớp khi thực hiện ActNew

## v0.8.0 (2025.11.7)

- Phiên bản xem trước cho nhà phát triển

## v0.1 (2025.03.06)

- Phát hành đầu tiên
