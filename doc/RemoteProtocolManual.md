# Sirius3 원격 제어 프로토콜 사용자 매뉴얼 (Remote Control Protocol Manual)


본 문서는 Sirius3 라이브러리의 원격 제어 기능을 통해 외부 장치나 프로그램이 시스템을 제어하기 위한 통신 프로토콜 명세서입니다.

## 1. 기본 통신 설정

- 지원 프로토콜:
  - TCP/IP Server (Default: 50001)
  - RS-232 Serial
  - WebSocket Server (웹 환경 표준 지원)
  - MQTT Client (IoT 및 원격 분산 제어 지원)
  
- 구분자 (Separator): | (파이프) (기본값)
- 종료자 (Terminator): ; (세미콜론) (기본값)
- 기본 응답:
  - 성공 시: OK;
  - 실패 시: NG;
  
- 주의사항: 모든 명령은 종료자(;)로 끝나야 하며, 명령어와 파라미터는 구분자(|)로 구분합니다. 
- WebSocket 사용시 doc\Remote_WebSocket_Demo.html 를 웹브라우저에서 실행해 통신 테스트가 가능합니다.


## 2. 마커 상태 및 제어 모드

- 제어 모드 (Control Mode):
  - Local: PC 사용자가 직접 제어합니다. 원격 제어 명령 중 쓰기/제어 동작 관련 명령은 NG를 반환합니다.
  - Remote: 원격 클라이언트의 제어를 허용합니다. PC 상의 직접적인 조작은 가급적 제한됩니다.


## 3. 자동 알림 기능 (Push Notification)

마커의 상태가 변경될 때 서버에서 클라이언트로 즉시 메시지를 전송합니다 (폴링 불필요).

- 마킹 시작 시: Status|Started;
- 마킹 종료 시: Status|Ended|{Result}|{Elapsed};
  - {Result}: OK 또는 NG
  - {Elapsed}: 소요 시간 (초 단위, 예: 1.234)


## 4. 주요 명령어 목록


## 4.1 마커 제어 (Marker)

- 마킹 시작: marker|start;
  - 응답: OK; (작업이 수락되었음을 의미하며, 가공 종료는 자동 알림으로 확인)
- 마킹 중지: marker|stop;
  - 응답: OK;
- 에러 리셋: marker|reset;
  - 응답: OK; 또는 NG;

## 4.2 제어 모드 조회 (ControlMode)

- 현재 제어 모드 조회: controlmode;
  - 응답: OK; 후 ControlMode|{Mode};
  - {Mode}: Local (로컬 제어) 또는 Remote (원격 제어)
  
## 4.3 상태 조회 (Status)

- 현재 상태 조회: status;
  - 응답: Status|{State};
  - {State}: Error (에러), Busy (동작 중), Ready (준비 완료), NotReady (준비 중)

## 4.4 레시피 제어 (Recipe)

- 레시피 열기: recipe|{FileName or Number};
  - 예: recipe|C:\Recipes\ProductA.sirius3; 또는 recipe|10;
  - 응답: OK; 또는 NG;
  - 번호(Number) 사용시 해당 'recipe\번호' 디렉토리의 'laser.sirius3'파일을 찾아 열기를 시도합니다.
- 현재 레시피 확인: recipe;
  - 응답: OK; 후 Recipe|{FileName};

## 4.5 오프셋 제어 (Offset)

마커에 설정된 오프셋 배열(좌표 및 회전)을 일괄 설정합니다. 

- **오프셋 배열 설정 (Batch Set)**: `offset|{Count}|{X1}|{Y1}|{Z1}|{Angle1}|...|{Xn}|{Yn}|{Zn}|{Angle n};`
  - `{Count}`: 설정할 오프셋의 총 개수
  - `{X}, {Y}, {Z}, {Angle}`: 이동(Translation) 및 회전(Rotate Z) 정보. 이 세트가 `{Count}`만큼 반복됩니다.
  - 응답: `OK;` 또는 `NG;`
