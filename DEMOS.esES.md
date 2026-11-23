# Lista de programas de demostración

## beginner

Inicialización de la biblioteca Sirius3
Inicialización de dispositivos, como escáneres y láseres, y conexión con la UI

## console_document

Inicialización de dispositivos, como escáneres y láseres, y uso de marcadores con documentos en una aplicación de consola

## console_syncaxis_setup

Esta demo de consola realiza la configuración y el diagnóstico de bajo nivel de un sistema XL-SCAN basado en RTC6 mediante syncAXIS.
Carga `syncAXISConfig.xml`, alterna entre simulación y hardware, controla Follow/Unfollow, mueve el escáner y la mesa y ejecuta trabajos de prueba de cuadrados, círculos, calibración y retardos de láser y sistema.

## editor_automatic_laser_control

Esta demo ajusta automáticamente la salida láser mediante Automatic Laser Control (ALC).
- El ALC basado en Defined Vector se activa desde la pluma de capa. Los valores de rampa inicial y final permiten variar linealmente la salida al comenzar y terminar el mecanizado.
- El ALC dependiente de la velocidad se activa desde la pluma de capa y permite seleccionar valores mínimo y máximo, señal de salida y modo.
- El ALC basado en SDC (Spot Distance Control) se activa desde la pluma de capa; la distancia entre puntos se configura en la pluma de entidad.
- Incluye medición para muestrear los datos de salida y mostrar los resultados gráficamente.

## editor_barcode

Permite crear distintos códigos de barras 2D y determinar sus combinaciones de celdas.
También crea entidades de texto con los mismos datos que el código de barras.
Las condiciones de procesamiento del código de barras y del texto se configuran mediante la pluma de entidad.

## editor_barcode_textconvert

Esta demo muestra cómo crear códigos de barras 2D y modificar sus datos.
- Eventos: el usuario modifica el texto justo antes del procesamiento mediante el controlador de eventos del marcador; el valor actualizado se aplica durante el procesamiento.
- Script: el usuario escribe y asigna código C#. El marcador modifica dinámicamente el texto del código de barras mediante el script justo antes del renderizado y aplica el resultado.
- Archivo externo: el usuario especifica un archivo. Antes de cada renderizado, el marcador lee su primera línea, la aplica a los datos y elimina automáticamente la línea utilizada.
- Offset: el usuario proporciona texto junto con varios desplazamientos. El marcador modifica el texto en cada posición inmediatamente antes de procesarla y lo aplica al código de barras.

## editor_clip

Demuestra `IClippable` y Deshacer/Rehacer del documento. Cargue una muestra geométrica y abra Clipping para previsualizar regiones XY en coordenadas globales y recortar trayectorias o regiones cerradas con opciones de sombreado, relleno/sentido y eliminación del origen.

## editor_dio

Esta demo utiliza la entrada/salida digital (DIO) del puerto de expansión de la tarjeta RTC.
Permite asignar nombres a los pines DIO y emitir el estado del sistema (Ready, Processing, Error) como salida.
También inicia el procesamiento al recibir una señal externa, como Start.

## editor_document

Los datos de mecanizado o recetas se gestionan en un Document, que contiene las entidades creadas.
El editor renderiza ese Document. Además de crear y sustituir documentos desde la aplicación, la demo muestra cómo representar un único documento en varios editores y visores mediante Viewer.

## editor_entity

Muestra cómo crear datos de mecanizado y datos vectoriales de Sirius3 y agregarlos a documentos y páginas.
Explica la reutilización mediante Blocks e Insert y la agrupación y gestión de varias entidades.

## editor_entity_custom

Esta demo añade tipos de entidad específicos de la aplicación a Sirius3.
Implementa rombos, marcas fiduciales y taladros personalizados, con edición de propiedades, regeneración geométrica, renderizado, clonación, sombreado, marcado e inserción en un documento.

## editor_fieldcorrection_2d

Esta demo utiliza la corrección de campo 2D del escáner.
Marca patrones a intervalos regulares, mide sus posiciones mediante un equipo como visión artificial y utiliza los errores para generar un nuevo archivo de calibración a partir del actual.
Compensa las distorsiones no lineales del plano 2D y muestra cómo cargar y seleccionar el nuevo archivo en la tabla de corrección.

