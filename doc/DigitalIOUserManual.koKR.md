# 디지털 입력·출력과 컨트롤러 트리거 사용자 매뉴얼

> 기준 버전: Sirius3 1.16.0 (공개 Release 기능)

[English](DigitalIOUserManual.md) | [한국어](DigitalIOUserManual.koKR.md)

## 1. 인터페이스와 구현 객체

IDInput·IDOutput은 디지털 I/O 계약입니다. DInputBase·DOutputBase는 채널 버퍼, 이름과 비트 접근을 제공하며 구체 RTC 어댑터가 연결한 Scanner의 포트를 사용합니다. 어댑터가 Scanner를 소유하지는 않습니다.

| 구현 객체 | 포트·사용 비트 |
|---|---|
| RtcDInputExtension1Port | EXTENSION 1 입력 16비트 |
| RtcDInputLaserPort | LASER 입력 2비트 |
| RtcDOutputExtension1Port | EXTENSION 1 출력 16비트 |
| RtcDOutputExtension2Port | EXTENSION 2 출력 8비트 |
| RtcDOutputLaserPort | LASER 출력 2비트 |
| DInputVirtual / DOutputVirtual | 물리 포트 없는 소프트웨어 I/O |

IOFactory의 CreateInputExtension1Port, CreateInputLaserPort, CreateOutputExtension1Port, CreateOutputExtension2Port, CreateOutputLaserPort, CreateVirtualInput, CreateVirtualOutput을 사용합니다. Scanner를 먼저 초기화하고 어댑터를 생성·초기화한 뒤 같은 인스턴스를 Editor에 등록합니다. 내장 어댑터는 채널 0을 사용하며 물리 포트 폭에 맞는 비트 번호인지 확인합니다.

## 2. 버퍼, 폴링과 출력 반영

| 작업 | 동작 |
|---|---|
| 입력 Initialize | 초기 입력값을 캡처합니다. 이미 HIGH인 비트를 새 Edge로 알리지 않습니다. |
| 입력 Update | 하드웨어를 읽고 버퍼를 갱신한 뒤 이전·현재 상태를 비교합니다. |
| 입력 IsAOn/IsAOff, GetChannel | 버퍼를 조회하며 그 자체로 하드웨어를 샘플링하지 않습니다. B 계열은 반대 논리입니다. |
| 출력 OutOn/OutOff, SetChannel | 출력 버퍼를 변경하고 쓰기 필요 상태로 설정합니다. |
| 출력 Update | 변경된 버퍼를 RTC에 쓰거나 지원하는 출력 읽기를 시도합니다. |
| 출력 IsOutOn/IsOutOff, GetChannel | 버퍼·읽기 결과를 조회하며 외부 장치의 수신을 증명하지 않습니다. |

버퍼 변경 성공과 하드웨어 제출 성공을 구분하여 Update 반환값·로그를 확인합니다. 출력 전체 word 쓰기는 같은 포트의 RTC 목록 명령과 충돌할 수 있으므로 출력 소유자를 정하고 겹치는 쓰기를 직렬화합니다.

## 3. ChannelNames로 이름 변경

ChannelNames는 string[][]이며 ChannelNames[채널][비트]로 접근합니다. 표시 이름이며 배선·비트 번호를 바꾸지 않습니다. Name은 장치·포트 전체 이름입니다. 각 행 길이를 실제 비트 수에 맞춥니다.

```csharp
using SpiralLab.Sirius3.IO;

if (input.ChannelNames != null && input.ChannelNames.Length > 0 &&
    input.ChannelNames[0] != null && input.ChannelNames[0].Length >= 2)
{
    var labels = (string[])input.ChannelNames[0].Clone();
    labels[0] = "Start sensor";
    labels[1] = "Part present";
    input.ChannelNames[0] = labels;
}
if (output.ChannelNames != null && output.ChannelNames.Length > 0 &&
    output.ChannelNames[0] != null && output.ChannelNames[0].Length > 0)
{
    var labels = (string[])output.ChannelNames[0].Clone();
    labels[0] = "Acknowledgement";
    output.ChannelNames[0] = labels;
}
```

Editor에 연결하기 전에 이름을 적용합니다. 이미 표시된 패널은 UpdateExtension1PortNames·UpdateLaserPortNames에 해당 16비트·2비트 이름 행을 전달하여 갱신합니다. 출력 패널에는 Extension 2 이름 갱신 경로도 있습니다. 배열 변경만으로 기존 Label 전체에 자동 알림을 보내지는 않습니다.

제어 엔티티의 BitName은 UI의 별도 이름 매핑을 사용하며 실제 BitPosition·마스크가 기준입니다. 한 어댑터의 ChannelNames를 바꿔도 대기 엔티티의 비트 선택은 바뀌지 않습니다. 호스트 설정에 이름을 저장하고 장치 생성 시 다시 적용합니다. [editor_dio](../demos/editor_dio/Form1.cs)를 참고합니다.

## 4. 비실시간 DInput.OnChanged