- **오프셋 조회**: `offset;`
  - 응답: `OK;` 후 `Offset|{Count}|{X1}|{Y1}|{Z1}|{Angle1}|...;`
  - 현재 설정된 모든 오프셋의 좌표 정보를 반환합니다.
- **주의**: `offset` 명령을 실행하면 마커의 오프셋 배열이 새로 초기화되므로, `text` 명령으로 이전에 설정했던 모든 확장 데이터(ExtensionData)가 리셋됩니다. 오프셋 설정 후 텍스트 데이터를 다시 설정해야 합니다.

## 4.6 동적 텍스트 데이터 제어 (Text)

각 오프셋 인덱스에 할당된 확장 데이터(`ExtensionData`)를 개별적으로 설정하거나 조회합니다. 텍스트 엔티티의 `TextConverter` 속성이 `Offset`으로 설정된 경우, 이 확장 데이터에서 엔티티 이름에 해당하는 값을 자동으로 찾아 치환합니다.

- **주의**: 이 명령은 반드시 **`offset` 명령이 선행**되어 오프셋 정보가 정의된 후에 실행되어야 하며, 오프셋 개수(`Count`)는 동일해야 합니다.
- **텍스트 데이터 일괄 설정**: `text|{Count}|{Text1}|...|{Text n};`
  - `{Count}`: 설정할 오프셋의 개수 (앞서 설정된 `offset` 명령의 `{Count}`와 일치해야 함)
  - `{Text}`: Key-Value 쌍 (`Name|Value`) 또는 단순 문자열. 
    - **주의**: 오직 Key|Value 구분자 형식 지원
  - 응답: `OK;` 또는 `NG;` (오프셋 개수 불일치 시 `NG;` 반환)
- **텍스트 데이터 조회**: `text;`
  - 응답: `OK;` 후 `Text|{Count}|{Text1}|...;`
  - 현재 설정된 모든 오프셋의 확장 데이터를 반환합니다.

**상세 사용 예시**:

1. **오프셋 및 텍스트 데이터 개별 설정**:
   - `offset|1|10|20|0|0;`
   - `text|1|Text_1|Hello;`
2. **오프셋 2개 및 각 텍스트 데이터 일괄 설정**:
   - `offset|2|0|0|0|0|10|10|0|0;`
   - `text|2|ID|001|ID|002;`
3. **텍스트 데이터 조회**:
   - `text;`
   - (응답: `Text|2|ID|001|ID|002;`)

## 4.7 개체 선택 및 해제 (Select/Deselect)

- 개체 이름으로 선택: select|{Count}|{Name1}|{Name2},...;
  - 예: select|1|Circle_1;
  - 응답: OK; 또는 NG;
- 선택 해제: deselect;
  - 응답: OK;
- 선택된 개체 조회: select;
  - 응답: OK; 후 Select|{Count}|{Name1}|...;

## 4.8 레이어/개체/개체펜 속성 제어 (Layer/Entity/EntityPen/LayerPen)

특정 속성의 값을 읽거나 쓸 수 있습니다. 속성 이름은 영문 대소문자를 구분할 수 있습니다.

- 속성 읽기: {target}|{name}|{propertyName};
  - 예: entity|Circle_1|Radius;
  - 응답: OK; 후 Entity|Circle_1|Radius|{Value};
- 속성 쓰기: {target}|{name}|{propertyName}|{Value};
  - 예: entity|Circle_1|Radius|20.0;
  - 예: entitypen|White|Power|50.0;
  - 응답: OK; 또는 NG;
- 지원 속성 리스트 조회: {target}|{name}|properties;
  - 예: entitypen|White|properties;
  - 응답: OK; 보낸 후 줄바꿈 과 함께 {propertyName}|{currentValue}; 가 연속 전송됨

## 4.9 스캐너 필드 보정 (Field Correction)

- 2D 보정 데이터 적용: fieldcorrection|{Rows}|{Cols}|{Interval}|{ErrX1}|{ErrY1}|...;
  - 응답: OK; 또는 NG;
  - 스캐너 보정용 창이 표시되며, 창이 종료될때까지 응답이 되지 않습니다.


## 5. 공통 응답 요약