## editor_fieldcorrection_3d

Esta demo utiliza la calibración de campo 3D del escáner.
Presenta la secuencia de calibración mediante RtcCalibrationLibrary.
- Calibración de la inclinación del haz respecto al movimiento del eje Z
- Calibración de la posición de enfoque óptima en Z=0
- Calibración del enfoque en distintas posiciones Z y determinación de los coeficientes (A, B, C) de Zout = A + Bl + Cl²
- Calibración de errores de tamaño por estiramiento en el espacio Z vertical

## editor_fieldcorrection_3d_pointscloud

Esta demo utiliza la corrección de campo 3D del escáner.
Genera un archivo de corrección 3D con alturas Z al procesar una nube de puntos que describe la superficie de una malla.
Al mecanizar con datos 2D, el enfoque láser se desplaza automáticamente a la superficie 3D en las posiciones X e Y correspondientes.
La aplicación proporciona X e Y; las coordenadas se proyectan sobre la superficie para realizar el mecanizado láser 3D.

## editor_hardjump

Los comandos Jump y Mark se ejecutan como microvectores a la velocidad indicada (mm/s), con intervalos de 10 microsegundos.
La demo sustituye el tramo de salto por un Hard Jump de un solo ciclo (10 μs).
Reduce considerablemente el tiempo de salto, pero aumenta la carga sobre el escáner. Úselo según sus especificaciones y las características de los datos de mecanizado.

## editor_hatch

Genera patrones de sombreado para objetos con distintos contornos cerrados.
Permite sombreados de línea o polígono, con intervalo, ángulo y posición inicial configurables.
La optimización de trayectorias (Sort) reduce el recorrido del sombreado y el tiempo de mecanizado;
cada patrón puede usar una pluma de entidad diferente para variar sus condiciones.

## editor_hatch_clip

Muestra la intersección de una región con un hueco y un rectángulo FOV, y el tramado de los contornos cerrados resultantes.

## editor_interrupt

La tarjeta RTC es un controlador en tiempo real: precarga los datos en un búfer de lista y los ejecuta en tiempo real, lo que dificulta insertar funciones de la aplicación durante el mecanizado.
La demo introduce una interrupción durante la ejecución del búfer para que la aplicación la gestione antes de continuar.
El búfer conserva su estado listo para ejecutar durante la interrupción, sin detener la ejecución de la lista.

## editor_laser_ui

Sirius3 permite ampliar los dispositivos láser mediante la implementación de ILaser.
También puede diseñar una UI adaptada a su dispositivo e integrarla directamente.
La demo crea e integra una UI personalizada mediante el evento OnCreateLaserUI.

## editor_marker

Incluye el código abierto de MarkerRtc, MarkerRtcFast y MarkerSyncAxis proporcionados por Sirius3.
Puede modificar estas implementaciones y conectarlas a Sirius3 para adaptar el método de procesamiento.

## editor_measurement_skywriting_wobbel

Permite seleccionar los modos Skywriting 1, 2, 3 y 4 desde la pluma de capa y aplicarlos al mecanizado.
También utiliza distintas formas de Wobbel mediante la pluma de entidad.
Incluye medición por muestreo de los datos de salida y visualización gráfica de los resultados.

## editor_mof_interrupt

MoF (Marking on the Fly) permite seguir y mecanizar un objeto en movimiento en tiempo real mediante su señal de codificador conectada a la tarjeta RTC.
Combina las interrupciones de `editor_interrupt` con MoF.

Antes de mecanizar el objeto se genera una interrupción para mover la mesa o cinta transportadora a la posición central.
La demo espera a que el codificador alcance esa posición, inicia el seguimiento MoF y lo finaliza al terminar el mecanizado.

## editor_mof_offsets

MoF sigue y mecaniza en tiempo real mediante las señales de un codificador de movimiento externo conectado a RTC.
Utiliza un campo de imagen virtual para procesar un mismo patrón MoF sobre un área muy grande.
- Espera a que el codificador corresponda a una de las 1,000 posiciones de desplazamiento y mecaniza en cuanto se cumple la condición.
- Supone desplazamientos X entre -200 y 0 y una mesa o cinta que avanza hacia la derecha,
- es decir, hacia valores X crecientes del escáner.
- La pluma de entidad configura velocidad y potencia y muestra las opciones SCANAhead de la combinación RTC6 + excelliSCAN.