이벤트 형식은 Action<IDInput, int, SignalEdges>입니다. 전송자는 입력 객체이며 비트 번호는 기반 구현에서 1000 × channel + bit로 표현합니다. 내장 단일 채널 포트에서는 채널 0의 비트 번호입니다. High는 관측한 LOW→HIGH, Low는 HIGH→LOW 변경입니다.

하드웨어 어댑터는 이전 버퍼와 새 Update 결과를 비교합니다. 자체 데이터 잠금을 해제한 뒤 Update 호출 스레드에서 동기적으로 이벤트를 실행합니다. 하드웨어 인터럽트, 타임스탬프 펄스 기록이나 실시간 트리거가 아닙니다. 두 샘플 사이에 HIGH가 됐다가 LOW로 돌아온 짧은 펄스는 놓칠 수 있습니다. UI 스케줄링, 드라이버 읽기와 느린 Handler가 지연을 더하며 타이머 간격을 줄여도 확정적인 마감 시간을 보장하지 않습니다.

통합 WinForms 패널은 일반적으로 100 ms, 독립 WinForms Control은 초기 50 ms 간격을 요청합니다. WPF는 기본 100 ms 간격과 worker task의 장치 읽기를 사용합니다. 표시·Loaded 상태와 호스트 설정에 따라 폴링이 달라집니다. 입력 객체 자체가 보장된 주기 sampler를 생성하지 않습니다.

Handler를 짧게 유지하고 UI 변경은 소유 Dispatcher로 전달하며 종료 시 구독을 해제합니다. 이벤트 전달 시간을 실제 Edge 발생 시간으로 해석하거나 정밀한 Laser·Scanner 동기화에 PC Handler를 사용하지 않습니다. DInputTriggerA/B의 소프트웨어 필터도 호스트 샘플링·시간을 따릅니다.

```csharp
using (var simulatedInput = IOFactory.CreateVirtualInput(0))
{
    simulatedInput.Initialize();
    System.Action<IDInput, int, SignalEdges> handler = (port, bit, edge) =>
        System.Diagnostics.Trace.WriteLine($"{port.Name}: bit {bit}, {edge}");
    simulatedInput.OnChanged += handler;
    try
    {
        simulatedInput.OnByForcily(0);
        simulatedInput.OffByForcily(0);
    }
    finally { simulatedInput.OnChanged -= handler; }
}
```

Virtual 강제 변경 메서드는 실제 값이 바뀌면 즉시 알립니다. 이벤트 연결 확인용이며 물리 입력 폴링과 동작이 다릅니다.

## 5. 컨트롤러 목록 조건과 Edge 대기

IRtcConditionalIO는 준비 중인 RTC 목록에 조건을 기록합니다. 실제 목록 실행 시 컨트롤러가 판정하므로 Windows 이벤트 루프의 지연을 가공 타이밍에서 분리합니다.

| 엔티티 | 역할 |
|---|---|
| EntityWaitDataExt16Cond | LOW·HIGH 마스크의 모든 조건을 함께 판정하는 Level 대기입니다. |
| EntityWaitDataExt16EdgeCond | EXTENSION 1 비트의 LOW→HIGH·HIGH→LOW 대기에 권장합니다. |
| EntityWriteDataExt16Cond | 조건부 EXTENSION 1 출력이며 입력 변경을 기다리지 않습니다. |

Edge 대기는 ListReadBitExtDI16EdgeWaitUntil을 사용합니다. Rising은 LOW를 먼저 기다린 뒤 HIGH를, Falling은 HIGH 뒤 LOW를 기다립니다. 시작할 때 이미 마지막 Level이면 새로운 반대 Level→목표 Level 순서를 기다립니다. SignalEdges.High는 Rising, Low는 Falling입니다.

이 대기는 목록 실행 중의 입력 Level을 관측합니다. 펄스는 해당 명령에서 취득·관측할 만큼 유지되어야 하며 준비 전에 발생한 임의 펄스를 저장하지 않습니다. 검증되지 않은 짧은 펄스보다 요청을 ACK까지 유지하는 PLC handshake를 사용합니다.

Level 대기는 RTC4·4e의 조건부 절대 점프, RTC5·6의 상대 점프를 사용합니다. 현재 RTC4·4e의 전용 Edge 메서드는 성공을 반환하지만 대기 명령을 기록하지 않습니다. RTC4에서는 아래의 두 Level 대기 조합을 사용하고, 전용 Edge 예제는 RTC5·6에서 적용합니다. Virtual은 물리 신호를 기다리지 않습니다. 실제 컨트롤러, 설치 SDK와 capability를 확인합니다. syncAXIS에는 이 RTC 제어 엔티티를 사용하지 않습니다. 미지원 Scanner에서 엔티티를 생략하고 성공을 반환할 수 있으므로 Boolean만으로 실제 대기를 확인하지 않습니다.

## 6. Edge 대기 후 ACK 출력

다음 코드는 RTC5·6용 레시피 조각을 생성하기만 합니다. 장치 초기화·문서 추가·가공 시작을 하지 않습니다. DI0가 요청이며 DO0가 ACK입니다.

