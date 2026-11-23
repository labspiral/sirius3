# Danh sách chương trình demo

## beginner

Khởi tạo thư viện Sirius3
Khởi tạo các thiết bị như máy quét, laser và kết nối với giao diện

## console_document

Khởi tạo thiết bị như máy quét, laser và sử dụng bộ đánh dấu với tài liệu trong môi trường console

## console_syncaxis_setup

Demo console này thực hiện thiết lập và chẩn đoán cấp thấp cho hệ thống XL-SCAN dựa trên RTC6 bằng syncAXIS.
Chương trình tải `syncAXISConfig.xml`, chuyển giữa mô phỏng và phần cứng, điều khiển Follow/Unfollow, di chuyển máy quét và bàn, chạy các tác vụ thử hình vuông, hình tròn, hiệu chuẩn, độ trễ laser và độ trễ hệ thống.

## editor_automatic_laser_control

Demo tự động điều chỉnh đầu ra laser bằng Automatic Laser Control (ALC).
- ALC dựa trên Defined Vector có thể bật qua bút lớp. Giá trị ramp đầu và cuối được đặt để đầu ra biến thiên tuyến tính khi bắt đầu và kết thúc gia công.
- ALC phụ thuộc tốc độ có thể bật qua bút lớp, cho phép chọn giá trị nhỏ nhất, lớn nhất, tín hiệu đầu ra và chế độ.
- ALC dựa trên SDC (Spot Distance Control) có thể bật qua bút lớp; khoảng cách điểm được đặt bằng bút đối tượng.
- Có chức năng đo để lấy mẫu dữ liệu đầu ra và hiển thị kết quả bằng đồ thị.

## editor_barcode

Cho phép tạo nhiều loại mã vạch 2D và xác định tổ hợp ô của chúng.
Đồng thời tạo đối tượng văn bản chứa cùng dữ liệu với mã vạch.
Điều kiện gia công mã vạch và văn bản được cấu hình bằng bút đối tượng.

## editor_barcode_textconvert

Demo minh họa cách tạo mã vạch 2D và sửa dữ liệu.
- Theo sự kiện: người dùng sửa văn bản ngay trước gia công qua trình xử lý sự kiện của bộ đánh dấu; giá trị cập nhật được áp dụng trong gia công.
- Script: người dùng viết và chỉ định mã C#. Bộ đánh dấu dùng script thay đổi động văn bản mã vạch ngay trước kết xuất và áp dụng kết quả.
- Tệp ngoài: người dùng chỉ định tệp. Trước mỗi lần kết xuất, bộ đánh dấu đọc dòng đầu để cập nhật dữ liệu, áp dụng kết quả rồi tự động xóa dòng đã sử dụng.
- Offset: người dùng cung cấp văn bản cùng nhiều độ lệch vị trí. Bộ đánh dấu cập nhật văn bản tại từng vị trí ngay trước gia công và áp dụng vào mã vạch.

## editor_clip

Minh họa `IClippable` và Hoàn tác/Làm lại của tài liệu. Tải mẫu hình học rồi mở Clipping để xem trước vùng XY trong tọa độ thế giới và cắt đường dẫn hoặc vùng khép kín với tùy chọn hatch, tô/quy tắc chiều và xóa đối tượng nguồn.

## editor_dio

Demo sử dụng I/O số (DIO) của cổng mở rộng trên thẻ RTC.
Cho phép gán tên cho chân DIO và xuất trạng thái hệ thống (Ready, Processing, Error).
Cũng hỗ trợ bắt đầu gia công khi nhận tín hiệu ngoài, chẳng hạn Start.

## editor_document

Dữ liệu gia công hoặc công thức được quản lý trong Document, nơi tạo và lưu các đối tượng.
Trình chỉnh sửa kết xuất Document đó. Ngoài cách tạo và thay thế nhiều tài liệu từ ứng dụng, demo minh họa hiển thị một tài liệu trên nhiều trình chỉnh sửa và khung xem qua Viewer.

