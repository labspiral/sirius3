# PropertyGrid 사용 설명서

> API 기준: Sirius3 Core/UI 1.17.0 (공개 기능)

[English](PropertyGridUserManual.md) | [한국어](PropertyGridUserManual.koKR.md) | [전체 안내서](README.koKR.md)

## 1. 편집 대상 선택

편집기 또는 TreeView에서 Page·Layer·개체·EntityPen·EntityLayerPen·장치를
선택합니다. PropertyGrid에는 대상이 지원하는 설정이 표시됩니다. 편집 전 선택한
이름과 값을 확인하십시오. 빈 화면은 보통 적용할 선택 대상이 없는 상태입니다.
여러 개체를 선택하면 공통으로 편집 가능한 속성을 표시합니다.

## 2. 설정 찾기

Ctrl+F로 검색 칸에 포커스를 두고 속성 이름·분류·설명을 검색합니다. 검색을 지우면
전체 목록이 복원되며 필터에서 제외한 속성이 삭제되는 것은 아닙니다. 등록 장치가
지원하지 않는 속성은 숨겨지거나 읽기 전용으로 표시됩니다. 설명은 선택한 UI 언어로
단위·선행 조건·관련 설정을 안내합니다.

## 3. 값과 미리보기 적용

표시한 단위로 값을 입력하고 Enter 또는 편집기의 수락 명령으로 완료합니다.
범위 제한이나 컨트롤러 해상도 변환이 적용되면 최종 표시 값을 확인하십시오.
편집기를 통한 형상 변경은 화면을 갱신하며 문서 Undo/Redo에 반영됩니다.

드롭다운에 Apply가 있으면 미리보기로 확인할 수 있습니다. 수락은 변경을 확정하고
취소 또는 수락하지 않은 닫기는 이전 값으로 복원합니다. 여러 대상을 선택했다면
의도한 대상 모두를 확인합니다. 텍스트 칸은 커서·문자·편집 키를 자체 사용하므로
그리기 단축키를 쓰려면 뷰포트로 포커스를 옮기십시오.

## 4. 주요 분류

| 분류 | 예 |
|---|---|
| 기본 / 변환 | 이름·표시/마킹 허용·위치·회전·배율 |
| 형상 / 텍스트 / 해치 | 치수·문자열·문자 간격·해치 목록 |
| EntityPen | 출력·주파수·속도·지연·래스터·점 설정 |
| EntityLayerPen | ALC·Sky Writing·가변 지연·지원 컨트롤러 모드 |

가공 설정의 관계는 [Pen](PenUserManual.koKR.md), [LayerPen](LayerPenUserManual.koKR.md)을
참고하십시오. 외관을 바꾸는 설정이 반드시 레이저 출력을 바꾸는 것은 아닙니다.

## 5. 프레임워크와 작업 상태

WinForms·WPF는 각 프레임워크의 컨트롤로 같은 문서·장치 속성 편집 흐름을 제공합니다.
설명 영역과 드롭다운은 호스트의 글꼴·DPI를 따릅니다. Marker가 Busy이면 편집이
제한됩니다. 가공 시작 전 선택 장치와 준비 상태를 확인하십시오.

[편집기](EditorUserManual.koKR.md), [TreeView](TreeviewUserManual.koKR.md),
[WinForms](WinForms.koKR.md), [WPF](WPF.koKR.md), [Font Editor](FontEditorUserManual.koKR.md)를 참고하십시오.


---
2026 Copyright (c) SpiralLAB. All rights reserved.