## editor_mof_trigger

MoF procesa datos en tiempo real al seguir una señal de codificador de un objeto externo en movimiento conectada a la tarjeta RTC.
- La demo espera un disparo externo en la entrada D.IN de expansión del RTC e inicia MoF inmediatamente al recibirlo.
- Permite generar los datos deseados antes de cada código de barras y precargarlos en el búfer RTC para procesar datos dinámicos.
- También utiliza Free Variables para acumular cuántas veces se ha procesado el código de barras después del disparo.

## editor_mof_xy

MoF permite a RTC procesar códigos de barras en tiempo real siguiendo la señal de un codificador de movimiento externo.
Muestra cómo crear condiciones de espera del codificador y configurarlas para cada objeto.

## editor_mof_xy_raster

MoF permite al escáner procesar datos en tiempo real siguiendo la señal de un codificador de movimiento externo conectada a RTC.
- En ráster, como imágenes, códigos 1D/2D e ImageText, comprueba la posición del codificador justo antes de cada línea.
- Una vez terminada la espera, el ráster se realiza en el centro del escáner o de la lente.
- Es útil para mecanizar logotipos u otras marcas mediante MoF + ráster sobre el lateral de un cilindro colocado horizontalmente y en rotación.

## editor_multibeam

Esta demo utiliza MultiBeam para compartir una fuente láser entre dos cabezales de escaneo y dos AOM.
- Cuando un cabezal comienza un salto, se interrumpe la salida láser hacia ese cabezal;
- el otro sale de espera y mecaniza. El control se alterna de nuevo al alcanzar el siguiente tramo de salto.
- Este método mejora el aprovechamiento de la fuente láser para aumentar la productividad.

## editor_multibeam2

Proporciona una UI para dos instancias RTC MultiBeam que comparten una fuente láser.
Comprueba el cableado entre cabezales, selecciona Head 1, Head 2 o Both y el lado de preparación, y controla Ready, Start, Stop y Reset de uno o ambos cabezales.

## editor_multiple

Crea dos sistemas de dispositivos y dos editores con documentos propios para procesar datos distintos en un sistema multicabezal.

## editor_multiple2

Crea dos sistemas de dispositivos y un editor con un único Document para procesar los mismos datos en un sistema multicabezal.

## editor_offset

Procesa repetidamente los mismos datos en varias posiciones de desplazamiento.
Cada posición puede definir dx, dy, dz, rotación en Z y escala.

## editor_pen

Muestra cómo modificar las condiciones de mecanizado mediante plumas de capa y de entidad.
También permite sustituir mediante eventos del marcador las funciones que aplican dichas plumas.

## editor_pen_multiple

Configura valores predeterminados personalizados al crear por primera vez plumas de capa y de entidad.
También aplica distintas plumas de entidad a diferentes objetos para variar las condiciones de mecanizado.

## editor_points_sync_pulses_count

El procesamiento de puntos o píxeles suele mantener el láser activo (LASER ON) durante un tiempo establecido.
- La salida SYNC OUT de la fuente láser se conecta a RTC como señal de sincronización;
- el tiempo LASER ON del punto se sincroniza con los pulsos de salida mediante los pulsos de entrada SYNC OUT.
- La demo también muestra otras señales de sincronización disponibles.

## editor_powermap

Crea y utiliza un mapa o tabla de compensación entre la salida de la fuente y la salida real deseada.
`my_powermap.cs` contiene el código abierto de adquisición del mapa, verificación y compensación; puede adaptarlo para utilizar el mapa según sus necesidades.

## editor_remote

Establece comunicación remota mediante serie, TCP/IP, WebSocket o MQTT para leer y escribir valores de marcadores, desplazamientos y entidades desde sistemas externos.
El ejemplo utiliza WebSockets.

## editor_remote_mcp

Conecta un servidor MCP integrado a un `SiriusEditorControl` WinForms. Admite HTTP autenticado configurado mediante el entorno, edición de documentos y entidades y control de dispositivos permitidos. Cree un servidor y puerto HTTP por editor. Los comandos MCP pueden accionar hardware conectado; revise el README y el modo de acceso antes de usarlos.