명령어는 대소문자를 구분하지 않지만 개체의 이름, 값은 대소문자를 구분하여 동작합니다.
명령어에 대한 즉각적인 성공,실패 여부는 OK; 또는 NG; 로 응답하며, 
상태 조회나 오프셋 조회와 같이 데이터가 포함된 응답은 명령에 따른 전용 포맷을 따릅니다.
마킹 작업의 비동기 결과는 3번 항목의 자동 알림을 통해 전달되므로 별도의 폴링 루프를 권장하지 않습니다.


## 6. 개발자 확장 가이드 (Developer Extension Guide)

Sirius3 라이브러리 사용자는 IRemote 인스턴스의 `RegisterCommandHandler` 메서드를 사용하여 
기본 프로토콜을 확장하거나 기존 빌트인 명령의 동작을 재정의(Override)할 수 있습니다.

6.1 명령 핸들러 등록 및 우선순위
- 등록 방식: `remote.RegisterCommandHandler("command_keyword", handler_delegate);`
- 우선순위:
  1. 등록된 커스텀 핸들러 (Registered Handlers) - 가장 먼저 확인하며, 존재할 경우 빌트인 로직을 무시하고 실행됩니다.
  2. 기본 빌트인 명령 (Built-in Commands) - 등록된 핸들러가 없는 경우에만 실행됩니다.
- 키워드 매칭: 대소문자를 구분하지 않습니다.

6.2 코드 예제 (C#)

```csharp
// 예제 1: 완전히 새로운 커스텀 명령 등록 ("MyCmd,Param1,Param2;")
remote.RegisterCommandHandler("MyCmd", async (r, tokens) =>
{
    // tokens[0]: 명령어 키워드 ("MyCmd")
    if (tokens.Length < 3) 
    {
        await r.Send($"{UI.Config.RemoteNG}{UI.Config.RemoteTerminator}");
        return true; 
    }
    string p1 = tokens[1];
    string p2 = tokens[2];
    
    // 사용자 정의 로직 수행...
    Console.WriteLine($"Custom command received: {p1}, {p2}");
    
    await r.Send($"{UI.Config.RemoteOk}{UI.Config.RemoteTerminator}");
    return true; // true 반환 시 명령어가 처리된 것으로 간주됨
});

// 예제 2: 빌트인 명령 오버라이드 (예: 'recipe' 명령 가로채기)
remote.RegisterCommandHandler("recipe", async (r, tokens) =>
{
    if (tokens.Length < 2) return false;
    
    bool success = true;

    // 사용자 로직 
    var doc = Marker.Document;
    // 전달 받은 recipeName 를 이용해 레시피 파일 open 처리
    string recipeName = tokens[1];
    string recipePath  = Path.Combine(Config.RecipePath, recipeName);

    success &= doc.ActOpen(recipePath);
    
    if (success)
    {
      // 처리 성공시
      await r.Send($"{UI.Config.RemoteOk}{UI.Config.RemoteTerminator}");
    }
    else
    {
      // 처리 실패시
      await r.Send($"{UI.Config.RemoteNG}{UI.Config.RemoteTerminator}");
    }

     return true; // 빌트인 로직이 실행되지 않도록 true 반환
});
```

6.3 주요 빌트인 명령 통신 테스트 예시 (Raw Protocol)
- 레시피 변경: recipe|laser.sirius3;
- 레이어 보이기/숨기기: layer|Default|IsVisible|False;
- 개체 위치 변경: entity|Circle_1|Center|10.0|20.0;
- 개체 속성 전체 조회: entity|Circle_1|properties;
- 펜 파워 변경: entitypen|White|Power|50.0;
- 오프셋 1개 설정: offset|1|10|0|0|0; (X축 10mm 이동)
- 오프셋 1개 및 텍스트 데이터 설정: offset|1|0|0|0|0|Text_1|Value_1;
- 특정 오프셋의 텍스트만 변경: offset|0|Text_1|NewValue;
- 개체 복수 선택: select|2|Circle_1|Rect_1; (2개 개체 선택)
- 선택 해제: deselect;


---
2026 Copyright (c) SpiralLAB. All rights reserved.