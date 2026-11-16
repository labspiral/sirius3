# Historial de versiones de Sirius3

## v1.17.2 (2026.12.x)

- added) Añadidos recursos en vietnamita y español
- updated) Añadidas descripciones detalladas de los procedimientos de calibración 3D

## v1.17.1 (2026.11.11)

- Añadido) `Rtc6SyncAxis`: pantalla de configuración
- Añadido) `Rtc6Ethernet`: supervisión watchdog
- Corregido) `IDocument.ActClip`: opción `ClipOptions.IsIncludeOnce`
  - Los datos compartidos en un límite se generan en una sola región de recorte

## v1.17.0 (2026.10.26)

- Actualizado) Mejora del manual del desarrollador
- Añadido) `IGrippable`
  - Creación interactiva de entidades y edición de vértices, posición, tamaño y rotación
  - Movimiento con ajuste mediante combinaciones de Ctrl, Alt y Shift
- Añadido) `FontEditorControl`
  - Creación y edición de archivos de fuentes para `EntitySiriusText`
- Añadido) `EntityImagePDF`
  - Importación PDF mediante `IDocument.ActImport`
- Corregido) `EntityMixedGroup`, `EntityUniformGroup`, `EntityGerber`
  - Renderizado más rápido al editar entidades hijas de entidades compuestas
- Corregido) `MCPServerOptions.AllowedFileRoots`
  - El acceso a archivos MCP permite por defecto todas las rutas accesibles al proceso host

## v1.16.1 (2026.10.12)

- Actualizado) Dependencia SCANLAB RTC6 actualizada a Software Package 1.25.1
- Añadido) Importación de archivos .3MF: 'IDocument.ActImport'
- Añadido) Importación de archivos .SVG: 'IDocument.ActImport'
- Añadido) `IPowerMap` permite elegir interpolación lineal o interpolación de curva no lineal (PCHIP): 'Config.PowerMapInterpolationMode'
- Añadido) `IClippable` divide datos de áreas grandes en regiones rectangulares mediante recorte: 'IDocument.ActClip'
- Corregido) Orden de las máscaras corregido en `ListWriteMaskExtDO16If`

## v1.15.0 (2026.10.1)

- Añadido) Compatibilidad HiDPI en WinForms
- Corregido) Liberación asíncrona de recursos de IRemote e IMCPServer
- Añadido) Renderizado de texturas en mallas 3D

## v1.14.4 (2026.9.24)

- Añadido) Compatibilidad MCP (Model Context Protocol)
  - Servidor MCP para que clientes de IA registren editores, editen documentos y entidades y controlen dispositivos
- Añadido) Compatibilidad WPF
  - Editores WPF (`SiriusEditorControl`, `SiriusMultiEditorControl`)
  - Versión preliminar
- Actualizado) `Microsoft.Extensions.Logging`, `Microsoft.Extensions.Logging.Abstractions` y `Microsoft.Extensions.Logging.Debug` unificados en 10.0.10 para todos los frameworks
- Corregido) Compatibilidad ampliada para salida multilingüe
- Corregido) Registro de excepciones mejorado con pilas de llamadas
- Corregido) Inicialización de `Rtc6SyncAxis`

## v1.13.0 (2026.8.31)

- Actualizado) SCANLAB RTC6 actualizado a Software Package 1.25.0; selección automática del firmware según la revisión de la tarjeta al inicializar y valores `RtcRevision` 0 para Revision 1 y 1 para Revision 2
- Añadido) Mallas de superficie 3D `EntityNURBSSurface`, `EntityTorus`, `EntityPlane` y `EntityPyramid` y sus métodos de fábrica; creación/edición de mallas y splines 3D y previsualización de secciones en WinForms
- Añadido) Ancho de carácter Fixed y ajuste de glifos en texto Sirius/GDI recto y circular y `EntityImageText`, conservando posiciones para espacios y glifos ausentes
- Modificado) Saltos de línea circulares en líneas concéntricas; líneas base y límites lógicos más estables y cálculo mejorado de márgenes transparentes en `EntityImageText`
- Corregido) Códigos 1D/2D conservan `Width` y `Height` como límites máximos independientes y ajustan su matriz sin exceder el tamaño solicitado
- Corregido) Marcado de puntos de varios pulsos en `EntityPoint` y `EntityPoints` aplica `EntityPen.IsPixelPulsesExit` al comando RTC
- Corregido) La cuadrícula de corrección RTC 2D acepta dx/dy separados por espacios o coma y vuelve a colorear las celdas según la distancia del error

