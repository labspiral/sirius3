# [사용자 메뉴얼] RTC 실시간 계측 및 데이터 분석 (Measurement & Plot)


## 1. 개요

  RTC 컨트롤러(RTC4, 5, 6)는 스캐너의 움직임과 동기화하여 내부 신호(위치, 레이저 상태, 에러 등)를 실시간으로 기록할 수
  있는 내부 계측 버퍼(Internal Measurement Buffer) 기능을 제공합니다. 이 기능은 초정밀 가공 시의 트래킹 에러 분석, 가공
  품질 검증(QA), 디버깅 등에 필수적입니다.


## 2. 계측 채널 (Measurement Channels)

  MeasurementChannels 열거형을 통해 기록할 신호를 선택할 수 있습니다. 
  각 채널은 원시 데이터(Raw)와 가독성 있는 단위(Human-readable)를 가집니다.

   - 주요 채널:
       - LaserOn: 레이저 출력 신호 상태 (0 또는 1)
       - SampleX, SampleY, SampleZ: 스캐너의 지령 좌표 (mm)
       - StatusAX, StatusAY: 스캐너 미러의 실제 피드백 위치 (iDRIVE 사용 시, mm)
       - SampleAX_Corr, SampleAY_Corr: 보정(Correction)이 적용된 출력 좌표
       - MarkSpeed: 현재 가공 속도 (mm/s)
       - Enc0Counter, Enc1Counter: MoF(Marking on the Fly) 사용 시 엔코더 카운트 값
       - ExtAO1, ExtAO2 / ExtDO: 아날로그/디지털 출력 상태

## 3. 리스트 명령 (IRtcMeasurement)

  가공 리스트 내에서 특정 구간의 데이터를 계측하도록 명령합니다.

  ListMeasurementBegin(frequency, channels)
   - 설명: 계측을 시작합니다.
   - 매개변수:
       - frequency: 샘플링 주파수 (Hz). 최대 100kHz (10µs 분해능).
       - channels: 기록할 채널 배열. (RTC4: 최대 2개, RTC5: 4개, RTC6: 8개)

  ListMeasurementEnd()
   - 설명: 계측을 종료합니다.


## 4. 데이터 저장 및 시각화 (RtcMeasurementHelper)

  기록된 데이터를 파일로 저장하거나 그래프로 출력합니다.

  데이터 저장 (Save)
   - RtcMeasurementHelper.Save(fileName, rtcMeasurement)
   - 기록된 데이터를 .txt 파일로 저장합니다. 시간(Time)과 각 채널의 물리적 단위(mm, V 등)로 변환된 값이 기록됩니다.

  그래프 플롯 (Plot)
   - RtcMeasurementHelper.Plot(fileName, plotMode, title)
   - 플롯 모드 (PlotModes):
       1. TimeChart (0): X축은 시간, Y축은 선택한 채널들의 값입니다. 신호의 변화를 시계열로 분석할 때 사용합니다.
       2. PositionChart (1): X축은 SampleX, Y축은 SampleY를 사용하여 레이저 가공 경로를 그립니다. LaserOn 신호를
          참조하여 레이저가 켜진 구간(검은색)과 꺼진 구간(회색 점선)을 구분하여 표시합니다.


## 5. MarkerRtc에서의 자동 처리

  MarkerRtc 클래스는 가공 완료 후 자동으로 계측 데이터를 처리하는 기능을 내장하고 있습니다.

  IsMeasurementPlot 속성
   - 설명: 가공이 끝난 후 계측 데이터를 자동으로 그래프로 출력할지 여부를 결정합니다.
   - 동작 방식 (Plot = true 일 때):
       1. 가공 리스트 내에 EntityMeasurementBegin과 EntityMeasurementEnd가 포함되어 있어야 합니다.
       2. 가공이 완료(NotifyEnded)되면 MarkerRtc 내부의 sessionQueue에 저장된 계측 세션들을 확인합니다.
       3. NotifyPlot() 메서드가 호출되면서 저장된 모든 세션에 대해 자동으로 Plot() 창이 팝업됩니다.


## 6. 예제 코드 (C#)


  기본 리스트 계측 방법

    1 var rtcMeasurement = rtc as IRtcMeasurement;
    2 var channels = new[] { MeasurementChannels.SampleX, MeasurementChannels.SampleY, MeasurementChannels.LaserOn };
    3
    4 rtc.ListBegin();
    5 rtcMeasurement.ListMeasurementBegin(10000, channels); // 10kHz 샘플링 시작
    6
    7 // 가공 엔티티들...
    8 rtc.ListJumpTo(new DVec2(0, 0));
    9 rtc.ListMarkTo(new DVec2(10, 10));
   10
   11 rtcMeasurement.ListMeasurementEnd(); // 계측 종료
   12 rtc.ListEnd();
   13 rtc.ListExecute(true);
   14
   15 // 결과 저장 및 그래프 출력
   16 string filePath = "measurement_result.txt";
   17 RtcMeasurementHelper.Save(filePath, rtcMeasurement);
   18 RtcMeasurementHelper.Plot(filePath, PlotModes.PositionChart, "Laser Path Analysis");

  MarkerRtc 활용 시 (UI 환경)

   1 // MarkerRtc 설정
   2 markerRtc.IsMeasurementPlot = true; // 가공 완료 후 자동 그래프 출력 활성화
   3
   4 // 가공 시작 (내부적으로 LayerWork 수행 후 NotifyPlot 호출됨)
   5 await markerRtc.StartAsync();


## 7. 주의 사항

   - 샘플링 주파수: 너무 높은 주파수(100kHz)로 장시간 계측 시 RTC 내부 버퍼 크기 제한을 초과할 수 있습니다. 가공 시간에 맞춰 적절한 주파수를 설정하십시오.
   - PositionChart 필수 채널: PositionChart 모드를 사용하려면 계측 채널에 반드시 SampleX, SampleY, LaserOn이 포함되어 있어야 합니다.
   - iDRIVE 온도: CtlGetTemperatureStateValues를 통해 실시간으로 스캐너의 갈바노 및 서보 보드 온도를 확인할 수 있습니다. (iDRIVE 스캐너 전용)