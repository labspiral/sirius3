# ULTRA SHORT PULSE LASER with PULSE PICKING AND SYNCHRONIZATION

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

본 매뉴얼은 펨토초(Femtosecond) 또는 피코초(Picosecond)와 같은 극초단파(USP, Ultra-short Pulse) 레이저 소스를 제어하기 위한 RTC6의 핵심 기능인 **펄스 피킹(Pulse Picking)** 모드 사용법을 설명합니다.

## 1. 펄스 피킹 레이저 모드 (Pulse Picking Laser Mode) 개요

수십 MHz로 발진하는 펨토초 레이저의 기본 클럭(Seeder) 중 가공에 필요한 펄스만 추출(Picking)하기 위해 반드시 사용해야 하는 모드입니다. 
RTC6 보드 내부의 분주기(Divider)를 사용하여 원하는 반복률(Repetition Rate)로 펄스를 선택적으로 출력할 수 있습니다.

### 동작 원리 및 신호 출력
*   **LASERON 신호:** 레이저 활성 상태(Laser Active) 시 ON 됩니다.
*   **LASER1 포트:** 레이저 소스에서 들어오는 기본 클럭(Base Clock)과 동기화된 변조 신호가 출력됩니다.
*   **LASER2 포트:** 설정된 분주비(Divider)에 따라 LASER1 신호 중 N번째 펄스마다 추출된 펄스가 출력됩니다.
*   **대기 상태 (Standby):** LASERON이 OFF된 대기 상태에서는 LASER1과 LASER2 모두 동일한 위상의 대기 펄스(Standby Pulse)를 출력하며, 이때는 펄스 피킹이 적용되지 않습니다.

---

## 2. 주요 기능 및 API 사용법

### 2.1 펄스 피킹 활성화 (`CtlPulsePickingMode`)
설정된 분주비 `N`에 따라 특정 펄스를 추출하도록 설정합니다.

```csharp
// N=2 설정 시: LASER1 펄스 2번당 LASER2로 1번의 펄스 출력 (50% 분주)
// N=10 설정 시: LASER1 펄스 10번당 LASER2로 1번의 펄스 출력 (10% 분주)
uint divider = 10; 
rtc.CtlPulsePickingMode(divider);
```

*   **매개변수 (`no`):**
    *   `0`: 펄스 피킹 해제. LASER2 포트를 통해 LASERON 신호가 그대로 출력됩니다.
    *   `1 ~ 63`: 펄스 피킹 분주비 설정. LASER1의 매 N번째 펄스마다 LASER2로 출력됩니다.
*   **특이사항:** 이 기능이 활성화되면 기존 `CtlLaserMode`를 통해 설정된 레이저 모드(0~6)는 무시됩니다.

### 2.2 고정된 펄스 폭 설정 (`CtlPulsePickingConstantLength`)
피킹된 펄스(LASER2 포트)의 길이를 LASER1 설정과 무관하게 독립적으로 일정하게 고정시키는 기능입니다.

```csharp
bool enable = true;
double pulseWidth = 0.5; // 0.5 usec (500ns)
rtc.CtlPulsePickingConstantLength(enable, pulseWidth);
```

*   **동작:** 기본적으로 LASER2의 펄스 폭은 LASER1의 펄스 폭을 따르지만, 이 기능을 켜면 실제 피킹되어 나가는 펄스의 폭만 따로 지정할 수 있습니다.
*   **활용:** USP 레이저의 트리거 신호 요구 조건에 맞춰 정밀한 펄스 폭 제어가 필요할 때 사용합니다.

### 2.3 LASER1 출력 펄스 동기화 (`CtlLASER1Synchronization`)
RTC 보드 내부에서 생성되는 LASER1 신호의 출력 타이밍을 외부 레이저 클럭 신호(DIGITAL IN1)에 맞춰 정밀하게 동기화하는 기능입니다.