## editor_entity

Minh họa tạo dữ liệu gia công và vector do Sirius3 cung cấp rồi thêm vào tài liệu, trang.
Minh họa tái sử dụng đối tượng bằng Blocks và Insert, đồng thời nhóm và quản lý nhiều đối tượng.

## editor_entity_custom

Demo thêm kiểu đối tượng riêng của ứng dụng vào Sirius3.
Triển khai hình thoi, dấu chuẩn và lỗ khoan tùy chỉnh, gồm sửa thuộc tính, tái tạo hình học, kết xuất, sao chép, hatch, đánh dấu và chèn vào tài liệu.

## editor_fieldcorrection_2d

Demo sử dụng hiệu chỉnh trường 2D của máy quét.
Đánh dấu mẫu cách đều trên vùng gia công, đo vị trí bằng thiết bị như thị giác máy và dùng sai số để tạo tệp hiệu chuẩn mới từ tệp hiện tại.
Bù méo phi tuyến trong không gian 2D và minh họa tải, chọn tệp mới trong bảng hiệu chỉnh.

## editor_fieldcorrection_3d

Demo sử dụng hiệu chuẩn trường 3D của máy quét.
Minh họa quy trình hiệu chuẩn theo từng bước bằng RtcCalibrationLibrary.
- Hiệu chuẩn độ nghiêng tia theo chuyển động trục Z
- Hiệu chuẩn vị trí tiêu điểm tối ưu tại Z=0
- Hiệu chuẩn tiêu điểm tối ưu tại nhiều vị trí Z và xác định hệ số (A, B, C) của Zout = A + Bl + Cl²
- Hiệu chuẩn sai số kích thước kéo giãn trong không gian Z thẳng đứng

## editor_fieldcorrection_3d_pointscloud

Demo sử dụng hiệu chỉnh trường 3D của máy quét.
Tạo tệp hiệu chỉnh 3D chứa độ cao Z bằng cách xử lý đám mây điểm mô tả bề mặt lưới 3D.
Khi gia công bằng dữ liệu 2D, tiêu điểm laser tự động dịch đến bề mặt lưới 3D tại vị trí X, Y tương ứng.
Ứng dụng chỉ cung cấp X, Y; các tọa độ được chiếu lên bề mặt để gia công laser trên bề mặt 3D.

## editor_hardjump

Các lệnh Jump và Mark được xử lý thành microvector ở tốc độ chỉ định (mm/s), với khoảng lệnh 10 microsecond.
Demo thay đoạn nhảy bằng Hard Jump trong một chu kỳ (10 μs).
Cách này giảm đáng kể thời gian nhảy nhưng tăng tải lên máy quét. Cần dùng phù hợp với thông số máy quét và đặc điểm dữ liệu gia công.

## editor_hatch

Tạo mẫu hatch cho đối tượng có nhiều dạng đường bao khép kín.
Cho phép hatch dạng đường hoặc đa giác, đặt khoảng cách, góc và vị trí bắt đầu.
Tối ưu đường dẫn (Sort) giúp giảm đường chạy hatch và thời gian gia công;
mỗi mẫu có thể dùng bút đối tượng khác để thay đổi điều kiện gia công.

## editor_hatch_clip

Minh họa cách lấy phần giao của vùng có lỗ với hình chữ nhật FOV và tạo nét gạch cho các đường bao kín thu được.

## editor_interrupt

Thẻ RTC là bộ điều khiển thời gian thực: nạp trước dữ liệu vào bộ đệm danh sách và xử lý theo thời gian thực, nên khó chèn hàm ứng dụng trong lúc gia công.
Demo tạo ngắt giữa quá trình xử lý bộ đệm để ứng dụng xử lý trước khi tiếp tục.
Bộ đệm vẫn ở trạng thái sẵn sàng thực thi khi có ngắt, thay vì dừng thực thi danh sách.

## editor_laser_ui