## v1.12.2 (2026.8.18)

- Añadido) Entidad `EntityCircularSiriusText`
- Corregido) AutoKerning de SiriusText calcula la separación a partir de la distancia mínima real entre segmentos, incluidas diagonales

## v1.12.1 (2026.8.16)

- Añadido) `EntitySiriusText.IsAutoKerning` ajusta glifos contiguos automáticamente desde contornos de fuente en caché en modo Variable
- Corregido) El espaciado Fixed conserva posiciones y márgenes de ambos extremos al cambiar el texto, admite muestras métricas multilingües configurables y elige alternativas mediante rangos Unicode 17
- Corregido) Si el destino de `TextConverters.Link` también usa `Link`, el marcado registra un error y se detiene, sin seguir una cadena de enlaces
- Modificado) Gentec-EO conserva la escala y el ajuste de escala automática del dispositivo cuando `scaleIndex` es `null`, en lugar de activar la escala automática
- Corregido) El texto de código de barras vacío o inválido registra un error y elimina la forma anterior Data Matrix, QR, PDF417, Aztec o 1D del editor
- Añadido) `ITransformable` expone inicio/fin de trayectorias en coordenadas de origen, modelo propio y globales acumuladas mediante `OriginalIn/Out`, `ModelIn/Out` y `RealIn/Out`
- Modificado) Diálogos de corrección RTC 2D/3D, errores de validación y botones de mensajes usan recursos Sirius3 en inglés, coreano, chino simplificado, japonés y alemán

## v1.11.14 (2026.8.7)

- Corregido) OpenTK 3 y 4 leen y restauran de forma segura los modos de polígono según el tipo de contexto, evitando fallos de acceso a memoria en Release, desaparición de cajas delimitadoras y una aserción de depuración incorrecta en OpenTK 3
- Corregido) La simulación de trayectorias se detiene con ESC sin demoras de PropertyGrid, abortos repetidos del RTC virtual al limpiar ni falsos errores por un aborto normal del láser virtual
- Corregido) Cancelar la confirmación de inicio F5 desde el editor o el árbol con foco ya no abre el diálogo por segunda vez
- Corregido) Las descripciones de PropertyGrid conservan ajustes relacionados, precauciones y orden de configuración en líneas separadas
- Corregido) En MultiBeam Both con JumpAndShoot, las esperas de liberación del token se solapan con saltos reales; solo los saltos cortos de menor coste se agrupan en lotes pequeños
- Corregido) Atajos del editor y árbol, incluidos CTRL+R, CTRL+M, cámara, simulación y marcador, funcionan de forma fiable y conservan todas las combinaciones de flechas para navegar por el árbol
- Corregido) Zoom y desplazamiento del editor disponibles durante marcado y escritorio remoto, manteniendo bloqueada la selección
- Modificado) Importaciones DXF, DWG, HPGL y PLT comparten `Config.ImportMergeDistance`, distancia configurable para unir extremos
- Modificado) Etiquetas de configuración Core/UI siguen el idioma seleccionado; DXF, DWG y Gerber conservan colores de origen o los asignan a la pluma más cercana
- Añadido) Búsqueda de propiedades por nombre, categoría o descripción, con foco mediante CTRL+F y borrado con un clic

## v1.11.11 (2026.8.5)

- Corregido) RTC6 lee estado e I/O analógicos mediante la API correcta, detecta correctamente errores Ethernet y se cierra sin competir con sus temporizadores de estado
- Corregido) Los trabajos syncAXIS liberan el estado ocupado al terminar e informan de errores de configuración de forma uniforme
- Corregido) Conexión, reconexión y cierre de StreamParser más estables, con limpieza segura de recepciones pendientes
- Corregido) Codificación opcional de códigos de barras; dimensiones de punto solicitadas conservadas y matriz real disponible aparte; cambiar la forma Data Matrix no reduce el tamaño solicitado

