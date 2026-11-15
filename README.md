# ArcherCraftor (아처크래프터) - 기초 프로토타입 뼈대

> 《이스케이프 프롬 덕코프(Escape from Duckov)》 스타일의 **3D 로우폴리 탑다운 쿼터뷰** + **궁술 탄막 상쇄(카운터샷) 액션** 프로토타입 프로젝트입니다.

---

## 🎮 핵심 조작법 (Controls)

| 키 입력 | 동작 | 설명 |
| :--- | :--- | :--- |
| **W, A, S, D** | 이동 | 캐릭터 이동 (탑다운 쿼터뷰 평면 기준) |
| **마우스 이동** | 조준 & 회전 | 마우스 커서 방향으로 궁수 캐릭터 360도 실시간 회전 |
| **마우스 휠 (Scroll)** | **특수 화살 선택** | 휠을 굴려 특수 화살 탄종(충격, 화염, 서리, 번개 등) 즉시 순환 선택 |
| **좌클릭 (탭/홀드)** | **기본 화살 발사 (속사/차징)** | [무한 탄약] 빠른 탄막 요격/상쇄(탭) 및 관통 파워샷(홀드) |
| **우클릭 (탭/홀드)** | **특수 화살 발사 (속사/차징)** | [제작 소모] 선택된 특수 화살 즉발 발사(탭) 및 광역/특수 기믹 강화(홀드) |
| **Space Bar** | 대시 (회피) | 이동 방향으로 순간 대시 (무적 회피) |
| **R 키** | 씬 재시작 | 테스트 샌드박스 즉시 리셋 |

---

## 🚀 빠른 시작 가이드 (Unity Editor)

1. **Unity Hub**를 실행합니다.
2. **[열기(Open)]** 버튼을 누르고 이 폴더(`c:\Users\niji0\Desktop\ArcherCraftor`)를 선택합니다.
   - 권장 에디터 버전: **Unity 6 (6000.3.22f1)** 또는 2022.3 LTS 이상
3. 프로젝트가 로드된 후 상단 메뉴바를 확인합니다:
   - **`[ArcherCraftor] -> [Setup Prototype Sandbox Scene]`** 클릭!
4. 클릭 한 번으로 바닥, 장애물 기둥, 궁수 플레이어, 적 골렘, 탄막/화살 프리팹, 쿼터뷰 카메라, FOV 시야각 메시, 조준선 UI가 자동으로 완벽하게 구성되어 **`Assets/Scenes/PrototypeSandbox.unity`**로 저장됩니다.
5. 에디터 상단의 **[ ▶ Play ]** 버튼을 누르면 즉시 플레이 가능합니다!

---

## 📂 프로젝트 아키텍처 구조

```
Assets/
├── Scripts/
│   ├── Combat/
│   │   ├── ArrowProjectile.cs     # 관통 및 자석 히트박스(OverlapSphere) 기반 탄막 상쇄
│   │   ├── EnemyBullet.cs         # 적 탄막 투사체 및 패링(OnParried) 연동
│   │   ├── Health.cs / IDamageable.cs # 피격/체력 및 무적/사망 처리
│   │   └── ImpactEffect.cs        # 충돌 스파크/이펙트 자동 파괴
│   ├── Player/
│   │   ├── PlayerController.cs    # 쿼터뷰 이동, 마우스 회전, 대시
│   │   └── PlayerBow.cs           # 퀵샷 vs 차징 파워샷 발사 및 화살 변경 시스템
│   ├── Enemy/
│   │   ├── EnemyAI.cs             # 카이팅 거리 유지 및 탄막 공격 인공지능
│   │   └── EnemyBulletEmitter.cs  # 단발 조준, 부채꼴 확산, 360도 링 탄막 생성기
│   ├── Vision/
│   │   └── FieldOfView.cs         # 덕코프식 장애물 차단 절차적 원뿔 시야각 메시
│   ├── Core/
│   │   ├── TopDownCameraFollow.cs # 60도 쿼터뷰 + 마우스 조준 방향 Aim Peek 시야 확장
│   │   └── GameManager.cs         # 상쇄 횟수 통계 및 게임 루프 관리
│   ├── UI/
│   │   ├── AimReticleUI.cs        # 마우스 크로스헤어, 사거리 링, 차징 게이지 UI
│   │   └── InGameHUD.cs           # HP 슬라이더 및 플레이어 상태
│   └── Editor/
│       └── ArcherCraftorSceneBuilder.cs # 1클릭 자동 프로토타입 씬 빌더 도구
├── Prefabs/                       # 자동 생성된 화살, 탄환, 스파크 프리팹
├── Materials/                     # URP 셰이더 기반 컬러 머티리얼
└── Scenes/                        # 샌드박스 씬
```

---

## 📚 기획 및 설정 문서 (Docs)
- [게임 기획 및 컨셉 문서 (GameConcept.md)](docs/GameConcept.md) : 조작 체계, 탄막 상쇄, 현장 즉석 제작 시스템
- [스토리 및 세계관 문서 (Story.md)](docs/Story.md) : 세계관 배경, 주인공 설정, 챕터별 서사 및 연출 방식

