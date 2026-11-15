---
description: ArcherCraftor Unity C# 코딩 컨벤션 및 품질 규칙
globs: Assets/Scripts/**/*.cs, Assets/Tests/**/*.cs
---

# ArcherCraftor C# 코딩 표준 및 규칙

## 1. 네이밍 컨벤션
- **클래스, 구조체, 인터페이스, Enum**: `PascalCase` (예: `ArrowProjectile`, `IDamageable`)
- **인터페이스**: 접두사 `I` 필수 (예: `IDamageable`, `ICraftable`)
- **메서드, 프로퍼티, 이벤트**: `PascalCase` (예: `TakeDamage`, `CurrentChargeRatio`, `OnHealthChanged`)
- **이벤트 이름**: 과거분사/수동형 명확화 (`OnDied`, `OnFired`, `OnChargeUpdated`)
- **private / protected 필드**: `camelCase` (예: `currentHealth`, `chargeTimer`)
- **SerializeField 필드**: `camelCase` (언더스코어 접두사 없이 일관성 유지)
- **상수(const), 읽기전용(static readonly)**: `PascalCase` 또는 `ALL_CAPS`

## 2. 직렬화 및 인스펙터 노출
- private 필드에 `[SerializeField]`를 사용하며, public 변수 노출을 지양합니다.
- 복잡한 수치 조정 필드에는 반드시 `[Header(...)]`, `[Tooltip(...)]`, `[Range(...)]`를 작성합니다.

```csharp
[Header("Charging Settings")]
[Tooltip("완충까지 소요되는 시간(초)")]
[Range(0.2f, 3.0f)]
[SerializeField] private float maxChargeTime = 1.0f;
```

## 3. 메모리 및 성능 규칙
- `Update()` / `FixedUpdate()` 내에서 힙 할당(`new List<T>`, `new GameObject`, LINQ 쿼리 등)을 엄격히 금지합니다.
- 물리 충돌 탐색 시 `Physics.OverlapSphereNonAlloc`을 사용하여 미리 선언된 고정 버퍼를 재사용합니다.
- 컴포넌트 캐싱: `GetComponent<T>()`는 반드시 `Awake()` 또는 `Start()`에서 1회 캐싱하여 사용합니다.

## 4. 네임스페이스 규칙
모든 스크립트는 `ArcherCraftor.<기능>` 네임스페이스를 명시해야 합니다:
- `ArcherCraftor.Combat`
- `ArcherCraftor.Player`
- `ArcherCraftor.Enemy`
- `ArcherCraftor.Crafting`
- `ArcherCraftor.Vision`
- `ArcherCraftor.Core`
- `ArcherCraftor.UI`
- `ArcherCraftor.Editor`
- `ArcherCraftor.Tests.EditMode`
- `ArcherCraftor.Tests.PlayMode`
- `ArcherCraftor.Tests.Harness`