```csharp
bool enable = true;
double delayTime = 0.1; // 0.1 usec (100ns)
rtc.CtlLASER1Synchronization(enable, delayTime);
```

*   **동작:** RTC6 보드는 LASER1 펄스를 즉시 출력하지 않고, LASER 커넥터의 **DIGITAL IN1** 핀으로 새로운 외부 클럭 펄스가 감지될 때까지 대기(지연)한 후 출력합니다.
*   **설정:** 
    *   `delayTime`: 외부 신호 감지 후 실제 펄스 출력까지의 추가 지연 시간입니다. 레이저 출력 주기보다 짧아야 합니다.
    *   **엣지 설정:** 외부 클럭의 상승/하강 엣지 판별은 `Rtc6LaserControlSignal.Bit.ExtSignalPulseRisingEdge` 설정을 통해 변경 가능합니다.
*   **주의사항:** 
    *   `Rtc6LaserControlSignal.Bit.OutputSynchronization` (스캐너 모션 동기화) 기능과 동시에 사용할 수 없습니다.
    *   외부 레이저 소스의 SYNC OUT 신호를 RTC6 LASER 커넥터의 DIGITAL IN1 핀에 연결해야 합니다.


---

## 3. 신호 연결 및 인터페이스 (Wiring)

USP 레이저 소스와 RTC6 보드 간의 일반적인 인터페이스 연결 구성은 다음과 같습니다.

| RTC6 포트 | 레이저 소스 입력/출력 | 설명 |
| :--- | :--- | :--- |
| **LASER1** | **Sync / Clock** | 레이저 소스의 내부 클럭과 동기화된 기본 트리거 신호 |
| **LASER2** | **Trigger / Picked** | 실제 피킹된 가공용 펄스 트리거 신호 |
| **LASERON** | **Gate / Enable** | 가공 시점에 레이저 출력을 허용하는 게이트 신호 |

---

## 4. 프로그래밍 예제

다음은 RTC6 제어 객체를 초기화하고 USP 모드를 설정하는 전체 예제 코드입니다.

```csharp
// 1. RTC6 스캐너 객체 생성 (기본 레이저 모드 설정)
var rtc = ScannerFactory.CreateRtc6(0, kFactor, LaserModes.Mode4, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, "correction.ct5");

// 2. USP 펄스 피킹 설정 (N=4 분주)
rtc.CtlPulsePickingMode(4);

// 3. (선택 사항) 피킹된 펄스의 폭을 200ns로 고정
rtc.CtlPulsePickingConstantLength(true, 0.2);

// 4. 가공 파라미터 설정 (주파수, 지연 시간 등)
rtc.CtlFrequency(100000, 0.1); // 100KHz, 0.1us pulse width
rtc.CtlDelay(100, 200, 100, 100, 100);

// 5. 마킹 수행
rtc.ListBegin()
rtc.ListJumpTo ...
rtc.ListMarkTo...
rtc.ListEnd()
rtc.ListExecute()
```

---

## 5. 주의 사항 및 팁

1.  **레이저 모드 복구:** 펄스 피킹 모드를 해제하고 일반 모드로 돌아가려면 'CtlLaserMode(LaserModes.Mode4)' 와 같이 표준 레이저 모드 설정 API를 다시 호출하면 됩니다.
2.  **Q-Switch Delay:** 펄스 피킹 모드에서도 'CtlQSwitchDelay'를 통한 Q-Switch 지연 설정이 유효하게 적용됩니다.
3.  **FPK (First Pulse Killer):** 펄스 피킹 모드 사용 시 'CtlFirstPulseKiller'를 통한 FPK 신호는 출력되지 않으므로 주의하십시오.
4.  **Standby Pulse:** 대기 상태에서의 'CtlStandBy'를 통한 펄스 출력 여부 및 폭은 레이저 소스의 안전 규정에 맞춰 적절히 설정해야 합니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
