# Hollow: Cook Off

<div align="center">

[🇺🇸 English](./README.md) | 🇰🇷 한국어

</div>

FSM 기반 AI와 핵심 전투 시스템을 설계·구현한 액션 사이드 슈팅 게임  
팀 프로젝트로 제작되었으며, 프로그래밍 전반을 담당했습니다.

[![Hollow: Cook Off Trailer](https://img.youtube.com/vi/cNTTR39F9BM/0.jpg)](https://www.youtube.com/watch?v=cNTTR39F9BM)

[▶ Steam 페이지](https://store.steampowered.com/app/2640380/Hollow_Cook_Off/)

| 항목 | 내용 |
|------|------|
| 장르 | 액션 사이드 슈팅 |
| 엔진 | Unity (C#) |
| 플랫폼 | PC (Steam) |
| 출시 연도 | 2024 |

## 주요 기능

- **FSM 기반 적 AI**: 상태별로 캡슐화된 FSM을 통해 일반 적과 멀티 페이즈 보스의 전투 행동을 구현했습니다.
- **무기 시스템**: 무기 타입과 사격 로직을 모듈화하여 확장이 용이하도록 설계했습니다.
- **플레이어 특수 능력**: 탄환 패링, 슬래시 콤보 등 액션 중심의 전투 시스템을 구현했습니다.
- **데이터 기반 대화 시스템**: JSON 데이터를 활용해 코드 수정 없이 대화 내용을 편집하고 재생할 수 있습니다.

## 코드 구조

```
Hollow-CookOff-Codes/
├── Enemy/                    # 적 유닛 및 보스 행동
├── UI/                       # UI 시스템
├── Util/                     # 범용 유틸리티
├── JsonCogi/                 # JSON 데이터 관리
├── MainCharacter.cs          # 플레이어 캐릭터 제어
├── SpecialAbility.cs         # 플레이어 특수 능력
├── BulletParry.cs            # 탄환 패링 시스템
├── SlashCombo.cs             # 슬래시 콤보 시스템
├── ChatDB.cs                 # 대화 데이터베이스
├── ChatEventTriger.cs        # 대화 이벤트 트리거
├── SaveDataManager.cs        # 세이브 데이터 관리
└── BGMManager.cs             # 사운드 제어```
```

## 주요 구현

### 1. FSM 기반 적 AI
- 각 상태(`Enter`, `Update`, `Exit`)를 독립된 클래스로 관리하여 유지보수와 상태 추가가 용이합니다.
- 보스의 경우 체력에 따른 페이즈 전환 로직을 포함하여 복합적인 공격 패턴을 제공합니다.

### 2. 무기 시스템
- 발사 방식, 탄속, 데미지 등의 수치를 Unity 인스펙터에서 즉시 조정 가능하도록 설계했습니다.
- `WeaponBox`를 활용한 씬 배치 방식으로 기획 의도에 따른 무기 배치가 자유롭습니다.

### 3. 액션 시스템
- **Bullet Parry**: 타이밍에 맞춰 탄환을 패링합니다.
- **Slash Combo**: 콤보를 판정합니다.
- **BioEnerge**: 특수 능력 사용에 필요한 자원 시스템입니다
- **SpecialAbility**: 착용 무기에 따라 특수 공격을 사용 가능합니다

### 4. 대화 시스템

- 대화 내용을 JSON으로 관리하여 기획자가 코드 수정 없이 편집할 수 있습니다
- `ChatEventTrigger`로 씬 내 특정 위치나 조건에서 대화를 재생합니다

## 라이선스

[MIT License](LICENSE)