## v1.11.10 (2026.8.1)

- Corregido) Los códigos de barras respetan el tamaño solicitado; trayectorias de mecanizado, códigos suplementarios, hatch y simulación de celdas de puntos se alinean correctamente
- Corregido) Seccionado de mallas 3D más rápido y fiable, con avisos claros para mallas dañadas o incompletas
- Corregido) Selección AABB más estable y rápida sin modificar la geometría
- Corregido) Editores de sombreado, ALC, plumas y listas actualizan valores y vistas previas con mayor fiabilidad
- Añadido) Simulación de trayectoria láser con marcadores de tamaño fijo, haz visible y partículas que se desvanecen con bajo coste
- Corregido) Importación vectorial, Gerber y Excellon une trayectorias cercanas, detecta contenido y omite archivos no compatibles de forma segura

## v1.11.0 (2026.7.27)

- Añadido) Objeto EntityStitchedImage añadido
    - Disponible como IView.StitchedImage
- Añadido) Interfaz IEntityCloneable
- Corregido) Algunas entidades no se resaltaban en negrita al seleccionarlas
- refactor) Renderizado OpenGL de entidades separado e indicadores de selección mejorados
    - Llamadas OpenGL trasladadas a la capa de renderizado
- Corregido) Valores de entrada de PropertyGrid
    - Los valores fuera de rango se limitan al mínimo/máximo en lugar de mostrar un aviso

## v1.10.14 (2026.7.10)

- Añadido) TextConverters.Link
    - TextConverter obtiene propiedades de la entidad vinculada por LinkEntity y las convierte en texto.
- Corregido) Mayor estabilidad de Deshacer y Rehacer.
    - Corregida la excepción al rehacer más allá del límite Config.UnReDoSize.
- Corregido) Inicialización OpenGL más estable
    - Bloqueo «No responde» con GPU Intel
- Corregido) EntityBarcode1D_V2
    - «QuiteZone» se interpreta como márgenes izquierdo y derecho
- Añadido) UI.Config.MaxDegreeOfParallelism
    - Límite del número máximo de tareas paralelas (por defecto, el 50% de los procesadores lógicos)

## v1.10.11 (2026.7.1)

- Refactorización) Texto
    - EntityText, EntityImageText, EntityCircularText: interletraje y centrado en modo «Fixed»
    - EntitySiriusText: centrado en «Fixed» y archivos binarios externos de fuentes .fnt
    - EntityImageText: ancho total mediante TargetWidthPixels y anchuras variables o fijas
    - EntityCircularText: anchuras variables y fijas
- Añadido) Config.IsConvertToControllerResolution
    - Determina si los valores de EntityPen y EntityLayerPen (tiempo, frecuencia, etc.) se muestran tras convertirlos a la resolución real de RTC
    - False: predeterminado; muestra lo introducido sin convertir
    - True: convierte los valores a la resolución de control de RTC
- Corregido) EntityImageZPL
    - Fuentes coreanas en la conversión mediante BinaryKits
    - Fuente de conversión configurable mediante Config.ZPLBinaryKitsFonts
- Corregido) Remote
    - Corregida la gestión de varias entradas de datos del comando text
    - Ejemplo 1) text|1|Text_1|ABCD123;
    - Ejemplo 2) text|2|Text_1|ABCD1|Text_2|ABCD2|Text_1|ABCD3|Text_2|ABCD4;
- Corregido) Errores
    - ZoomFit fallaba con objetos de tamaño 0
    - La primera línea usada por TextConverters.File no se eliminaba
    - Excepción al editar propiedades después de Deshacer

## v1.10.10 (2026.6.22)

- Añadido: Ancho total fijo de objetos de texto
    - Entidades: EntityText, EntitySiriusText
    - Propiedad «Target width»
    - Con 0, genera texto al tamaño de fuente óptimo, como antes;
    - con un valor mayor que 0, ajusta la escala automáticamente según `Target width`.
- Añadido) Conversión local de imágenes ZPL.
    - Entidad `EntityImageZPL`.
    - Antes: conversión en línea mediante Labelry.
    - Ahora: conversión sin conexión mediante BinaryKits.
    - Predeterminado: BinaryKits;
    - Servicio seleccionable mediante UI.Config.ZPLService;