Sirius3 cho phép mở rộng thiết bị laser bằng cách triển khai ILaser.
Cũng có thể thiết kế giao diện riêng cho thiết bị laser và tích hợp trực tiếp.
Demo tạo và tích hợp giao diện tùy chỉnh qua sự kiện OnCreateLaserUI.

## editor_marker

Bao gồm mã nguồn mở của MarkerRtc, MarkerRtcFast và MarkerSyncAxis do Sirius3 cung cấp.
Có thể sửa các triển khai này và kết nối với Sirius3 để tùy chỉnh phương thức gia công.

## editor_measurement_skywriting_wobbel

Cho phép chọn chế độ Skywriting 1, 2, 3, 4 qua bút lớp và áp dụng khi gia công.
Cũng sử dụng nhiều dạng Wobbel qua bút đối tượng.
Bao gồm đo lấy mẫu dữ liệu đầu ra và hiển thị kết quả bằng đồ thị.

## editor_mof_interrupt

MoF (Marking on the Fly) cho phép theo dõi và gia công đối tượng di chuyển theo thời gian thực bằng tín hiệu bộ mã hóa đưa vào thẻ RTC.
Kết hợp chức năng ngắt của `editor_interrupt` với MoF.

Trước khi gia công đối tượng, chương trình tạo ngắt để di chuyển bàn hoặc băng tải đến vị trí trung tâm.
Demo chờ bộ mã hóa đạt vị trí đó, bắt đầu theo dõi MoF và kết thúc MoF khi gia công hoàn tất.

## editor_mof_offsets

MoF theo dõi và gia công thời gian thực bằng tín hiệu bộ mã hóa từ đối tượng di chuyển bên ngoài đưa vào RTC.
Sử dụng trường ảnh ảo để gia công một mẫu MoF trên vùng rất lớn.
- Chờ bộ mã hóa tương ứng với một trong 1,000 vị trí độ lệch và gia công ngay khi thỏa điều kiện.
- Giả sử độ lệch X trong khoảng -200 đến 0 và bàn hoặc băng tải di chuyển sang phải,
- tức theo chiều X tăng của máy quét.
- Bút đối tượng thiết lập tốc độ, công suất và minh họa tùy chọn SCANAhead trong tổ hợp RTC6 + excelliSCAN.

## editor_mof_trigger

MoF xử lý dữ liệu thời gian thực bằng cách theo dõi tín hiệu bộ mã hóa của đối tượng di chuyển bên ngoài đưa vào thẻ RTC.
- Demo chờ tín hiệu kích hoạt ngoài ở đầu vào D.IN mở rộng của RTC và bắt đầu MoF ngay khi nhận được.
- Cho phép tạo dữ liệu mong muốn ngay trước khi gia công từng mã vạch và nạp trước vào bộ đệm RTC để xử lý dữ liệu động.
- Cũng dùng Free Variables để tích lũy số lần mã vạch thực sự được gia công sau tín hiệu kích hoạt.

## editor_mof_xy

MoF cho phép RTC gia công mã vạch theo thời gian thực bằng cách theo dõi tín hiệu bộ mã hóa chuyển động ngoài.
Minh họa tạo điều kiện chờ bộ mã hóa và cấu hình riêng cho từng đối tượng.

## editor_mof_xy_raster

MoF cho phép máy quét xử lý dữ liệu thời gian thực bằng cách theo dõi tín hiệu bộ mã hóa chuyển động ngoài đưa vào RTC.
- Với gia công raster như ảnh, mã vạch 1D/2D và ImageText, vị trí bộ mã hóa được kiểm tra ngay trước từng dòng.
- Sau khi kết thúc chờ, raster được gia công tại tâm máy quét hoặc thấu kính.
- Phù hợp với gia công MoF + raster cho logo hoặc dấu khác trên mặt bên của trụ đặt nằm ngang và quay.

## editor_multibeam

