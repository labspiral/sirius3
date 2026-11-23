# Sirius3
Nền tảng Windows/.NET cho gia công laser chính xác, tích hợp điều khiển SCANLAB, thiết bị, xử lý hình học, hiển thị OpenGL, chỉnh sửa tài liệu, lời nhắc AI qua máy chủ MCP tích hợp, mô phỏng và thực thi đánh dấu.

Ngôn ngữ: [English](README.md) · [한국어](README.koKR.md) · [简体中文](README.zhCN.md) · [日本語](README.jaJP.md) · [Deutsch](README.deDE.md) · [Tiếng Việt](README.viVN.md) · [Español](README.esES.md)

![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_logo.png)

---

## Tính năng nổi bật
![sirius3_logo1](https://spirallab.co.kr/sirius3/sirius3_logo1.png)
![sirius3_editor](https://spirallab.co.kr/sirius3/sirius3_editor.png)

- Bộ điều khiển SCANLAB RTC
   - RTC4 / RTC4e / RTC5 / RTC6 / RTC6e
   - XL-SCAN (RTC6 + ACS qua syncAXIS)
- Đo lường và phân tích
   - Ghi lại quỹ đạo máy quét và tín hiệu đầu ra dưới dạng đồ thị
   - Hiển thị đường gia công bằng mô phỏng thời gian thực
- Tùy chọn đánh dấu nâng cao
   - Độ trễ đa giác và độ trễ nhảy biến đổi
   - Đầu quét thứ hai và 3D
   - MoF (Marking on the Fly) và MoF Extension (còn gọi là Fly extension)
   - Sky Writing: chế độ 1–3 trên RTC5; chế độ 1–4 trên RTC6 với phần mềm và firmware tương thích; không hỗ trợ trên RTC4
   - Độ trễ tự động bằng SCANAhead
   - MultiBeam (1 nguồn laser + 2 AOM + 2 đầu quét)
- ALC (Automatic Laser Control) hoặc phát xung theo yêu cầu
   - Vector được xác định
      - Biến thiên tuyến tính
   - Phụ thuộc tốc độ
      - Tốc độ đặt
      - Tốc độ thực tế
   - Phụ thuộc bộ mã hóa
      - Tốc độ bộ mã hóa
   - Phụ thuộc vị trí
      - Bảng theo khoảng cách và hệ số tỷ lệ
   - Cũng hỗ trợ kết hợp SCANAhead, cộng tốc độ bộ mã hóa, hiệu chỉnh tốc độ nghịch đảo, biến đổi ngược và SDC+Skywriting
- Hiệu chỉnh trường máy quét
   - Hiệu chỉnh 2D
   - Hiệu chỉnh 3D cho độ nghiêng, tiêu điểm, hệ số a,b,c và hệ số kéo giãn
- Điều khiển công suất laser
   - Tần số, hệ số lấp đầy, tương tự và số
   - Tích hợp nhà cung cấp: AdvancedOptoWave, Coherent, IPG, JPT, Photonics Industry, Spectra Physics và các hãng khác
- Máy đo công suất và bản đồ công suất
   - Coherent (PowerMax), Thorlabs (qua OPM), Ophir (qua StarLab) và Gentec-EO
   - Bù đầu ra theo bản đồ công suất
- Kết xuất và xử lý hình học
   - Bộ kết xuất OpenGL 3.3+ 2D/3D với một camera trực giao và năm camera phối cảnh
   - Tăng tốc AABB để kiểm tra lựa chọn điểm, đường thẳng, dải đường thẳng và tam giác
   - Cắt lớp lưới 3D có xét cấu trúc tô pô, kèm chẩn đoán đường bao mở và khép kín
   - Nhiều mẫu hatch có xét chiều đường bao, vùng lồng nhau và ô mã vạch liên thông
- Đối tượng, văn bản và mã vạch
   - Điểm, đường thẳng, cung tròn, elip, đường gấp khúc, tam giác, hình chữ nhật, dấu cộng, đường xoắn ốc, Trepan, Lissajous và spline
   - Khối lập phương, hình cầu, hình trụ, hình nón, lưới, lớp, nhóm, khối và BlockInsert
   - Đối tượng Text, SiriusText, ImageText, văn bản theo vòng tròn, văn bản liên kết và kết xuất ZPL
   - Mã vạch 1D, QR, DataMatrix, PDF417 và Aztec với gia công đường bao, hatch và ô điểm
- Nhập tệp và trao đổi dữ liệu
   - Sirius3 (`.sirius3`): lưu, mở và nhập tài liệu.
   - DXF/DWG: bản vẽ CAD 2D (DWG cần ODA File Converter).
   - SVG: đường bao vector, vùng tô và văn bản có thể chỉnh sửa trong một đoạn (chuyển văn bản phức tạp thành đường dẫn).
   - HPGL/PLT: đường vector của máy vẽ.
   - Gerber/Excellon: hình mạch PCB và dữ liệu khoan.
   - PNG/BMP/GIF/JPG(JPEG)/TIF(TIFF): ảnh raster.
   - PDF: nhập trang dưới dạng ảnh raster (không hỗ trợ PDF có mật khẩu).
   - STL/OBJ/PLY/STP(STEP)/3MF: nhập và cắt lớp lưới 3D.
   - G-code (`.gcode`, `.ngc`): đường chạy dao CNC và laser.
   - CXF/LFF/FNT/DOT: phông chữ ngoài cho SiriusText.
- Giao tiếp từ xa và dữ liệu động
   - Điểm cuối TCP/IP, nối tiếp (RS-232), WebSocket và MQTT để điều khiển bộ đánh dấu và truy cập dữ liệu
   - Máy chủ HTTP MCP tích hợp để AI hỗ trợ đăng ký trình chỉnh sửa, chỉnh sửa tài liệu và đối tượng, mô phỏng và điều khiển thiết bị đã đăng ký
   - Chuyển đổi dữ liệu văn bản và mã vạch bằng sự kiện, tệp, độ lệch, đối tượng liên kết và script C#
- Tài liệu, trình chỉnh sửa và mô phỏng
   - Bốn trang tài liệu với lớp, bút, nhóm, khối và Hoàn tác/Làm lại có thể cấu hình
   - Cả WinForms và WPF đều hỗ trợ `SiriusEditorControl` / `SiriusMultiEditorControl` trong Debug và Release; một tài liệu có thể hiển thị trên nhiều khung nhìn
   - Tài nguyên giao diện bằng tiếng Anh, Hàn, Trung giản thể, Nhật, Đức, Việt và Tây Ban Nha
   - [Trình chỉnh sửa phông chữ](SpiralLab.Sirius3.UI/FontEditor/README.md): chỉnh sửa ký tự CXF/LFF/FNT/DOT độc lập từ menu **Font Edit** trong WinForms/WPF; vector theo mm, ô DOT nguyên, BBox/số đo, Hoàn tác/Làm lại, nhập DXF, xem trước văn bản và chuyển đổi TTF đã cài đặt. Luôn có trong Debug và Release.
   - Hiển thị đường laser thời gian thực với dấu chỉ có kích thước cố định trên màn hình, hiệu ứng tia và hạt tùy chọn
   - Khung nhìn ảnh ghép theo lưới cho camera và kiểm tra
- Kiến trúc mở
   - Giao diện mở rộng cho trình chỉnh sửa, đối tượng, bộ đánh dấu, máy quét, laser, máy đo công suất, bản đồ công suất và giao tiếp từ xa
   - Triển khai máy quét, laser, máy đo công suất và bộ đánh dấu ảo để phát triển và mô phỏng

## Thay đổi chính
| Đặc điểm | SIRIUS3 | SIRIUS2 |
|:-----------------------------|:-----------------------------------------|:--------------------------------------|
| Nhiều trang | 4 trang | Không có trang / một tài liệu |
| Camera | 6 camera tích hợp | Phối cảnh |
| Tốc độ kết xuất | Bộ máy shader OpenGL tăng tốc GPU | Bộ máy shader tích hợp |
| Chế độ kết xuất | Model, PerVertex, PerNormal, ZHeightMap, Texture, TexturePerVertex | Không có |
| Tốc độ kiểm tra lựa chọn | Kiểm tra điểm/đường/tam giác tăng tốc AABB | Chậm |
| Hatch | Nhiều mẫu hatch có xét chiều đường bao | Một mẫu hatch |
| Lưới 3D và cắt lớp | Bộ cắt lớp tích hợp cho STL, OBJ, PLY, STEP, 3MF | Không có |
| Gerber / Excellon | Nhập theo nhận dạng nội dung | Không có |
| Tệp phông chữ | Định dạng CXF, LFF, FNT, DOT | Định dạng CXF, LFF tùy chỉnh |
| Bút | Bút cho đối tượng và lớp | Bút đối tượng |
| MCP | Có hỗ trợ | Không có |
| Cập nhật thư viện | Qua trình quản lý gói NuGet | Thủ công |

![sirius3_hatch](https://spirallab.co.kr/sirius3/sirius3_hatch.png)
![sirius3_pod](https://spirallab.co.kr/sirius3/sirius3_pod.png)
![sirius3_slicer](https://spirallab.co.kr/sirius3/sirius3_slicer.png)
![sirius3_syncaxis](https://spirallab.co.kr/sirius3/sirius3_syncaxis.png)

## Gói / DLL
- `SpiralLab.Sirius3.Dependencies` — runtime SCANLAB RTC4/5/6 và syncAXIS, phông chữ và dữ liệu mẫu
- `SpiralLab.Sirius3` — điều khiển phần cứng và hợp đồng dùng chung cho máy quét, laser, I/O số, máy đo và bản đồ công suất
- `SpiralLab.Sirius3.UI` — đối tượng, xử lý hình học, kết xuất OpenGL, điều khiển WinForms/WPF và tích hợp MCP
 > Dễ dàng cập nhật thư viện bằng trình quản lý gói NuGet.

## Nền tảng đích
- `net481`
- `net8.0-windows`
- `net9.0-windows`
- `net10.0-windows`

## Yêu cầu hệ thống
- Windows 10/11 (x64)
- GPU/trình điều khiển hỗ trợ tối thiểu OpenGL 3.3 (khuyến nghị dùng trình điều khiển mới nhất)
- Trình điều khiển/runtime cần cho thiết bị đã chọn; tệp native từ NuGet không cài đặt trình điều khiển thiết bị
- Biên dịch: .NET Framework 4.8.1 Developer Pack cho `net481`, hoặc .NET SDK tương ứng với nền tảng hiện đại đã chọn
- Chạy: .NET Framework 4.8.1, hoặc Windows Desktop Runtime tương ứng cho ứng dụng .NET phụ thuộc framework
- Phát triển: Visual Studio 2022 với workload phát triển ứng dụng desktop .NET cho các demo công khai

## Thành phần phụ thuộc

Các phiên bản sau mô tả tích hợp Sirius3 và các thành phần phụ thuộc .NET chính. NuGet khôi phục các thành phần managed; cài riêng trình điều khiển cần cho thiết bị.

- SCANLAB
   - RTC4: v2023.11.02
   - RTC5: v2024.09.27
   - RTC6: 2026.9.10 v1.25.1
   - syncAXIS: v1.8.2 (2023.03.09)

- .NET
   - `net481`: OpenTK 3.3.3
   - `net8.0-windows`, `net9.0-windows`, `net10.0-windows`: OpenTK và OpenTK.Mathematics 4.9.4
   - Các gói phụ thuộc dùng chung cho mọi framework đích
      - Microsoft.Extensions.Logging 10.0.10
      - Microsoft.Extensions.Logging.Abstractions 10.0.10
      - Microsoft.Extensions.Logging.Debug 10.0.10
      - Newtonsoft.Json 13.0.4

## Cài đặt gói
- Thêm tham chiếu
   - [SpiralLab.Sirius3.Dependencies](https://www.nuget.org/packages/SpiralLab.Sirius3.Dependencies)
   - [SpiralLab.Sirius3](https://www.nuget.org/packages/SpiralLab.Sirius3)
   - [SpiralLab.Sirius3.UI](https://www.nuget.org/packages/SpiralLab.Sirius3.UI)

## Bắt đầu nhanh

Cài ba gói NuGet Sirius3 đã nêu. Các đoạn mã minh họa luồng chính; `...` lược bỏ việc tạo thiết bị, kiểm tra và nối sự kiện. Đặt các bước khởi động, tải và đóng vào sự kiện tương ứng của ứng dụng chủ.

Các demo công khai đọc `config*.ini`, tạo thiết bị qua `ScannerFactory`, `LaserFactory`, `IOFactory`, `PowerMeterFactory` và `MarkerFactory`, rồi khởi tạo trước khi đăng ký. `RegisterDevices` kết nối thiết bị với trình chỉnh sửa và gọi `marker.Ready` với tài liệu, khung nhìn của nó. Hàm tải INI thuộc về demo; ứng dụng chủ cung cấp luồng cấu hình riêng.

# [WinForms](#tab/winforms)

Thiết lập dự án

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <PlatformTarget>x64</PlatformTarget>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWindowsForms>true</UseWindowsForms>
    <ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>
</PropertyGroup>
```

Khởi tạo Core, tạo trình chỉnh sửa rồi đăng ký các thiết bị đã khởi tạo. Thiết lập DPI của WinForms chạy trước các bước này; xem phần HiDPI bên dưới.

```csharp
using SpiralLab.Sirius3.MCP;
using System.Windows.Forms;

// Startup: initialize Core and create the host UI.
if (!SpiralLab.Sirius3.Core.Initialize()) return;
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
editor.RegisterDevices(rtc, laser, powerMeter,
    dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

// Optional MCP: start after device registration (Remote license).
var server = MCPFactory.CreateServer(0, "MCP", editor);
server.AccessMode = MCPAccessModes.All;
editor.MCPServer = server;
await server.Start();
...
```

Khi đóng bất đồng bộ, giữ vòng lặp sự kiện giao diện hoạt động đến khi hoàn tất dọn dẹp:

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

Thiết lập dự án

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <PlatformTarget>x64</PlatformTarget>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWPF>true</UseWPF>
    <ApplicationManifest>app.manifest</ApplicationManifest>
</PropertyGroup>
```

Khởi tạo Core trước khi tạo trình chỉnh sửa WPF. Đăng ký các thiết bị đã khởi tạo khi Window tải.

```csharp
using SpiralLab.Sirius3.MCP;
using System.Windows;

// Startup: initialize Core and create the host UI.
if (!SpiralLab.Sirius3.Core.Initialize()) return;
var editor = new SpiralLab.Sirius3.UI.WPF.SiriusEditorControl();
var window = new Window { Content = editor };
...

// Loaded: create and initialize the scanner, laser, I/O, power meter and marker.
...
editor.RegisterDevices(rtc, laser, powerMeter,
    dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

// Optional MCP: start after device registration (Remote license).
var server = MCPFactory.CreateServer(0, "MCP", editor);
server.AccessMode = MCPAccessModes.All;
editor.MCPServer = server;
await server.Start();
...
```

Khi đóng bất đồng bộ, giữ vòng lặp sự kiện giao diện hoạt động đến khi hoàn tất dọn dẹp:

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

---

`DisposeDevices()` chờ máy chủ MCP và kết nối từ xa đã đăng ký trước khi giải phóng thiết bị. Hoàn tất dọn dẹp trình chỉnh sửa/tài liệu trước khi ứng dụng gọi `Core.Cleanup()`. Ví dụ chi tiết trình bày cơ chế bảo vệ sự kiện đóng và xử lý lỗi.

Tài liệu được gán vẫn thuộc sở hữu bên gọi. WPF `Unloaded` tạm dừng hiển thị; chủ động chờ dọn dẹp thiết bị, giải phóng trình chỉnh sửa và tài liệu khi kết thúc vòng đời. Kiểm tra kết quả khởi tạo, dọn thiết bị tạo dở khi lỗi và ghép mỗi lần `Core.Initialize()` thành công với `Core.Cleanup()` sau khi giao diện đóng.

Ví dụ đầy đủ: [hướng dẫn WinForms](https://github.com/labspiral/sirius3/blob/main/doc/WinForms.md), [hướng dẫn WPF](https://github.com/labspiral/sirius3/blob/main/doc/WPF.md), [các dự án demo](DEMOS.viVN.md) · [hướng dẫn tích hợp MCP](SpiralLab.Sirius3.UI/MCP/README.md).

## HiDPI trong WinForms

Cập nhật DLL/gói trước. Ứng dụng `DpiUnaware` hiện có có thể giữ hành vi cũ. Để bật tỷ lệ theo màn hình, cấu hình EXE sử dụng thư viện trước khi tạo điều khiển; DLL không thay đổi nhận biết DPI của tiến trình. Form chủ có thể giữ `AutoScaleMode.Font`, phông chữ và mức thiết kế. Kiểm tra riêng bố cục tùy chỉnh theo pixel cố định và cách tính tỷ lệ thủ công.

Với ứng dụng .NET 8/9/10 có `Main` viết thủ công, chọn Per-Monitor V2 trước `EnableVisualStyles` và `Core.Initialize`:

```csharp
#if NET8_0_OR_GREATER
Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
#endif
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
```

Với .NET Framework 4.8.1, gộp phần này vào app.config hiện có của EXE, giữ chuyển hướng liên kết, khai báo tương thích Windows 10 trong manifest và gọi `EnableVisualStyles` trước khi tạo điều khiển. Bỏ khai báo DPI không nhận biết xung đột và `EnableWindowsFormsHighDpiAutoResizing=false`.

```xml
<System.Windows.Forms.ApplicationConfigurationSection>
  <add key="DpiAwareness" value="PerMonitorV2" />
</System.Windows.Forms.ApplicationConfigurationSection>
```

Nếu dùng `ApplicationConfiguration.Initialize()`, đặt `<ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>` trong dự án EXE thay vì thêm bước khởi động khác. Cỡ phông vẫn tính bằng point. Dựng lại ImageList do ứng dụng sở hữu từ ảnh nguồn được giữ lại theo từng DPI, đặt `ImageSize`/`ColorDepth` trước khi thêm ảnh, giữ khóa và chỉ số, thay tham chiếu trước khi chỉ giải phóng danh sách cũ do mình sở hữu. WPF dùng đơn vị logic 96 DPI và đã áp dụng mật độ hiển thị; không nhân bố cục WPF với DPI lần nữa.

[Microsoft: HiDPI của .NET Framework](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support-in-windows-forms), [tự động co giãn WinForms](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/forms/autoscale).

## Chương trình demo

Demo WinForms và WPF công khai nhắm đúng `net481;net8.0-windows;net9.0-windows`, cần Visual Studio 2022 trở lên cùng SDK cho nền tảng đã chọn. Thư viện cũng hỗ trợ `net10.0-windows`. Bắt đầu với [editor_ui](https://github.com/labspiral/sirius3/tree/main/demos/editor_ui) hoặc [wpf_editor_ui](https://github.com/labspiral/sirius3/tree/main/demos/wpf_editor_ui).

- Xem [DEMOS.viVN.md](DEMOS.viVN.md)
- Tạo và khởi tạo thiết bị, sau đó đăng ký với SiriusEditorControl.
- Ví dụ: https://github.com/labspiral/sirius3/tree/main/demos

## Giấy phép
- Cần mua giấy phép để sử dụng thương mại.
- Giấy phép: số thực thể RTC + [Tùy chọn]
    - Tùy chọn MoF: gia công khi đối tượng di chuyển bằng bộ mã hóa ngoài, gồm theo dõi thời gian thực, chờ và các chức năng khác.
    - Tùy chọn MultiBeam: 1 nguồn laser + 2 AOM + 2 đầu quét, cho phép đổi đường đi tia laser theo thời gian thực trong đoạn nhảy.
    - Tùy chọn syncAXIS: giải pháp gia công vùng lớn XL-SCAN, đồng bộ đầu quét và bàn chuyển động qua bộ điều khiển ACS cùng đầu quét excelliSCAN.
    - Tùy chọn Remote: đổi công thức, điều khiển gia công và đọc/ghi dữ liệu qua socket, nối tiếp, WebSocket và MQTT; gồm máy chủ MCP tích hợp để AI hỗ trợ chỉnh sửa tài liệu/đối tượng và điều khiển thiết bị đã đăng ký.
- Xem chính sách giấy phép và thư viện bên thứ ba trong [LICENSE.viVN.txt](LICENSE.viVN.txt) và [THIRD-PARTY-NOTICES.viVN.txt](THIRD-PARTY-NOTICES.viVN.txt).
- Email: hcchoi@spirallab.co.kr | https://spirallab.co.kr
> Nếu không có khóa giấy phép, phần mềm chạy ở chế độ đánh giá với thời gian sử dụng tối đa 30 phút.

## Lịch sử phiên bản
- Xem [HISTORY.viVN.md](HISTORY.viVN.md)

## Tài liệu API
- Xem https://spirallab.co.kr/sirius3/doc