- Añadido) Conversión de dimensiones completas;
    - OriginalDimension: tamaño de la entidad original;
    - ModelDimension: tamaño (ancho, alto, profundidad) en coordenadas locales;
    - RealDimension: dimensiones globales tras acumular ModelMatrix de todos los padres
- Añadido) Alineación de sombreado
    - Alignment en HatchLine
    - None: sin alineación
    - Center (predeterminado): centrado
    - Fit: recalcula y uniformiza el intervalo
- Añadido) Formato GS1
    - Conversión de delimitadores &lt;GS&gt; y (,) en GS1
- Corregido) Texturas de imagen en varias vistas
    - Entidades: EntityImage, EntityImageText, EntityImageZPL
    - Corregidas texturas ausentes al usar varias vistas
- Corregido) EntityUniformGroup
    - Restricciones para convertir objetos en grupos uniformes
    - No se permiten objetos de control ni los que contienen ITextConvertible o IHatch
- Corregido) Calibración 2D del escáner
    - Hasta 99x99 puntos en RtcCorrection2D
- Corregido) Calibración RtcCalibrationLibrary
    - Transformación inversa automática de coordenadas medidas al usar MatrixPrimaryInternal
    - Cálculo automático de los datos originales con un escáner girado

## v1.9.0 (2026.6.1)

- Añadido) Importación de G-code
    - Extensiones: archivos .gcode o .ngc
- Corregido) Mejoras de TextConverters.Offset en TextConverter
    - Antes: usaba ExtensionData de Offset como texto convertido
    - Ahora: si ExtensionData de Offset usa «Entity1|Value1|Entity2|Value2;...», TextConverter analiza y utiliza las claves y valores correspondientes
    - Añadido) Remote
        - Comando text añadido
        - Formato: text|count|Name1|Text1|Name2|Text2|...;
- Actualizado) Ophir StarLab v4.00
- Corregido) Creación de entidades externas
    - Consulte la demo editor_entity_custom

## v1.8.6 (2026.5.14)

- Corregido) Hatch
    - Opción HatchFills para sombreado de líneas
    - Mejora: sombreado correcto con celdas Outline de códigos de barras
- Añadido) Tamaño de la cuadrícula de fondo
    - Configurable mediante IView.CheckerSize
- Corregido) Deshacer, Rehacer
    - Deshacer admite atajos en EditorControl
    - Estabilidad mejorada

## v1.8.5 (2026.5.8)

- Añadido) Compatibilidad Undo y Redo
    - ActUndo y ActRedo en IDocument
    - Válido solo para ciertas funciones IDocument.Act
    - Desactivable mediante Config.IsUnReDoEnable
    - Historial configurable mediante Config.UnReDoSize (predeterminado: 30)
- Añadido) Códigos de barras
    - Aztec 2D
    - PLESSEY 1D
    - Tamaño de píxel editable (Dimension)
- Añadido) Formulario CreateGrid
    - Creación de puntos, círculos, cruces y cuadrículas
- Añadido) Rtc6
    - Selección de pulsos para láseres de femtosegundos
- Corregido) IRtcStepper
    - Inicialización y espera asíncronas
- Corregido) Formulario DIO
    - Visualización de valores de salida analógica corregida
- Corregido) MultiBeam
    - Intervalo de salto/intercambio de token durante el ráster modificado
    - Antes: intercambio en el salto ListRasterLine al final de cada línea
    - Ahora: intercambio en ListRasterPixel entre píxeles
- Corregido) Fuente semi ocr
    - Método de mecanizado ráster aplicado

## v1.8.1 (2026.4.22)

- Añadido) Remote
    - Interfaz IRemote para cambiar recetas, consultar/modificar propiedades y enviar inicio, parada, reinicio y desplazamientos de mecanizado mediante comunicación externa
    - Comunicación serie
    - Comunicación TCP/IP
    - Comunicación WebSocket
    - Comunicación MQTT
- Añadido) Script
    - Modificación de texto en tiempo real durante el mecanizado mediante scripts C# externos
    - Integración con TextConverter.SimpleScript
    - Scripts C# propios en la carpeta Script
    - ScriptInstance en IMarker
