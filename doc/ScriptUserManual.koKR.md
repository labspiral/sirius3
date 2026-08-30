# Sirius3 IScript 개발자 매뉴얼 (C# Scripting Integration)

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

본 매뉴얼은 Sirius3의 동적 텍스트 시스템의 핵심인 `IScript` 인터페이스의 기능과 이를 확장하여 현업에 특화된 기능을 구현하는 개발자 가이드를 제공합니다.

---

## 1. 개요 (Introduction)

`IScript`는 레이저 마킹 중 실시간으로 변경되어야 하는 텍스트 데이터를 처리하기 위한 "글로벌 실행 환경"을 정의합니다. `TextConverter`를 `SimpleScript`로 설정하면, 모든 C# 스크립트 표현식은 `IScript` 환경 내부에서 실행됩니다.

- **핵심 역할**: 시리얼 번호 관리, 날짜/시간 포맷팅, 외부 데이터베이스 연동, 생산 이력 로깅.
- **기본 구현**: `SpiralLab.Sirius3.Scripting.Script` 클래스가 모든 내장 기능을 구현하고 있으며, 개발자는 이를 상속받아 기능을 무한히 확장할 수 있습니다.

---

## 2. 주요 속성 및 함수 통합 API (Core API)

`IScript`는 레이저 가공과 컨텍스트 추적에 필요한 모든 내장 속성과 함수를 제공합니다. 스크립트 작성 시 아래의 모든 항목을 즉시 호출할 수 있습니다.

### 2.1 마킹 라이프사이클 이벤트
| 메서드 | 설명 |
|:---|:---|
| `OnStarted(IMarker)` | 마킹 공정이 시작될 때 호출됩니다. 커스텀 스크립트에서 재정의(override)하여 초기화 로직을 넣을 수 있습니다. |
| `OnEnded(IMarker, bool, TimeSpan?)` | 마킹 공정이 종료되었을 때 호출됩니다. 성공 여부와 소요 시간을 전달받아 로그를 남기는 데 유용합니다. |

### 2.2 식별자 및 공정 컨텍스트 (속성)
| 속성 (Property) | 타입 | 설명 | 예제 (입력 및 결과) |
|:---|:---|:---|:---|
| `Marker` | `IMarker` | 현재 실행 중인 마커 인스턴스의 참조입니다. | `Marker.Name` -> `"Rtc5_0"` |
| `LotCode` | `string` | 제품의 LOT 코드입니다. | `$"Lot: {LotCode}"` -> `"Lot: LOT-A123"` |
| `LineName` | `string` | 생산 라인의 명칭입니다. | `LineName` -> `"LINE_01"` |
| `Message` | `string` | 시스템의 공유 메시지입니다. | `Message` -> `"Ready"` |
| `DeviceID` | `string` | 장치 식별자입니다. | `DeviceID` -> `"DEV_842"` |
| `OperatorID` | `string` | 작업자 식별자입니다. | `OperatorID` -> `"OP_KIM"` |
| `PartNo` | `string` | 제품 파트 번호입니다. | `PartNo` -> `"PN-9988"` |
| `CustomerID` | `string` | 고객사 식별자입니다. | `CustomerID` -> `"SAMSUNG"` |
| `EquipmentID` | `string` | 설비 식별자입니다. | `EquipmentID` -> `"EQ-01"` |

### 2.3 시리얼 번호 제어
| 메서드 | 반환 타입 | 설명 | 예제 (입력 및 결과) |
|:---|:---|:---|:---|
| `NextSerialNo(inc)` | `int` | 현재 시리얼을 반환하고 증가시킵니다. | `NextSerialNo()` -> `1` (내부값 2로 변경됨) |
| `NextSerialNo(fmt, inc)` | `string` | 현재 시리얼을 형식화하여 반환하고 증가시킵니다. | `NextSerialNo("D4", 2)` -> `"0001"` (내부값 3으로 변경됨) |
| `NextNamedSerialNo(name, inc, start)` | `int` | 명명된 카운터를 반환/증가시킵니다. | `NextNamedSerialNo("BOX", 1, 100)` -> `100` |
| `NextNamedSerialNo(name, fmt, inc, start)`| `string` | 명명된 카운터를 형식화하여 반환/증가시킵니다. | `NextNamedSerialNo("BOX", "D2")` -> `"01"` |
| `ResetSerialNo(value)` | `void` | 기본 시리얼 번호를 재설정합니다. | `ResetSerialNo(1)` (반환값 없음, 속성값 1로 변경) |
| `ResetNamedSerialNo(name, value)` | `void` | 명명된 카운터를 재설정합니다. | `ResetNamedSerialNo("BOX", 1)` (반환값 없음) |