Demo sử dụng MultiBeam để chia sẻ một nguồn laser giữa hai đầu quét và hai AOM.
- Khi một đầu quét bắt đầu nhảy, đầu ra laser tới đầu đó được ngắt;
- đầu còn lại rời trạng thái chờ để gia công. Quyền điều khiển tiếp tục luân phiên khi đến đoạn nhảy kế tiếp.
- Phương pháp này tận dụng nguồn laser tốt hơn để tăng năng suất.

## editor_multibeam2

Cung cấp giao diện cho một cặp thực thể RTC MultiBeam dùng chung nguồn laser.
Kiểm tra nối dây tín hiệu giữa các đầu, chọn Head 1, Head 2 hoặc Both và phía chuẩn bị, điều khiển Ready, Start, Stop, Reset cho từng đầu hoặc cả hai.

## editor_multiple

Tạo hai hệ thống thiết bị và hai trình chỉnh sửa với tài liệu riêng để gia công dữ liệu khác nhau trên hệ thống nhiều đầu quét.

## editor_multiple2

Tạo hai hệ thống thiết bị và một trình chỉnh sửa với một Document để gia công cùng dữ liệu trên hệ thống nhiều đầu quét.

## editor_offset

Gia công lặp lại cùng dữ liệu tại nhiều vị trí độ lệch.
Mỗi vị trí có thể đặt dx, dy, dz, góc xoay trục Z và tỷ lệ.

## editor_pen

Minh họa sửa điều kiện gia công bằng bút lớp và bút đối tượng.
Cũng minh họa ghi đè hàm áp dụng các bút qua sự kiện bộ đánh dấu.

## editor_pen_multiple

Tùy chỉnh giá trị mặc định khi lần đầu tạo bút lớp và bút đối tượng.
Cũng áp dụng bút đối tượng khác nhau cho từng đối tượng để thay đổi điều kiện gia công.

## editor_points_sync_pulses_count

Gia công điểm hoặc pixel thường giữ laser bật (LASER ON) trong thời gian chỉ định.
- Đầu ra SYNC OUT của nguồn laser được đưa vào RTC làm tín hiệu đồng bộ;
- thời gian LASER ON của điểm được đồng bộ với xung đầu ra bằng xung SYNC OUT đầu vào.
- Demo cũng minh họa các tín hiệu đồng bộ khác có thể sử dụng.

## editor_powermap

Tạo và dùng bản đồ hoặc bảng bù giữa đầu ra nguồn laser và đầu ra thực tế mong muốn.
`my_powermap.cs` chứa mã nguồn mở cho quy trình lập bản đồ, xác minh và bù; có thể sửa để sử dụng bản đồ theo nhu cầu.

## editor_remote

Thiết lập giao tiếp từ xa qua nối tiếp, TCP/IP, WebSocket hoặc MQTT để hệ thống ngoài đọc, ghi giá trị bộ đánh dấu, độ lệch và đối tượng.
Ví dụ sử dụng WebSockets.

## editor_remote_mcp

Kết nối một máy chủ MCP tích hợp với một `SiriusEditorControl` WinForms. Hỗ trợ HTTP xác thực cấu hình qua môi trường, chỉnh sửa tài liệu, đối tượng và điều khiển thiết bị theo danh sách cho phép. Tạo máy chủ và cổng HTTP riêng cho từng trình chỉnh sửa. Lệnh MCP có thể vận hành phần cứng đã kết nối; kiểm tra README của demo và chế độ truy cập trước khi dùng.

## editor_scanahead

Sử dụng SCANAhead do RTC6 hỗ trợ.
Khi bật Auto delays, tự động tính và áp dụng độ trễ nhảy, đa giác, đánh dấu và thời gian bật/tắt laser.
Danh sách thuộc tính bút đối tượng tự động hiển thị hoặc ẩn thiết lập theo Auto delays.

## editor_scanahead_sdc

Sử dụng SCANAhead do RTC6 hỗ trợ.
Kết hợp SCANAhead và ALC bằng bút lớp, đặt khoảng cách SDC (Spot Distance Control) bằng bút đối tượng.

## editor_script