- Añadido) Fuentes SEMI OCR
    - Archivos de fuentes .dot
    - Fuentes de puntos mediante SiriusText
- Corregido) IDocument
    - Error de búsqueda FindByName corregido
- Corregido) MultiBeamControl
    - Estado de los botones de alternancia corregido

## v1.7.1 (2026.4.16)

- Actualizado) Paquete RTC6 v1.24.0
    - Versión publicada: 31 de marzo de 2026
- Corregido) IMarker
    - Procesamiento asíncrono (cambios en código abierto)
    - Refactorización con tareas en lugar de hilos e implementación por herencia
- Corregido) IRtcMultiBeam
    - Verificado el control de sincronización exclusiva entre RTC
    - Verificación de SiriusEditorControl completada
        - Admite 2 conjuntos de datos y 2 combinaciones de plumas diferentes
    - Verificación de SiriusMultiEditorControl completada
        - 1 conjunto de datos de procesamiento idéntico + 1 combinación de plumas diferente
- Añadido) LogControl
    - Filtrado y búsqueda de mensajes de registro;
- Corregido) Shader;
    - Los objetos no se renderizaban en la vista en un entorno de consola;
    - Gestión de shaders por destino en varias vistas;
- Corregido) Correction 3D
    - Procesamiento de resolución de 16 y 20 bits para los coeficientes A, B y C
    - Mejoras en la manipulación de datos con Correction3DRtcForm
- Corregido) Fuga de memoria
- Corregido) Error al guardar imágenes de la vista mediante Snapshot
- Corregido) Mayor velocidad de ejecución de scripts C#

## v1.6.1 (2026.4.9)

- Añadido) ViewerControl
    - Se ha añadido un control de usuario
    - Renderizado simultáneo de un documento en el visor y el editor
    - Eliminada la restricción de conexión 1:1 entre un Document y una View
    - Creación y modificación de documentos desde código externo
- Corregido) IRtc3D
    - Procedimientos mejorados de calibración 3D basados en RtcCalibrationLibrary
        - 1. Calibración de inclinación del haz: RtcCalibrationLibrary.BeamTiltCalibration
        - 2. Corrección de campo 2D: RtcCalibrationLibrary.XyCalibration
        - 3. Calibración del foco en z=0: RtcCalibrationLibrary.FocusCalibrationAtZ0
        - 4. Calibración del foco para los coeficientes A, B, C: RtcCalibrationLibrary.FocusCalibrationCoeffABC
        - 5. Calibración de estiramiento del volumen Z: RtcCalibrationLibrary.StretchCalibration
    - Eliminado RtcCorrection3D: sustituido por RtcCalibrationLibrary;
    - Eliminado KZScale: sustituido por la compensación de foco de RtcCalibrationLibrary;
    - Eliminado ZOffset: sustituido por la traslación Z de MatrixStack;
- Añadido) Objeto EntityPoint;
- Añadido) Objeto EntityBarcode1D_V2
    - Admite varios tipos de celdas, como los códigos de barras 2D:
    - Permite combinar puntos, líneas, tramas, etc.:
- Añadido) Apertura y guardado de listas de vértices para los siguientes objetos:
    - EntityPoints:
    - EntityPolyline2D:
    - EntityPolyline3D:
    - Control de usuario OffsetControl

## v1.5.4 (2026.4.2)

- Corregido) Corrección urgente
    - Excepción al crear SiriusEditorControl en tiempo de diseño
    - Excepción al crear SiriusMultiEditorControl en tiempo de diseño

## v1.5.3 (2026.3.31)

- Corregido) IDocument
    - Añadida la configuración de conexión de IView
- Añadido) IPowerMeter
    - Añadido MeasureUnits para los modos de medición de potencia y energía
- Corregido) Rtc6
    - Notificación de eventos al cambiar la propiedad IsActivateAutoDelays
    - Los elementos de EntityPen y EntityLayerPen se hacen visibles al cambiar IsActivateAutoDelays
- Corregido) EntityBarcode2D
    - Cambiado el orden de marcado mediante EntityPen al procesar CellDot
