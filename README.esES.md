# Sirius3
Plataforma Windows/.NET para el procesamiento láser de precisión que integra control SCANLAB, dispositivos, geometría, visualización OpenGL, edición de documentos, instrucciones de IA mediante un servidor MCP integrado, simulación y ejecución del marcado.

Idiomas: [English](README.md) · [한국어](README.koKR.md) · [简体中文](README.zhCN.md) · [日本語](README.jaJP.md) · [Deutsch](README.deDE.md) · [Tiếng Việt](README.viVN.md) · [Español](README.esES.md)

![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_logo.png)

---

## Características principales
![sirius3_logo1](https://spirallab.co.kr/sirius3/sirius3_logo1.png)
![sirius3_editor](https://spirallab.co.kr/sirius3/sirius3_editor.png)

- Controladores SCANLAB RTC
   - RTC4 / RTC4e / RTC5 / RTC6 / RTC6e
   - XL-SCAN (RTC6 + ACS mediante syncAXIS)
- Medición y análisis
   - Registro de las trayectorias del escáner y las señales de salida con gráficos
   - Visualización de trayectorias de mecanizado mediante simulación en tiempo real
- Opciones avanzadas de marcado
   - Retardos variables de polígono y salto
   - Segundo cabezal y 3D
   - MoF (Marking on the Fly) y MoF Extension (también Fly extension)
   - Sky Writing: modos 1–3 en RTC5; modos 1–4 en RTC6 con software y firmware compatibles; no disponible en RTC4
   - Retardos automáticos mediante SCANAhead
   - MultiBeam (1 fuente láser + 2 AOM + 2 cabezales de escaneo)
- ALC (Automatic Laser Control) o pulsos bajo demanda
   - Vector definido
      - Rampa
   - Dependiente de la velocidad
      - Velocidad establecida
      - Velocidad real
   - Dependiente del codificador
      - Velocidad del codificador
   - Dependiente de la posición
      - Tabla por distancia y factor de escala
   - También admite combinaciones de SCANAhead, suma de velocidad del codificador, corrección de velocidad inversa, transformación inversa y SDC+Skywriting
- Corrección del campo del escáner
   - Corrección 2D
   - Corrección 3D de inclinación, enfoque, coeficientes a,b,c y factores de estiramiento
- Control de potencia láser
   - Frecuencia, ciclo de trabajo, analógico y digital
   - Integraciones de proveedores: AdvancedOptoWave, Coherent, IPG, JPT, Photonics Industry, Spectra Physics y otros
- Medidores y mapas de potencia
   - Coherent (PowerMax), Thorlabs (mediante OPM), Ophir (mediante StarLab) y Gentec-EO
   - Compensación de salida basada en mapas de potencia
- Renderizado y procesamiento geométrico
   - Renderizador 2D/3D OpenGL 3.3+ con una cámara ortográfica y cinco cámaras en perspectiva
   - Aceleración AABB para pruebas de selección de puntos, líneas, tiras de líneas y triángulos
   - Seccionado de mallas 3D que respeta la topología, con diagnóstico de contornos abiertos y cerrados
   - Múltiples tramas que respetan el sentido de los contornos, las regiones anidadas y las celdas de códigos de barras conectadas
- Entidades, texto y códigos de barras
   - Punto(s), línea, arco, elipse, polilínea, triángulo, rectángulo, cruz, espiral, Trepan, Lissajous y spline
   - Cubo, esfera, cilindro, cono, malla, capa, grupo, bloque y BlockInsert
   - Entidades Text, SiriusText, ImageText, texto circular, texto vinculado y renderizado ZPL
   - Códigos 1D, QR, DataMatrix, PDF417 y Aztec con procesamiento de contornos, tramado y celdas de puntos
- Importación de archivos e interoperabilidad
   - Sirius3 (`.sirius3`): guardar, abrir e importar documentos.
   - DXF/DWG: dibujos CAD 2D (DWG requiere ODA File Converter).
   - SVG: contornos vectoriales, rellenos y texto editable de un solo tramo (convierta el texto complejo en trayectorias).
   - HPGL/PLT: trayectorias vectoriales de trazadores.
   - Gerber/Excellon: diseños de PCB y datos de perforación.
   - PNG/BMP/GIF/JPG(JPEG)/TIF(TIFF): imágenes ráster.
   - PDF: importar páginas como imágenes ráster (no admite PDF protegidos por contraseña).
   - STL/OBJ/PLY/STP(STEP)/3MF: importación y seccionado de mallas 3D.
   - G-code (`.gcode`, `.ngc`): trayectorias para CNC y láser.
   - CXF/LFF/FNT/DOT: fuentes externas para SiriusText.
- Comunicación remota y datos dinámicos
   - Conexiones TCP/IP, serie (RS-232), WebSocket y MQTT para controlar el marcador y acceder a datos
   - Servidor HTTP MCP integrado para registro de editores, edición de documentos y entidades, simulación y control de dispositivos registrados mediante IA
   - Conversión de datos de texto y códigos de barras mediante eventos, archivos, desplazamientos, entidades vinculadas y scripts C#
- Documentos, editores y simulación
   - Cuatro páginas de documento con capas, plumas, grupos, bloques y Deshacer/Rehacer configurable
   - WinForms y WPF admiten `SiriusEditorControl` / `SiriusMultiEditorControl` en Debug y Release; un documento puede mostrarse en varias vistas
   - Recursos de UI en inglés, coreano, chino simplificado, japonés, alemán, vietnamita y español
   - [Editor de fuentes](SpiralLab.Sirius3.UI/FontEditor/README.md): edición independiente de caracteres CXF/LFF/FNT/DOT desde **Font Edit** en WinForms/WPF; vectores en mm, celdas DOT enteras, BBox/métricas, Deshacer/Rehacer, importación DXF, vista previa del texto y conversión de TTF instaladas. Incluido siempre en Debug y Release.
   - Visualización de trayectorias láser en tiempo real con marcadores de tamaño fijo en pantalla, haz y partículas opcionales
   - Vista de imágenes compuestas en cuadrícula para cámara e inspección
- Arquitectura abierta
   - Interfaces ampliables de editor, entidad, marcador, escáner, láser, medidor de potencia, mapa de potencia y comunicación remota
   - Implementaciones virtuales de escáner, láser, medidor de potencia y marcador para desarrollo y simulación

## Cambios principales
| Característica | SIRIUS3 | SIRIUS2 |
|:-----------------------------|:-----------------------------------------|:--------------------------------------|
| Varias páginas | 4 páginas | Sin páginas / documento único |
| Cámara | 6 cámaras integradas | Perspectiva |
| Velocidad de renderizado | Motor de sombreadores OpenGL acelerado por GPU | Motor de sombreadores integrado |
| Modo de renderizado | Model, PerVertex, PerNormal, ZHeightMap, Texture, TexturePerVertex | Ninguno |
| Velocidad de selección | Pruebas de puntos/líneas/triángulos aceleradas por AABB | Lenta |
| Tramado | Múltiples tramas que respetan el sentido de los contornos | Tramado único |
| Mallas 3D y seccionado | Seccionador integrado para STL, OBJ, PLY, STEP, 3MF | Ninguno |
| Gerber / Excellon | Importación detectada por contenido | Ninguna |
| Archivos de fuentes | Formatos CXF, LFF, FNT, DOT | Formatos CXF, LFF personalizados |
| Plumas | Plumas de entidad y capa | Pluma de entidad |
| MCP | Compatible | No disponible |
| Actualización de la biblioteca | Mediante el Administrador de paquetes NuGet | Manual |

![sirius3_hatch](https://spirallab.co.kr/sirius3/sirius3_hatch.png)
![sirius3_pod](https://spirallab.co.kr/sirius3/sirius3_pod.png)
![sirius3_slicer](https://spirallab.co.kr/sirius3/sirius3_slicer.png)
![sirius3_syncaxis](https://spirallab.co.kr/sirius3/sirius3_syncaxis.png)

## Paquetes / DLL
- `SpiralLab.Sirius3.Dependencies` — runtimes SCANLAB RTC4/5/6 y syncAXIS, fuentes y datos de ejemplo
- `SpiralLab.Sirius3` — control de hardware y contratos compartidos para escáneres, láseres, E/S digital, medidores y mapas de potencia
- `SpiralLab.Sirius3.UI` — entidades, procesamiento geométrico, renderizado OpenGL, controles WinForms/WPF e integración MCP
 > El Administrador de paquetes NuGet facilita la actualización de las bibliotecas.

## Plataformas de destino
- `net481`
- `net8.0-windows`
- `net9.0-windows`
- `net10.0-windows`

## Requisitos del sistema
- Windows 10/11 (x64)
- GPU/controlador con OpenGL 3.3 como mínimo (se recomienda encarecidamente el controlador más reciente)
- Controladores/runtimes requeridos por los dispositivos seleccionados; los archivos nativos de NuGet no instalan los controladores de dispositivo
- Compilación: .NET Framework 4.8.1 Developer Pack para `net481`, o el SDK de .NET correspondiente al destino moderno elegido
- Ejecución: .NET Framework 4.8.1, o Windows Desktop Runtime correspondiente para aplicaciones .NET dependientes del framework
- Desarrollo: Visual Studio 2022 con la carga de trabajo de desarrollo de escritorio .NET para las demos públicas

## Dependencias

Las versiones siguientes describen las integraciones Sirius3 y las principales dependencias .NET. NuGet restaura las dependencias administradas; instale por separado los controladores de sus dispositivos.

- SCANLAB
   - RTC4: v2023.11.02
   - RTC5: v2024.09.27
   - RTC6: 2026.9.10 v1.25.1
   - syncAXIS: v1.8.2 (2023.03.09)

- .NET
   - `net481`: OpenTK 3.3.3
   - `net8.0-windows`, `net9.0-windows`, `net10.0-windows`: OpenTK y OpenTK.Mathematics 4.9.4
   - Dependencias de paquetes comunes a todos los frameworks de destino
      - Microsoft.Extensions.Logging 10.0.10
      - Microsoft.Extensions.Logging.Abstractions 10.0.10
      - Microsoft.Extensions.Logging.Debug 10.0.10
      - Newtonsoft.Json 13.0.4

## Instalar paquetes
- Agregar referencias
   - [SpiralLab.Sirius3.Dependencies](https://www.nuget.org/packages/SpiralLab.Sirius3.Dependencies)
   - [SpiralLab.Sirius3](https://www.nuget.org/packages/SpiralLab.Sirius3)
   - [SpiralLab.Sirius3.UI](https://www.nuget.org/packages/SpiralLab.Sirius3.UI)

## Inicio rápido

Instale los tres paquetes NuGet Sirius3 anteriores. Los fragmentos muestran el flujo principal; `...` omite creación de dispositivos, validación y conexión de eventos. Coloque las etapas de inicio, carga y cierre en los eventos correspondientes del host.

Las demos públicas leen `config*.ini`, crean dispositivos mediante `ScannerFactory`, `LaserFactory`, `IOFactory`, `PowerMeterFactory` y `MarkerFactory`, y los inicializan antes del registro. `RegisterDevices` los conecta al editor y llama a `marker.Ready` con su documento y vista. El cargador INI pertenece a la demo; cada aplicación host proporciona su propio flujo de configuración.

# [WinForms](#tab/winforms)

Configuración del proyecto

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <PlatformTarget>x64</PlatformTarget>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWindowsForms>true</UseWindowsForms>
    <ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>
</PropertyGroup>
```

Inicialice Core, cree el editor y registre los dispositivos ya inicializados. La configuración DPI de WinForms se ejecuta antes; consulte la sección HiDPI siguiente.

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

Durante el cierre asíncrono, mantenga activo el bucle de eventos de la UI hasta finalizar la limpieza:

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

Configuración del proyecto

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <PlatformTarget>x64</PlatformTarget>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWPF>true</UseWPF>
    <ApplicationManifest>app.manifest</ApplicationManifest>
</PropertyGroup>
```

Inicialice Core antes de construir el editor WPF. Registre los dispositivos inicializados al cargar la Window.

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

Durante el cierre asíncrono, mantenga activo el bucle de eventos de la UI hasta finalizar la limpieza:

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

`DisposeDevices()` espera a los servidores MCP y las conexiones remotas registrados antes de liberar los dispositivos. Termine la limpieza del editor/documento antes de llamar a `Core.Cleanup()`. Los ejemplos detallados incluyen las protecciones del cierre y la gestión de errores.

Los documentos asignados siguen siendo propiedad del llamador. WPF `Unloaded` pausa la visualización; espere explícitamente la limpieza de dispositivos y libere el editor y el documento al terminar su vida útil. Compruebe la inicialización, limpie los dispositivos creados parcialmente si hay errores y corresponda cada `Core.Initialize()` satisfactorio con `Core.Cleanup()` tras cerrar la UI.

Ejemplos completos: [guía WinForms](https://github.com/labspiral/sirius3/blob/main/doc/WinForms.md), [guía WPF](https://github.com/labspiral/sirius3/blob/main/doc/WPF.md), [proyectos de demostración](DEMOS.esES.md) · [guía de integración MCP](SpiralLab.Sirius3.UI/MCP/README.md).

## HiDPI en WinForms

Actualice primero la DLL o el paquete. Las aplicaciones `DpiUnaware` existentes pueden conservar su comportamiento. Para activar el escalado por monitor, configure el EXE consumidor antes de crear controles; la DLL no cambia la conciencia DPI del proceso. Los formularios host pueden conservar `AutoScaleMode.Font`, su fuente y la base del diseñador. Revise por separado los diseños personalizados con píxeles fijos y el escalado manual.

En aplicaciones .NET 8/9/10 con `Main` manual, seleccione Per-Monitor V2 antes de `EnableVisualStyles` y `Core.Initialize`:

```csharp
#if NET8_0_OR_GREATER
Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
#endif
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
```

En .NET Framework 4.8.1, combine esta sección con app.config del EXE, conserve las redirecciones de enlace, declare compatibilidad con Windows 10 en el manifiesto y llame a `EnableVisualStyles` antes de crear controles. Quite las declaraciones DPI incompatibles de modo no consciente y `EnableWindowsFormsHighDpiAutoResizing=false`.

```xml
<System.Windows.Forms.ApplicationConfigurationSection>
  <add key="DpiAwareness" value="PerMonitorV2" />
</System.Windows.Forms.ApplicationConfigurationSection>
```

Si utiliza `ApplicationConfiguration.Initialize()`, establezca `<ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>` en el proyecto EXE en lugar de agregar otro inicio. Los tamaños de fuente siguen en puntos. Reconstruya las ImageLists propiedad de la aplicación desde las imágenes de origen conservadas en cada DPI, establezca `ImageSize`/`ColorDepth` antes de agregar imágenes, conserve claves e índices y sustituya las referencias antes de liberar solo las listas antiguas propias. WPF usa unidades lógicas de 96 DPI y ya aplica la densidad; no vuelva a multiplicar su diseño por DPI.

[Microsoft: HiDPI en .NET Framework](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support-in-windows-forms), [escalado automático de WinForms](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/forms/autoscale).

## Programas de demostración

Las demos públicas WinForms y WPF usan exactamente `net481;net8.0-windows;net9.0-windows` y requieren Visual Studio 2022 o posterior con los SDK de los destinos elegidos. La biblioteca también admite `net10.0-windows`. Comience con [editor_ui](https://github.com/labspiral/sirius3/tree/main/demos/editor_ui) o [wpf_editor_ui](https://github.com/labspiral/sirius3/tree/main/demos/wpf_editor_ui).

- Consulte [DEMOS.esES.md](DEMOS.esES.md)
- Cree e inicialice los dispositivos y regístrelos con SiriusEditorControl.
- Ejemplos: https://github.com/labspiral/sirius3/tree/main/demos

## Licencia
- El uso comercial requiere adquirir una licencia.
- Licencia: número de instancias RTC + [Opciones]
    - Opción MoF: procesamiento en movimiento mediante un codificador externo, con seguimiento en tiempo real, espera y otras funciones.
    - Opción MultiBeam: 1 fuente láser + 2 AOM + 2 cabezales de escaneo, con modificación en tiempo real de la trayectoria del haz durante los saltos.
    - Opción syncAXIS: solución XL-SCAN de procesamiento de áreas grandes con control sincronizado de cabezal y mesa mediante un controlador ACS y cabezales excelliSCAN.
    - Opción Remote: cambios de receta, control del procesamiento y lectura/escritura de datos mediante sockets, serie, WebSocket y MQTT; incluye el servidor MCP integrado para edición de documentos/entidades y control de dispositivos registrados mediante IA.
- Para las políticas de licencia y bibliotecas de terceros, consulte [LICENSE.esES.txt](LICENSE.esES.txt) y [THIRD-PARTY-NOTICES.esES.txt](THIRD-PARTY-NOTICES.esES.txt).
- Correo: hcchoi@spirallab.co.kr | https://spirallab.co.kr
> Si no se proporciona una clave de licencia, el software funciona en modo de evaluación con un límite de 30 minutos.

## Historial de versiones
- Consulte [HISTORY.esES.md](HISTORY.esES.md)

## Documentación de la API
- Consulte https://spirallab.co.kr/sirius3/doc
