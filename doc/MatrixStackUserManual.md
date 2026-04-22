# Coordinate Transformation & Matrix Stack User Manual


## 1. 개요 (Overview)

Sirius3는 복잡한 가공 경로를 직관적이고 효율적으로 제어하기 위해 '계층적 좌표 변환' 시스템을 사용합니다. 
사용자(UI/Recipe) 레벨에서의 유연한 도면 배치와 하드웨어(RTC/Scanner) 레벨에서의 물리적 보정을 분리하여 관리하는 것이 핵심입니다.

## 2. MatrixStack (소프트웨어 레벨 변환)

MatrixStack은 OpenGL과 유사한 Push/Pop 방식의 4x4 행렬 스택입니다.

[특징 및 장점]
- 계층적 가공(Nesting): 부품 > 서브 부품 > 개별 도형으로 이어지는 복잡한 구조에서 각 단계의 좌표계를 독립적으로 관리할 수 있습니다.
- 3D 공간 변환: RTC 하드웨어가 직접 처리하기 힘든 3D 회전(Roll, Pitch, Yaw) 및 원근 투영 연산을 PC 소프트웨어 단에서 완벽하게 계산합니다.
- IRtc.MatrixStack의 가치: 가공 도면(Entity) 자체를 수정하지 않고도, 가공 직전에 스택에 행렬을 푸시하는 것만으로 전체 도면의 위치와 각도를 즉시 변경할 수 있습니다. (예: 다배열 트레이 가공 시 유용)

## 3. Offset 구조체와 연동

Offset은 행렬 수학에 익숙하지 않은 사용자도 직관적으로 좌표를 조작할 수 있게 돕는 도구입니다.

- 주요 속성: Translate (이동), AngleZ (회전), Scale (배율).
- ToMatrix 연산: 사용자가 입력한 수치들은 `Scale -> RotateZ -> Translate` 순서의 4x4 행렬로 자동 변환됩니다.
- 활용: 비전 시스템의 측정값(X, Y, Theta 오차)을 Offset에 대입하고, 이를 `MatrixStack.Push(offset.ToMatrix)` 함으로써 실시간 작업물 위치 보정(Part Displacement)을 손쉽게 구현합니다.

## 4. RTC 내부 행렬 (Hardware Internal Matrix)

IRtc 인터페이스는 `MatrixPrimaryInternal` (1번 헤드)과 `MatrixSecondaryInternal` (2번 헤드) 속성을 제공합니다. 
이는 RTC 제어기 내부 하드웨어 레벨에서 처리되는 행렬입니다.

[MatrixStack vs. Internal Matrix 차이]
- MatrixStack (UI/PC): 가공 레시피의 논리적 배치용. PC의 CPU/GPU 자원을 사용하여 4x4 변환 수행.
- Internal Matrix (HW): 실제 장비의 물리적 상태 보정용. RTC 보드 내의 DSP가 가공 중 실시간(10µs 단위)으로 2x2 변환 및 오프셋 적용.

## 5. 최적의 활용 가이드 (Best Practices)

가공 정밀도와 시스템 성능을 극대화하려면 다음과 같이 역할을 분담하십시오.

[MatrixStack 사용 권장]
- 도면 내 개체들의 배열, 그룹화된 부품의 배치.
- 비전 보정에 의한 실시간 궤적 변환.

[Internal Matrix 사용 권장 - 효과적!]
- 스캐너 헤드의 물리적 회전 보정: 스캔 헤드가 장비에 90도 또는 180도 회전되어 장착된 경우, 소프트웨어 도면을 고칠 필요 없이 Internal 행렬만 설정하면 됩니다.
- 엔코더 방향(Flip) 맞춤: MoF(Marking on the Fly) 가공 시 컨베이어 이동 방향과 스캐너 축이 반대라면, 하드웨어 행렬에서 축을 반전시키는 것이 가장 빠르고 안전합니다.
- 다중 헤드 정렬: 두 개의 스캔 헤드를 동일한 좌표계로 일치시킬 때 각 헤드의 Internal 행렬을 독립적으로 설정하여 물리적 오차를 상쇄하십시오.

[결론]
"도면의 논리적 배치는 MatrixStack으로, 장비의 물리적 정렬과 하드웨어 특성(회전/반전) 보정은 Internal Matrix로 관리하는 것이 가장 설계 의도에 부합하며 시스템 부하가 적습니다."

---
2026 Copyright (c) SpiralLAB. All rights reserved.