```csharp
using SpiralLab.Sirius3.Entity;

IEntity[] triggerSequence =
{
    EntityFactory.CreateWaitDataExt16EdgeCond(0, SignalEdges.High),
    EntityFactory.CreateWriteDataExt16Cond(0, 0, 0x0001, SignalEdges.High)
};
```

입력 마스크 0은 해당 쓰기 위치에 도달한 뒤 무조건 출력하도록 지정하며 Edge 대기가 순서를 정합니다. 실제 레이어 진행 순서에서 가공 개체 앞에 배치합니다. 가공 후에는 응용 프로그램의 검증된 ACK 해제·handshake 순서를 추가합니다. 연결하지 않은 조각은 해제하거나 일반 문서 작업으로 소유권을 이전합니다.

단일 비트 Rising의 동등한 순서는 CreateWaitDataExt16Cond(0x0001, 0) 다음 CreateWaitDataExt16Cond(0, 0x0001)입니다. Falling은 반대로 구성합니다. 전용 Edge 엔티티가 이 의도를 직접 표현합니다.

이 계약에는 자동 Timeout이 없습니다. 신호가 오지 않을 때 호스트·컨트롤러 중지 경로를 유지해야 합니다. Laser 출력이 비활성인 명확한 위치에 대기를 배치합니다. 목록 제출은 완료와 다릅니다.

## 7. 16비트 마스크와 이번 소스 수정

LOW/HIGH는 입력 신호 상태입니다. uint의 하위·상위 word를 뜻하지 않습니다. RTC6 매뉴얼의 “하위 16비트만 판정”은 각 인수의 비트 0–15를 판정한다는 의미입니다.

| 호출 경로 | 인자 순서 |
|---|---|
| CreateWriteDataExt16Cond / CreateWaitDataExt16Cond | LOW, HIGH, 출력(쓰기에서만 사용) |
| IRtcConditionalIO.ListWriteMaskExtDO16If / ListReadMaskExtDI16WaitUntil | HIGH, LOW, 출력(쓰기에서만 사용) |
| SDK set_io_cond_list / clear_io_cond_list | Mask1=HIGH, Mask0=LOW, 출력 마스크 |

예를 들어 DI0=HIGH, DI1~3=LOW일 때 DO4를 HIGH로 설정합니다.

```csharp
var entity = EntityFactory.CreateWriteDataExt16Cond(
    0x000E, 0x0001, 0x0010, SignalEdges.High);
```

Entity.Mark는 HIGH=0x0001, LOW=0x000E를 RTC 인터페이스에 전달하고 SDK도 같은 HIGH, LOW 순서로 받습니다. 조건은 `(input & high) == high && ((~input) & low) == low`입니다. 입력 마스크 둘 다 0이면 무조건 쓰며 출력 마스크의 0 비트는 유지합니다.

이전 소스는 입력을 0x00FF까지만 허용하고 조건부 출력의 HIGH/LOW를 SDK에 반대로 전달했습니다. 이번 소스 수정은 RTC4·4e·5·6·Virtual에서 0x0000–0xFFFF를 허용하며 범위 초과 또는 같은 비트의 HIGH/LOW 중복 요구를 Warning 로그와 false로 거부합니다. 출력 마스크도 16비트로 검증합니다. Mask Level 대기는 모든 요구 조건이 같은 입력 샘플에서 맞을 때만 진행하도록 수정했습니다.

이 설명은 현재 소스 수정입니다. 이미 배포된 NuGet 1.16.0 DLL·캐시가 자동으로 교체된다는 뜻이 아닙니다. 실제 사용하는 빌드의 반영 여부를 확인합니다. 이전 구현을 마스크 반전으로 보상한 응용 프로그램은 수정 빌드로 전환할 때 그 보상을 제거합니다.

RTC6 Software Package 1.25.1 헤더와 [RTC6 Manual](SCANLAB/RTC6_Manual.en.pdf) Doc. Rev. 1.1.4 en-US의 §9.3.2, list_jump_rel_cond, set_io_cond_list(335, 576, 790쪽)를 기준으로 대조했습니다.

## 8. 소유권과 관련 흐름

Editor에 동일한 I/O 인스턴스를 등록하고 그 수명 동안 Scanner를 유지합니다. await editor.DisposeDevices로 등록한 Remote·MCP·장치 정리를 끝낸 뒤 Control·Document를 해제하고 Core.Cleanup을 호출합니다. 독립 호스트는 폴링 작업, 이벤트와 장치 수명을 소유합니다.

정밀한 입력 기반 가공은 전체 목록을 준비하고 컨트롤러 대기·조건으로 다음 명령을 진행시킵니다. 별도 외부 하드웨어 Start는 컨트롤러의 External Start 설정을 사용합니다. DInput.OnChanged에서 소프트웨어 Start를 호출하는 방법은 Windows 지연이 있는 호스트 트리거입니다.

관련 문서: [Marker](MarkerUserManual.koKR.md), [MoF](MarkingOnTheFlyUserManual.koKR.md), [엔티티](EntityUserManual.koKR.md), [WinForms](WinForms.koKR.md), [WPF](WPF.koKR.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