- Corregido) IRtcMultiBeam
    - Verificada la gestión del token exclusivo
- Corregido) IRtcCorrection2D, IRtcCorrection3D
    - Procesamiento de datos sin procesar mediante la matriz interna (rotación, etc.) configurada en el cabezal de escaneo

## v1.5.2 (2026.3.27)

- Añadido) Control de motores paso a paso
    - Control de motores paso a paso externos mediante el puerto de motores de RTC5 y RTC6
    - Añadida la interfaz IRtcStepper
    - Añadida la interfaz de usuario StepperControl
    - Movimiento absoluto y relativo de motores paso a paso
- Añadido) Comunicación serie
    - Comunicación serie mediante el puerto RS232 de RTC5 y RTC6
    - Añadida la interfaz IRtcSerialComm
    - Añadida la interfaz de usuario SerialCommControl
    - Monitorización de datos binarios transmitidos y recibidos en la pestaña Laser
    - Añadido el evento OnSerialReceived
- Añadido) Mejoras de Fly Extension
    - Mejoras de la extensión Marking on the fly para RTC6
    - Refactorizada la interfaz IRtcMoFExtension
    - Combinaciones de 3 ejes (X, Y, Z o ejes de rotación)
    - Añadida la comunicación McBSP
- Corregido) Refactorización de controles de usuario
    - OffsetControl
    - MarkerControl
    - ScannerControl
    - LaserControl
- Corregido) Pila de matrices
    - Eliminado BaseMatrix de MatrixStack
    - Soporte integrado mediante IRtc.CtlMatrix y ListMatrix
- Corregido) PowerMeter
    - Error al leer valores de potencia de dispositivos CoherentPowerMax y GentecEO
- Corregido) SiriusEditorControl
    - Importación de archivos .sirius3 externos para añadirlos o fusionarlos como capas del documento actual
    - El árbol muestra si el nodo de capa utiliza el color de la pluma de capa
- Añadido) Pegado en matriz en el control del editor

## v1.4.1 (2026.3.10)

- Añadido) Documentación mediante servidor web
    - Sitio web disponible: https://spirallab.co.kr/sirius3/doc
    - También puede extraer sirius3\doc\sirius3_doc_{version}.zip y ejecutar el archivo por lotes 'start_doc.bat'
- Añadido) Edición de valores con el ratón
    - Permite aumentar o reducir valores arrastrando a izquierda o derecha con el botón derecho pulsado en PropertyGrid
- Corregido) Rtc6
    - Al usar ListLaserOn, la señal SYNC OUT de una fuente láser externa se utiliza para contar pulsos, pero el tiempo de espera se procesaba como 10 veces más largo.
- Corregido) EntityPen
    - Los valores Power, PowerPercentage y PowerMapCategory no eran visibles
- Corregido) SiriusEditorControl
    - Las 4 páginas están disponibles de forma predeterminada
    - WaferMap y Substratemap desactivados

## v1.4.0 (2026.3.3)

- Añadido) Entornos de desarrollo .NET 9.0-Windows y .NET 10.0-Windows
- Añadido) Salida por número de pulsos mediante la señal de sincronización de una fuente láser externa
    - Entrada de sincronización externa mediante DIGITAL IN1 del conector LASER
    - IRtc.ListLaserOn(tiempo de espera, número de pulsos, salida al alcanzar el número de pulsos)
    - Se configura mediante PixelPulses e IsPixelPulsesExit de la pluma EntityPen
        - 0: Emite LASERON durante el tiempo del píxel, como antes
        - 1~65535: Emite LASERON durante el tiempo del píxel mientras espera el número indicado de pulsos de sincronización externos
        - Con IsPixelPulsesExit, finaliza de inmediato cuando el recuento de sincronización externa alcanza PixelPulses y pasa al siguiente comando de lista
- Añadido) Interfaz IRtcMultiBeam (experimental)
    - Sistema multihaz con una fuente láser + dos RTC + dos controladores RF AOM
    - Rtc6MultiBeam
- Añadido) EntityPoints
    - Optimización de la ruta más corta