## editor_scanahead

Utiliza SCANAhead, disponible en RTC6.
Al activar Auto delays, calcula automáticamente los retardos de salto, polígono y marcado y los tiempos de encendido/apagado del láser.
La lista de propiedades de la pluma de entidad muestra u oculta los ajustes según Auto delays.

## editor_scanahead_sdc

Utiliza SCANAhead, disponible en RTC6.
Combina SCANAhead y ALC mediante la pluma de capa y configura la distancia SDC (Spot Distance Control) mediante la pluma de entidad.

## editor_script

Muestra cómo los scripts del marcador actualizan datos de entidades inmediatamente antes del marcado.
Crea SiriusText con `TextConverters.SimpleScript`, genera números de serie mediante expresiones y permite inspeccionar, guardar, restaurar, cargar y compilar scripts desde `.script`, código C# o DLL.

## editor_slicer

Demuestra funciones AM (Additive Manufacturing): carga de una malla 3D, seccionado por un plano Z y sombreado de las regiones resultantes.

## editor_steppermotor

Utiliza las funciones de control del puerto de expansión para motores paso a paso de la tarjeta RTC.

## editor_stitchedimage

Crea una `EntityStitchedImage` a partir de la cuadrícula de cámara, la resolución de imagen y el campo de visión.
Simula el desplazamiento del escáner por cada mosaico y la adquisición desde archivos, muestra el resultado mediante `View.StitchedImage` y permite borrar y reconstruir la imagen compuesta.

## editor_syncaxis

Está diseñada para sistemas XL-SCAN, en lugar de tarjetas RTC convencionales.
Para sistemas de gran área con ACS Motion Control + excelliSCAN (o intelliSCAN iV) + RTC6, muestra el control sincronizado del escáner y el movimiento y la función Motion Decomposition mediante syncAXIS.

## editor_ui

Los controles y `EditorControlSupport.cs` comparten el tamaño de herramientas adaptado a DPI. Los formularios existentes con escala Font pueden activar la configuración DPI del EXE descrita en README; las ImageLists propias y los diseños de píxeles fijos deben ajustarse al DPI del monitor.

Incluye el código abierto de los controles SiriusEditorControl y SiriusMultiEditorControl proporcionados por Sirius3.
Puede utilizarlo para diseñar y personalizar la UI.

## editor_viewer

Los datos de mecanizado o recetas se gestionan en un Document que contiene las entidades creadas.
Muestra cómo conectar ese documento a un editor y un visor para renderizarlo: un documento con varias vistas.

## editor_zpl

Crea `EntityImageZPL` con varias muestras de etiquetas ZPL mediante el renderizador BinaryKits sin conexión.
Configura el tamaño y la densidad de impresión, datos ZPL Unicode, asignaciones de fuente `^CW` y fuentes de respaldo para previsualización y mecanizado locales.

## wpf_editor_entity

Esta demo WPF nativa reutiliza las muestras de `editor_entity` con `SpiralLab.Sirius3.UI.WPF.SiriusEditorControl`. Incluye edición en PropertyGrid, selección múltiple, Deshacer/Rehacer, ventanas del menú contextual y visualización de la cuadrícula en los frameworks compatibles. Utiliza dispositivos virtuales de forma predeterminada.

## wpf_editor_multiple

Esta versión WPF de `editor_multiple` aloja dos instancias independientes de `SiriusEditorControl` en pestañas. Cada editor registra sus dispositivos y posee su Document para editar y procesar datos distintos de manera independiente. El `config.ini` compartido usa dispositivos virtuales de forma predeterminada.

## wpf_editor_multiple2

Esta versión WPF de `editor_multiple2` utiliza un `SiriusMultiEditorControl` y dos sistemas de dispositivos registrados. Cambia el sistema activo conservando un Document compartido para procesar los mismos datos con cualquiera de los sistemas. El `config.ini` compartido usa dispositivos virtuales de forma predeterminada.

## wpf_editor_ui

Proporciona el código fuente público de los controles WPF nativos `SiriusEditorControl`, `SiriusMultiEditorControl` y su archivo de soporte para personalizar la interfaz del editor. El host utiliza dispositivos virtuales de forma predeterminada.