### 2.4 날짜, 시간 및 교대조
| 메서드/속성 | 반환 타입 | 설명 | 예제 (입력 및 결과) |
|:---|:---|:---|:---|
| `Date(format)` | `string` | 현재 날짜 (기본 `yyyy-MM-dd`) | `Date("yyMMdd")` -> `"260419"` |
| `Time(format)` | `string` | 현재 시간 (기본 `HH:mm:ss`) | `Time("HH:mm")` -> `"15:30"` |
| `AmPm(am, pm)` | `string` | 오전(`am`) 또는 오후(`pm`) | `AmPm("오전", "오후")` -> `"오후"` |
| `DayNight(day, night)`| `string` | 8~20시 기준 주간(`day`), 그 외 야간(`night`) | `DayNight("D", "N")` -> `"D"` |
| `Shift(s1, s2, s3)` | `string` | 8시간 간격(6,14,22시) 3교대 반환 | `Shift("A", "B", "C")` -> `"B"` |
| `Shift2(s1, s2)` | `string` | 12시간 간격(8,20시) 2교대 반환 | `Shift2("Day", "Night")` -> `"Day"` |
| `WeekOfYear()` | `int` | 현재 연중 ISO 주차 반환 | `WeekOfYear()` -> `16` |
| `DayOfYear()` | `int` | 현재 연중 일수 반환 | `DayOfYear()` -> `109` |
| `Year`, `Month` | `int` | 현재 년도, 월(1-12) (속성) | `Month` -> `4` |
| `Day`, `Hour` | `int` | 현재 일(1-31), 시(0-23) (속성) | `Hour` -> `15` |

### 2.5 유틸리티 및 데이터 저장소
| 메서드 | 설명 | 예제 (입력 및 결과) |
|:---|:---|:---|
| `Set(key, value)` | 유지되어야 하는 사용자 정의 데이터를 저장합니다. | `Set("Count", 10)` (반환값 없음) |
| `Get(key)` | `Set`으로 저장한 데이터를 가져옵니다. | `(int)Get("Count")` -> `10` |
| `Pad(input, len, char)`| `input` 앞쪽에 `char`를 채워 총 길이 `len`을 맞춥니다. | `Pad("7", 3, '0')` -> `"007"` |

---

## 3. 커스텀 스크립트 개발 (Custom Implementation)

개발자는 `Script` 클래스를 상속받아 자신만의 비즈니스 로직을 작성할 수 있습니다.

### 3.1 기본 구조
```csharp
using SpiralLab.Sirius3.Scripting;

public class MyProductionScript : Script
{
    // 1. 속성 정의 (Sirius3 UI 속성창에 자동 노출됨)
    public string FactoryName { get; set; } = "Seoul_01";

    // 2. 커스텀 로직 정의 (SourceText에서 호출 가능)
    public string CalculateMagicCode() {
        return FactoryName + "-" + DateTime.Now.Ticks.ToString().Substring(10);
    }
}
```

---

## 4. 공개 데모 (Public Demo)

공개 저장소의 `demos/editor_script`가 기준 예제입니다.

- `Form1.cs`: `EntitySiriusText`를 만들고 `TextConverter = TextConverters.SimpleScript`, `SourceText = "NextSerialNo(1)"`, `IsAllowConvert = true`를 설정합니다.
- `Marker.ScriptInstance`: 현재 Marker가 사용할 `IScript` 인스턴스입니다.
- `ScriptFactory.Create(fileName)`: 공개 `.cs` 또는 빌드된 `.dll`에서 Script를 만듭니다.
- `ScriptSerializer.Open/Save`: `.script` 설정 파일을 열고 저장합니다.

CSV, Database, Network 접근처럼 오래 걸리거나 실패할 수 있는 작업을 SourceText 한 줄에서 직접 반복하지 마십시오. 필요한 데이터는 Marker 시작 전에 준비하고 Script Method는 짧고 결정적으로 동작하도록 구현하십시오.

---

## 5. UI와의 통합 (UI Integration)

커스텀하게 작성된 `.cs` 파일이나 빌드된 `.dll` 파일을 사용하는 방법은 다음과 같습니다.

1. **스크립트 등록**: `Marker.ScriptInstance`에 `IScript` 인스턴스를 할당합니다.
   - 기본 구현은 Marker마다 별도의 ScriptInstance를 사용합니다.
   - 같은 인스턴스를 여러 Marker에 동시에 할당하지 마십시오.
2. **속성창 활용**: 상속받은 클래스에서 정의한 `public` 프로퍼티는 Sirius3 UI의 속성창(Property Grid)에 자동으로 나타나며, 마킹 중 실시간 변경이 가능합니다.
3. **실시간 반영**: 시리얼 번호 증가나 데이터 변경 시 `NotifyPropertyChanged`가 호출되므로 UI는 즉시 갱신됩니다.

