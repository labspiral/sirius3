---
# SIRIUS 3 Multi-Beam Laser Control System : User Manual


## 1. 시스템 개요 (Introduction)

본 시스템은 하나의 레이저 소스를 두 개의 스캔 헤드(HEAD 1, HEAD 2)가 공유하여 
가공 효율을 극대화하도록 설계된 멀티빔 레이저 제어 시스템입니다. 
두 개의 RTC 제어 보드와 두 개의 AOM(Acousto-Optic Modulator)을 사용하며, 
토큰(Token) 교환 방식의 비대칭 우선순위 뮤텍스(Asynchronous Priority Mutex) 
로직을 통해 실시간 레이저 독점 가공 권한을 제어합니다.


## 2. 레이저 광학 경로 설정 (Optical Path Configuration)

레이저 빔은 아래와 같은 경로를 거치며 AOM의 상태(ON/OFF)에 따라 분기됩니다.
```
[LASER] -> [AOM1] -->  0ORDER  --> [AOM2] --> 0ORDER --> [DUMP]
[SOURCE]     .                        .
                .                 . 
                   .           .    
           1st ORDER  .     .  1st ORDER   
                         .   
                      .     . 
                   .           . 
                .                 .   
             .                       .
            v                         v
        [SCAN ]                    [SCAN ]
        [HEAD1]                    [HEAD2]
```

■ SCAN HEAD 2 가공 경로:
  - AOM 1 (Card 1) = ON (1st-order 회절광 생성)
  - 결과: 빔이 AOM 1에서 꺾여 HEAD 2로 즉시 전달됩니다.

■ SCAN HEAD 1 가공 경로:
  - AOM 1 (Card 1) = OFF (0-order 직진광 통과)
  - AOM 2 (Card 0) = ON (1st-order 회절광 생성)
  - 결과: 빔이 AOM 1을 통과하여 AOM 2에서 꺾인 후 HEAD 1로 전달됩니다.

■ BEAM DUMP (안전 상태):
  - AOM 1 = OFF (직진)
  - AOM 2 = OFF (직진)
  - 결과: 레이저 에너지가 모든 AOM을 통과하여 최종 흡수 장치(DUMP)로 소멸됩니다.


## 3. 하드웨어 연결 및 배선 (Hardware Wiring)

두 RTC 보드 간의 실시간 동기화를 위해 EXTENSION 16 DIO 포트를 다음과 같이 연결합니다.
(참고) 아래 사용된 핀맵은 TokenBitMask, AOMBitMask,AOMChannel 값으로 변경이 가능합니다.


[RTC Card 0 (Role 0)]                 [RTC Card 1 (Role 1)]
---------------------                 ---------------------
DO0 (Token OUT)      -----(직결)---->  DI0 (Token IN)
DI0 (Token IN)       <----(직결)-----  DO0 (Token OUT)
DO1 (AOM 2 ON/OFF)                    DO1 (AOM 1 ON/OFF)
A.OUT1 (AOM 2 Volt)                   A.OUT1 (AOM 1 Volt)

* 중요: Card 0은 가공 경로의 두 번째 AOM(AOM 2)을, 
       Card 1은 첫 번째 AOM(AOM 1)을 제어하는 교차(Cross) 제어 구조입니다.


## 4. 가공 모드 및 처리 방법 (Processing Modes)

사용자는 필요에 따라 소프트웨어에서 다음 4가지 모드를 선택할 수 있습니다.

1) None (표준 모드):
   - 멀티빔 로직을 사용하지 않는 대기 상태입니다.
   - 모든 AOM이 OFF되어 빔은 DUMP로 흐릅니다.

2) Head 1 전용 모드:
   - 가공 권한이 HEAD 1(Card 0)에 고정됩니다.
   - Card 1의 AOM 1은 항상 OFF 상태를 유지하여 광로를 열어줍니다.

3) Head 2 전용 모드:
   - 가공 권한이 HEAD 2(Card 1)에 고정됩니다.
   - Card 1이 직접 AOM 1을 제어하여 가공합니다.

4) Both (배타적 상호 가공 모드):
   - 두 헤드가 동시에 가공 데이터를 처리하되, 레이저 소스는 한 번에 하나씩만 사용합니다.
   - 토큰 신호(Token Signal)를 통해 매우 고속드로 가공 권한(Token)을 주고받습니다.


## 5. 배타적 가공 순서 (Exclusive Processing Sequence)

Both 모드에서 시스템은 내부적으로 다음과 같은 순서로 가공을 수행합니다.

Step 1. [광로 차단]: 자신의 가공 세그먼트가 끝나면 즉시 자신의 AOM을 끕니다.
Step 2. [권한 반납]: 자신의 토큰 신호(DO)를 LOW로 만들어 상대방에게 사용 가능 상태임을 알립니다.
Step 3. [스캐너 이동]: 토큰이 없는 동안 스캐너는 다음 가공 위치로 미리 이동합니다.
Step 4. [권한 획득]: 상대방의 토큰 신호가 해제됨을 감지(DI 가 LOW)하면 자신의 DO를 HIGH로 
        설정하여 레이저 독점권(Token)을 가져옵니다.
Step 5. [광로 개방]: 토큰을 얻은 상태에서만 자신의 AOM을 켜고 실제 가공을 시작합니다.


## 6. 주의사항 및 안전 가이드 (Precautions)

- [배선 확인]: 보드 간 DIO 배선이 엇갈리거나 단선될 경우 시스템 데드락(Deadlock) 
  현상이 발생하여 가공이 멈출 수 있습니다.
- 최초 프로그램 초기화 이후 CheckPins 함수를 이용해 각 카드간 DIO 연결상태를 확인해야 합니다.
- [AOM 전압]: AOM 1과 AOM 2의 회절 효율이 다를 수 있으므로, 각 카드별로 아날로그 
  출력 전압을 미세 조정하여 두 헤드의 가공 에너지를 균일하게 맞추십시오.
- [수동 제어 금지]: Both 모드 작동 중에는 리스트(List) 외부에서 수동으로 레이저를 
  켜지 마십시오. 토큰 동기화가 깨져 위험할 수 있습니다. 수동 조작시에는 반드시 Head 1 또는 Head 2 전용 모드로 전환 후 제어하십시오.


## 7. 데모 프로그램 (Demo Program)

GitHub 링크 : https://github.com/labspiral/sirius3/
프로젝트 위치 : Demos/editor_multibeam (2개의 편집기 2개의 Document 데이타 사용으로 서로 다른 가공 데이타를 2개의 스캔헤드가 배타적으로 가공)
프로젝트 위치 : Demos/editor_multibeam2 (1개의 편집기 1개의 Document 데이타 사용으로 서로 같은 가공 데이티를 2개의 스캔헤드가 배타적으로 가공)
- 사용자 환경에 맞게 config_multibeam.ini 파일을 편집하십시오.
- AOM 장치 및 레이저 소스가 연결되지 않은 상태에서 테스트를 먼저 진행하십시오.
- Check Pins 테스트를 통해 RTC 카드간 DIO 연결을 테스트 하십시오.


---
2026 Copyright (c) SpiralLAB. All rights reserved.