- Añadido) Interfaz IRtcIO
- Corregido) EntityWaitDataExt16Cond, EntityWaitDataExt16EdgeCond, EntityWriteDataExt16, EntityWriteDataExt16Cond
    - Máscara de bits de tipo ushort en lugar de cadena
- Corregido) Control SiriusEditorControl
    - Excepción al añadirlo en tiempo de diseño
    - Excepción al crear el control desde código debido a que OpenGL no estaba inicializado
    - Eliminado el código que forzaba Document a ActNew al cargar el control
- Corregido) Licencia
    - Al superar el máximo de instancias permitidas o carecer de opciones
    - Antes: No se podía utilizar
    - Ahora: Activa el modo de evaluación de 30 minutos

## v1.3.2 (2026.2.20)

- Corregido) Compatibilidad con Extended Mode en Automatic Laser Control
    - Se pueden combinar las señales Actual Velocity + Encoder + SCANAhead + Inverse Speed Correction + Backward Transformation + SDC + SkyWriting
    - Configuración de combinaciones de Extended Mode en la lista PoD de las propiedades EntityLayerPen
    - Añadido EntityPoD
- Corregido) EntityPen
    - Configuración de la distancia entre puntos de la función SDC en SpotDistanceSCANa.
- Añadido) IRtcMoF
    - Notificación de errores de señal del codificador: evento IRtcMoF.OnEncoderSignalError
    - Notificación al salir del campo de imagen virtual: IRtcMoF.OnOutOfVirtualImageField
    - Configuración del filtro de señales del codificador (solo RTC6)
        - CtlMoFEncoderFilter aplica la media aritmética en condiciones de ruido o admite velocidades superiores a 4MHz
    - Al consultar valores del codificador, se pueden obtener por separado las posiciones absolutas y relativas.
    - Modificado el argumento del evento OnEncoderChanged
- Corregido) IRtcWaitID renombrado a IRtcInterrupt

## v1.3.1 (2026.2.9)

- Añadido) Interfaz IRtcSCANAhead
    - Añadidos a EntityPen parámetros específicos de SCANAhead (Corner, End, Acc Scale)
    - Configuración del límite Position (o Trajectory) Acknowledge Limit (predeterminado: 0.28% del rango total de posición)
    - Con RTC6 + SCANAhead funciona como Trajectory ACK Limit
- Añadido) Interfaz IRtcWaitID

## v1.3.0 (2026.2.5)

- Añadido) EntityPolyline2D y EntityPolyline3D
    - Editor de listas de vértices
- Añadido) Control SiriusMultiEditorControl
    - Procesamiento de un documento con varios dispositivos
- Añadido) EntityLayerPen
    - Interfaz para editar valores de pluma y ofrecer asistencia
- Sustituido) gnuplot por gráficos integrados
- Corregido) Formulario incorrecto de desplazamiento manual del escáner
- Añadido) Programa conversor ODA
    - Uso opcional del conversor ODA al procesar archivos .dwg y .dxf
    - El usuario debe instalar ODA por separado (https://www.opendesign.com/guestfiles/oda_file_converter)
- Licencia) Cambios en la política de licencias
    - La opción 3D pasa a formar parte de las funciones básicas
    - La instancia syncAXIS pasa a ser una función opcional

## v1.2.7 (2026.1.26)

- Añadido) Variable Delays añadido a EntityLayerPen
    - Variable polygon delay: Retardo variable según el ángulo de giro (predeterminado: activado)
    - Variable jump delay: Retardo variable según la distancia del salto
- Corregido) RTC7
    - Valor LaserOnShift incorrecto para Skywriting
- Corregido) Config.IsMarkArcsIntoLines
    - True: Los arcos (EntityArc) y las polilíneas (EntityPolyline2D) se descomponen en líneas (ListMarkTo)
    - False: Los arcos (EntityArc) y las polilíneas (EntityPolyline2D) se procesan mediante ListArcTo
- Corregido) Contour
   - IsClosed se calculaba incorrectamente al extraer contornos
- Corregido) Edición de Config.EntityPenColors y Config.LayerPenColors
- Corregido) Error de ActRemove para entidades simuladas

## v1.2.6 (2026.1.20)

- Añadido) Entidad elipse
- Añadido) EntityLine, EntityArc, EntityPolyline2D
	- Propiedad RampFactor para Automatic laser control (defined vector)
