# Laser Power Control & RTC Integration User Manual


## 1. 개요 (Overview)

여기에서는 가상 레이저 제어 객체인 LaserVirtual 에 대해 설명하며, 특정 레이저 소스 벤더에 대한 객체 설계는 이를 참고해 상속 구현해야 합니다.
Sirius3의 레이저 제어 시스템은 RTC 제어기의 하드웨어 리소스를 활용하여 다양한 방식의 레이저 소스 출력을 제어합니다. 
특히 '가공 리스트(List)' 명령어와 결합할 경우, 스캐너의 이동과 10µs 단위로 동기화된 초정밀 파워 변조가 가능합니다.

## 2. 파워 제어 방식 (Power Control Methods)

레이저 소스의 인터페이스 사양에 따라 아래 중 하나의 방식을 선택하여 설정합니다.

1) Frequency (주파수 변조)
   - 원리: 입력 파워(W)에 비례하여 레이저 출력 주파수(Hz)를 가변합니다.
   - 대상: 주파수에 따라 에너지가 선형적으로 변하는 CO2 또는 특정 DPSS 레이저.
   - 하드웨어: RTC LASER1/2 포트의 주파수 신호.

2) Duty Cycle (펄스폭 변조)
   - 원리: 주파수는 고정한 채, 펄스의 ON/OFF 비율(Duty %)을 조절하여 평균 출력을 제어합니다.
   - 대상: PWM 제어 방식을 사용하는 대부분의 레이저 소스.
   - 하드웨어: RTC LASER1/2 포트의 Pulse Width 신호.

3) Analog (아날로그 전압)
   - 원리: 0~10V 사이의 전압을 레이저 소스에 인가하여 파워를 제어합니다.
   - 하드웨어: RTC ANALOG OUT 1 또는 2번 포트 (12비트 분해능).

4) Digital Bits (디지털 비트)
   - 원리: 8비트(0~255) 또는 16비트(0~65535) 디지털 값을 직접 전송합니다.
   - 하드웨어: RTC EXTENSION 1(16-bit) 또는 2(8-bit) 포트.

5) RS-232 (시리얼 통신)
   - 원리: RTC 내장 시리얼 포트를 통해 특정 형식의 문자열(예: "P50.0")을 전송합니다.
   - 특징: 리스트 명령(`ListSerialWrite`)을 통해 가공 중 실시간 송신이 가능합니다.

## 3. ListPower와 RTC 제어기 처리 (Real-time Processing)

`ListPower(targetWatt, category)` 함수가 호출되면 소프트웨어는 내부적으로 다음과 같은 과정을 거쳐 RTC 명령을 생성합니다.

Step 1: 파워 보정 (Power Mapping Lookup)
- `category`가 지정된 경우, 해당 카테고리의 파워 매핑 테이블을 조회하여 비선형성이 보정된 `compensatedWatt`를 계산합니다.

Step 2: 백분율 환산 (Percentage Calculation)
- `compensatedWatt / MaxPowerWatt * 100`을 통해 0~100% 사이의 제어 비율을 산출합니다.

Step 3: RTC 하드웨어 명령 매핑
선택된 제어 방식에 따라 RTC 리스트 버퍼에 다음 명령이 기록됩니다.
- Frequency: `rtc.ListFrequency(newFreq, pulseWidth)`
- Duty Cycle: `rtc.ListFrequency(freq, newPulseWidth)`
- Analog: 
  - RTC5: `rtcIO.ListWriteData(ExtAO1/2, voltage)`
  - RTC6: `rtc6.ListLaserPower(ExtAO1/2, voltage)` -> 최적화된 파워 전용 명령 사용
- Digital Bits: `rtcIO.ListWriteData(ExtDO8/16, bits)`
- RS-232: `rtcSerialComm.ListSerialWrite(formattedString)`

Step 4: 안정화 대기 (Settling Delay)
- 파워 변경 직후 `PowerControlDelayTime` 만큼 `rtc.ListWait` 명령이 추가되어, 레이저 소스가 물리적으로 출력을 변경할 시간을 확보합니다.

## 4. 주요 특징 및 장점 (Key Advantages)

- 지연 없는 변경: 가공 리스트 내에서 파워를 변경하므로, PC의 성능과 무관하게 하드웨어 타이밍(10µs)에 맞춰 즉각적인 출력이 반영됩니다.
- 최적화된 통신: 동일한 파워 값이 연속으로 지시될 경우 불필요한 RTC 명령 생성을 자동으로 생략하여 리스트 메모리를 효율적으로 사용합니다.
- 가이드 제어: `CtlGuide` 기능을 통해 가공 전 가이드 레이저(Red Pointer) 신호를 통합 제어할 수 있습니다.

## 5. 주의 사항 (Cautions)

- MaxPowerWatt 설정: 이 값이 실제 레이저 소스의 최대 출력과 일치해야 정확한 W 단위 제어가 가능합니다.
- 하드웨어 포트 확인: Analog나 DigitalBits 사용 시 실제 배선된 포트 번호(`AnalogPortNo`, `DigitalBitsPortNo`)가 올바른지 반드시 확인하십시오.
- 시리얼 통신 제약: RS-232 방식은 다른 방식에 비해 전송 속도가 느리므로, 매우 짧은 선분 가공 시에는 지연이 발생할 수 있습니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
