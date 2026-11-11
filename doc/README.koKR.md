# Sirius3 문서 안내

> API 기준: Sirius3 Core/UI 1.17.1 (공개 기능)

[English](README.md) | [한국어](README.koKR.md)

공개 Core/UI 1.17 API와 편집기 사용 흐름을 설명합니다. Core·UI·Dependencies 패키지에서
호환되는 버전을 사용하십시오. 패키지별 버전은 다를 수 있습니다. 예제·메뉴를 적용할 때
설치한 패키지가 필요한 기능을 지원하는지 확인합니다.

프레임워크 안내서부터 시작해 개체를 생성·선택하고 PropertyGrid에서 편집합니다.
폰트 파일은 **Font Edit**로 편집합니다. Marker 미리보기와 실제 마킹 전 가공 조건을
확인하십시오. 문서의 경로 재생은 화면 표시 기능이며 syncAXIS 시뮬레이션은 컨트롤러
작업을 계획하여 Viewer 검토에 사용합니다.

## Sirius3 1.17.1 추가 기능

다음 기능은 1.17.1 공개 API를 포함한 Core·UI 패키지가 필요합니다. 새 컨트롤이나 예제를 사용하기 전에 설치한 버전을 확인합니다.

1. [syncAXIS 셋업](Rtc6SyncaxisUserManual.koKR.md#syncaxis-setup): WinForms·WPF의 여섯 단계, 편집 가능한 문서 패턴, 모드별 Run·Stop·Reset, 성공한 시뮬레이션의 Viewer 자동 실행과 외부 CT5 선택·재초기화를 제공합니다.
2. [RTC6 Ethernet](Rtc6UserManual.koKR.md): 와치독 모니터링, 별도의 링크 손실 대응, StreamParser 파형 원격 측정을 제공하며 등록 장치의 [MCP 기능](MCP.koKR.md)으로도 사용할 수 있습니다.
3. [클리핑](ClippingUserManual.koKR.md): `IsIncludeOnce`로 공유 경계 데이터를 처음 포함하는 영역에만 배정하며 두 UI 프레임워크에서 옵션을 제공합니다.
4. [오프셋 콜백](OffsetUserManual.koKR.md#offset-callbacks)과 [레이어 펜](LayerPenUserManual.koKR.md): 위치별 처리 계속 여부를 반환하는 핸들러와 syncAXIS 레이저 지연 시작값을 제공합니다.
5. [호스트 연동](WinForms.koKR.md): HiDPI WinForms의 중첩 컨트롤 폰트를 일관되게 처리하고 [WPF](WPF.koKR.md)에서도 셋업 탭을 제공합니다. 현재 G-code·테마 enum 형식은 [UI Config](Sirius3UIConfigUserManual.koKR.md)를 참고합니다.

## Sirius3 1.17.0 주요 변경

1. [편집기](EditorUserManual.koKR.md): 개체 대화형 생성과 Grip 편집, 클릭 가능한
   하단 모드 표시, Arc·Ellipse 중심점 편집을 제공합니다.
2. [Font Editor](FontEditorUserManual.koKR.md): 기존 Sirius 폰트를 편집하거나 설치된
   폰트를 CXF·LFF·DOT로 변환합니다. 변환기의 기본 출력은 CXF입니다.
3. [문서와 개체](DocumentUserManual.koKR.md): PDF 페이지 이미지와 페이지 자동 동기화를
   지원하고 복합 개체의 하위 편집 성능을 개선했습니다. MCP의 기본 파일 접근 범위는
   [MCP 안내서](MCP.koKR.md)를 참고합니다.

## 시작과 프로그램 연동

- [Sirius3 WinForms 통합 가이드](WinForms.koKR.md)

- [Sirius3 WPF 통합 가이드](WPF.koKR.md)

- [SiriusEditorControl User Manual](SiriusEditorUserManual.koKR.md)

- [SiriusMultiEditorControl User Manual](SiriusMultiEditorUserManual.koKR.md)

## 생성·선택·편집

- [EditorControl 사용 설명서](EditorUserManual.koKR.md)

- [IDocument & Recipe Management User Manual](DocumentUserManual.koKR.md)

- [EntityFactory & Entity User Manual](EntityUserManual.koKR.md)

- [TreeView Page Control User Manual](TreeviewUserManual.koKR.md)

- [PropertyGrid 사용 설명서](PropertyGridUserManual.koKR.md)

- [ViewerControl 사용 설명서](ViewerUserManual.koKR.md)

- [화면 표시와 카메라 사용 안내](ViewUserManual.koKR.md)

## 폰트·문자·형상

- [Font Editor 사용 설명서](FontEditorUserManual.koKR.md)

- [텍스트 개체 사용자 매뉴얼](TextUserManual.koKR.md)

- [TextConverters & ITextConvertible Developer Manual](TextConvert.koKR.md)

- [바코드 개체 사용자 매뉴얼](BarcodeUserManual.koKR.md)

- [IHatch 해치 기능 사용자 매뉴얼](HatchUserManual.koKR.md)

- [문서 클리핑 사용자 매뉴얼](ClippingUserManual.koKR.md)

- [Marker Offset 사용자 매뉴얼](OffsetUserManual.koKR.md)

- [Coordinate Transformation & Matrix Stack User Manual](MatrixStackUserManual.koKR.md)

## 가공과 설정

- [EntityPen 사용자 설명서](PenUserManual.koKR.md)

- [EntityLayerPen 사용자 설명서](LayerPenUserManual.koKR.md)

- [RTC Laser Marker Control User Manual](MarkerUserManual.koKR.md)

- [[사용자 메뉴얼] 가공 경로 시뮬레이션 (Simulation)](SimulationUserManual.koKR.md)

- [SpiralLab.Sirius3.Config 설정 가이드](Sirius3ConfigUserManual.koKR.md)

- [SpiralLab.Sirius3.UI.Config 설정 가이드](Sirius3UIConfigUserManual.koKR.md)

- [System Log Monitoring & Logger Events User Manual](LogUserManual.koKR.md)

## 컨트롤러와 장치

- [SCANLAB RTC4 Controller User Manual](Rtc4UserManual.koKR.md)

- [SCANLAB RTC5 Controller User Manual](Rtc5UserManual.koKR.md)

- [SCANLAB RTC6 Controller User Manual](Rtc6UserManual.koKR.md)

- [RTC6 syncAXIS(XL-SCAN) 설정 및 사용 가이드](Rtc6SyncaxisUserManual.koKR.md)

- [SyncAXIS Integrated Marker Control User Manual](MarkerSyncaxisUserManual.koKR.md)

- [ILaser와 사용자 Laser UI 매뉴얼](LaserUsermanual.koKR.md)

- [Laser Power Control & RTC Integration User Manual](LaserVirtualUserManual.koKR.md)

- [디지털 입력·출력과 컨트롤러 트리거 사용자 매뉴얼](DigitalIOUserManual.koKR.md)

- [RTC RS-232 Serial Communication User Manual](SerialCommUserManual.koKR.md)

- [RTC Stepper Motor Control User Manual](StepperMotorUserManual.koKR.md)

- [ULTRA SHORT PULSE LASER with PULSE PICKING AND SYNCHRONIZATION](UltraShotPulseLaserRtc6.koKR.md)

## 교정과 고급 가공

- [RTC Field Correction & 3D Calibration User Manual](CalibrationUserManual.koKR.md)

- [2D Scanner Field Correction User Manual](Correction2dUserManual.koKR.md)

- [Integrated 3D Calibration User Manual](Correction3dUserManual.koKR.md)

- [[사용자 메뉴얼] RTC 실시간 계측 및 데이터 분석 (Measurement & Plot)](MeasurementUserManual.koKR.md)

- [레이저 파워 매핑 및 보정 사용자 매뉴얼](PowermapUserManual.koKR.md)

- [SpiralLab.Sirius3 MoF (Marking on the Fly) 통합 사용자 매뉴얼](MarkingOnTheFlyUserManual.koKR.md)

- [SIRIUS3 Multi-Beam Laser Control System User Manual](MultibeamSystemManual.koKR.md)

- [IRtcMultiBeam Developer Manual](MultibeamDeveloperManual.koKR.md)

## 자동화와 통신

- [Sirius3 MCP Editor 데모: AI 클라이언트 연결 방법](MCP.koKR.md)

- [Sirius3 원격 제어 프로토콜 사용자 매뉴얼 (Remote Control Protocol Manual)](RemoteProtocolManual.koKR.md)

- [Sirius3 IScript 개발자 매뉴얼 (C# Scripting Integration)](ScriptUserManual.koKR.md)

---
2026 Copyright (c) SpiralLAB. All rights reserved.