- Añadido) IHatch.HatchRepeats para repetir tramas
- Corregido) Valores incorrectos mostrados en EntityPen y EntityLayerPen
- Corregido) Rutina CtlCompensate de PowerMap
	- Antes: Se volvían a medir los rangos izquierdo y derecho
	- Ahora: Actualiza de inmediato los datos medidos
- Corregido) IMarker.Preview
   - Antes: Mostraba un único rectángulo delimitador para los objetos seleccionados
   - Ahora: Muestra todos los rectángulos delimitadores individuales de los objetos seleccionados

## v1.2.5 (2026.1.15)

- Añadido) ClipHelper para intersecciones
- Añadido) La barra espaciadora activa la selección de subentidades
- Corregido) Prueba de intersección de rayos de IHitTestable
   - Config.RayHitTestPixelSize: Prueba de selección con distancia umbral dinámica
- Corregido) IMarker
   - Marcado recursivo de entidades hijas con MarkTargets.Selected
- Actualizado) zxing v0.16.11
- Actualizado) clipper2 v.2.0.0

## v1.2.4 (2026.1.7)

- Añadido) Atajos de teclado
   - CTRL + R: Activa o desactiva el renderizado
   - CTRL + M: Activa o desactiva el marcado
   - Cambia la fuente o el color del nodo al activar o desactivar el renderizado o el marcado
- Añadido) Evento IRtcFreeVariable.OnFreeVariableChanged
   - Se genera al cambiar el valor de FreeVariable
- Añadido) Config.GridCloudInterval
   - Se utiliza al llamar a IDocument.ActGridCloud
- Corregido) Mayor velocidad de análisis de archivos Gerber
- Corregido) Prueba de selección con información más detallada
   - IDocument.SubHitEntities
- Añadido) Otra función ActHitTest
- Corregido) Excepción al ejecutar ActUngroup con un nodo vacío

## v1.0.1 (2025.12.22)

- Añadido) Archivos de documentación .chm
- Añadido) ActExpand
   - Expande o contrae contornos según una distancia
- Añadido) Medidores de potencia Gentec-EO
- Actualizado) PowerMeterOphir mediante StarLab v3.93
- Corregido) Enumeración de uniones de tramas
- Corregido) IDocument.FindByLayerUsedPenColors
- Corregido) Más mensajes de registro para Marker.EntityWork

## v0.9.3 (2025.12.5)

- Añadido) Ajustar zoom al contenido
   - Doble clic en el árbol
   - Después de abrir un archivo
- Añadido) Nuevo TextConverters.Offset
   - Se utiliza con Offset.ExtensionData
- Corregido) Archivos Gerber
   - Añadido) UI.Config.IsGerberWithUniformGroup para acelerar el renderizado
   - Corregido) UI.Config.IsGerberTessellation para la teselación incorrecta
- Renombrado) Scanner pen a entity pen

## v0.9.2 (2025.11.25)

- Añadido) Conversión a bloque e inserción de bloque en el menú
- Renombrado) EntityGroup a EntityMixedGroup
- Corregido) Error de ActUngroup
- Corregido) Rendimiento de ActMixedGroup y ActUniformGroup
- Corregido) Tiempo de carga al importar archivos Gerber
- Corregido) Desbordamiento de pila al guardar archivos

## v0.9.1 (2025.11.18)

- Añadido) Programa 'gnuplot' incluido en Spirallab.Sirius3.Dependencies
- Añadido) Botón para crear grupos uniformes en el editor
- Corregido) Renderizado incorrecto de EntityUniformGroup
- Corregido) Fugas de memoria
- Corregido) Vértices de spline incorrectos
- Corregido) Memoria insuficiente al crear demasiados nodos
- Modificado) Firmas de Core.Initialize

## v0.8.2 (2025.11.11)

- Corregido) Fallo al analizar HPGL
- Corregido) No se aplicaba la pluma del escáner
- Corregido) Actualización de la pluma del escáner/capa al ejecutar ActNew

## v0.8.0 (2025.11.7)

- Versión preliminar para desarrolladores

## v0.1 (2025.03.06)

- Versión inicial