Minh họa script của bộ đánh dấu cập nhật dữ liệu đối tượng ngay trước đánh dấu.
Tạo SiriusText bằng `TextConverters.SimpleScript`, dùng biểu thức tạo số sê-ri và minh họa kiểm tra, lưu, khôi phục, tải, biên dịch script từ tệp `.script`, mã C# hoặc DLL.

## editor_slicer

Minh họa chức năng AM (Additive Manufacturing): tải lưới 3D, cắt theo mặt phẳng Z chỉ định và hatch vùng thu được.

## editor_steppermotor

Sử dụng chức năng điều khiển cổng mở rộng động cơ bước trên thẻ RTC.

## editor_stitchedimage

Tạo `EntityStitchedImage` từ lưới camera, độ phân giải ảnh và trường nhìn.
Mô phỏng máy quét di chuyển đến từng ô và lấy ảnh từ tệp, hiển thị kết quả ghép qua `View.StitchedImage`, minh họa xóa và dựng lại ảnh ghép.

## editor_syncaxis

Dành cho hệ thống XL-SCAN thay vì thẻ RTC thông thường.
Dành cho hệ thống vùng lớn kết hợp ACS Motion Control + excelliSCAN (hoặc intelliSCAN iV) + RTC6, minh họa điều khiển đồng bộ máy quét và bộ điều khiển chuyển động, cùng Motion Decomposition bằng syncAXIS.

## editor_ui

Các điều khiển và `EditorControlSupport.cs` dùng chung cơ chế tính kích thước thanh công cụ theo DPI. Form chủ hiện có dùng tỷ lệ Font có thể bật thiết lập DPI của EXE trong README; ImageList do ứng dụng sở hữu và bố cục pixel cố định cần phù hợp DPI màn hình.

Bao gồm mã nguồn mở của các điều khiển SiriusEditorControl và SiriusMultiEditorControl do Sirius3 cung cấp.
Có thể dùng mã nguồn này để thiết kế và tùy chỉnh giao diện.

## editor_viewer

Dữ liệu gia công hoặc công thức được quản lý trong Document, nơi tạo và lưu đối tượng.
Minh họa kết nối tài liệu với trình chỉnh sửa và khung xem để kết xuất: một tài liệu với nhiều khung nhìn.

## editor_zpl

Tạo `EntityImageZPL` từ nhiều mẫu nhãn ZPL bằng bộ kết xuất BinaryKits ngoại tuyến.
Thiết lập kích thước, mật độ in, dữ liệu ZPL Unicode, ánh xạ phông `^CW` và phông dự phòng để xem trước, gia công cục bộ.

## wpf_editor_entity

Demo WPF native tái sử dụng mẫu `editor_entity` với `SpiralLab.Sirius3.UI.WPF.SiriusEditorControl`. Minh họa sửa PropertyGrid, chọn nhiều, Hoàn tác/Làm lại, cửa sổ menu chuột phải và hiển thị lưới trên các framework hỗ trợ. Mặc định dùng thiết bị ảo.

## wpf_editor_multiple

Phiên bản WPF của `editor_multiple` chứa hai thực thể `SiriusEditorControl` độc lập trong các tab. Mỗi trình chỉnh sửa đăng ký hệ thống thiết bị và sở hữu Document riêng để chỉnh sửa, gia công dữ liệu khác nhau độc lập. `config.ini` dùng chung mặc định sử dụng thiết bị ảo.

## wpf_editor_multiple2

Phiên bản WPF của `editor_multiple2` sử dụng một `SiriusMultiEditorControl` với hai hệ thống thiết bị đã đăng ký. Chuyển hệ thống hoạt động trong khi giữ một Document dùng chung để cả hai hệ thống có thể gia công cùng dữ liệu. `config.ini` dùng chung mặc định sử dụng thiết bị ảo.

## wpf_editor_ui

Cung cấp mã nguồn công khai của `SiriusEditorControl`, `SiriusMultiEditorControl` WPF native và tệp hỗ trợ để tùy chỉnh giao diện biên tập. Host mặc định dùng thiết bị ảo.