---

## 6. 개발자 팁 (Tips)

- **스레드 안전**: 마킹은 고속으로 진행되므로, 파일 쓰기나 네트워크 통신 시 반드시 비동기 처리나 락(Lock)을 고려해야 합니다.
- **컴파일 시점**: 사용자 Script는 Marker가 Busy가 아닐 때 미리 Load/Compile하고 오류를 확인하십시오.
- **수명주기**: `OnStarted`, Text 변환, `OnEnded`가 같은 Marker의 ScriptInstance를 사용합니다.
- **스마트 평가**: `SourceText`에 `{ }`를 사용하면 내부적으로 보간 문자열(`$""`)로 변환되어 매우 직관적인 코드 작성이 가능합니다.

---

## 7. 현업 밀착형 다중 행 스크립트 예제 (Advanced Multi-line Scripts)

현장에서 복잡한 조건을 처리할 때, 속성창의 `SourceText` 입력란(멀티라인 편집기)에 직접 작성하여 즉시 적용해볼 수 있는 실무 스크립트 5가지입니다.

### 7.1 복합 2D 바코드 데이터 조합 (다중 필드)
여러 공정 정보를 조합하여 DataMatrix 또는 QR 코드에 하나의 문자열로 압축해 넣을 때 사용합니다. 세미콜론이 포함된 다중 구문이므로 자동 인식되어 실행됩니다.
```csharp
string prf = LotCode.Substring(0, Math.Min(LotCode.Length, 3)); // 앞 3자리 추출
string dt = Date("yyMMdd");
string tm = Time("HHmm");
string sn = NextSerialNo("D5");
string sh = Shift("A", "B", "C");

return $"{prf}-{dt}-{tm}-{sn}-{sh}";
```

### 7.2 일일 단위(자정 기준) 시리얼 자동 초기화
날짜가 변경될 때마다 시리얼 번호를 자동으로 1번부터 다시 시작하도록 제어합니다. `Set`과 `Get`을 이용해 마지막 마킹 날짜를 기억합니다.
```csharp
string today = Day.ToString();
string lastDay = Get("LastDay") as string;

// 날짜가 바뀌었으면 카운터 리셋
if (today != lastDay) {
    ResetSerialNo(1);
    Set("LastDay", today);
}

return $"SN-{NextSerialNo("D4")}";
```

### 7.3 다중 독립 카운터 (박스 및 개별 제품 연동)
제품 카운터가 특정 수치에 도달하면 제품 카운터는 리셋하고, 박스 카운터를 증가시키는 이중 카운팅 로직입니다.
```csharp
int itemMax = 10; // 박스당 10개 제품
int item = NextSerialNo(); 

// 제품 10개를 다 채웠으면
if (item > itemMax) {
    ResetSerialNo(1);
    item = NextSerialNo();
    NextNamedSerialNo("BoxCounter"); // 박스 카운터 1 증가
}

// "BoxCounter" 카운터값을 가져옴 (증가시키지 않음)
int box = NextNamedSerialNo("BoxCounter", 0);

return $"BOX:{box:D3}-ITEM:{item:D2}";
```

### 7.4 주/야간 교대 및 요일에 따른 조건부 텍스트
단순 날짜가 아닌 시간대와 요일에 따라 마킹되는 식별 문자를 다르게 적용합니다.
```csharp
string dn = DayNight("DAY", "NIGHT");
string dw = DateTime.Now.DayOfWeek.ToString().Substring(0, 3).ToUpper(); // MON, TUE...

// 야간 근무이면서 주말인 경우 특수 마크 추가
if (dn == "NIGHT" && (dw == "SAT" || dw == "SUN")) {
    return $"[W-NGT] {LotCode}-{NextSerialNo("D4")}";
}

return $"[{dw}-{dn}] {LotCode}-{NextSerialNo("D4")}";
```

### 7.5 목표 수량 도달 시 특수 알람 반환
설정된 목표 생산량에 도달했을 때, 작업자에게 시각적인 알람(또는 마킹 중지를 유도하는 텍스트)을 표출합니다.
```csharp
int target = 500; // 목표 수량
int current = (int)(Get("ProduceCount") ?? 0);
current++;
Set("ProduceCount", current);

if (current >= target) {
    // 목표 달성 시 특수 문자열 표출
    return "★★★ TARGET REACHED ★★★";
}

// 달성률을 함께 스크립팅하여 텍스트로 보임
return $"Lot:{LotCode} [{current}/{target}]";
```

---
2026 Copyright (c) SpiralLAB. All rights reserved.
