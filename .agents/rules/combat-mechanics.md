---
description: ArcherCraftor 전투 수식, 탄막 상쇄, 차징 조준 메커니즘 명세
globs: Assets/Scripts/Combat/**/*.cs, Assets/Scripts/Player/**/*.cs
---

# ArcherCraftor 전투 메커니즘 규칙 명세

## 1. 조준 시스템 수식 (Aiming & Charging)

활시위를 당길수록(차징 비율 $C \in [0.0, 1.0]$) 조준 각도는 좁아지고, 사거리는 증가합니다.

- **조준 부채꼴 각도**:
  $$\text{SpreadAngle}(C) = \text{Lerp}(120^\circ, 0^\circ, C)$$
  - 속사(Tap, $C = 0$): $120^\circ$ (넓은 탄막 요격 범위)
  - 완충(Full Charge, $C = 1.0$): $0^\circ$ (완전한 일직선 정밀 저격)

- **조준 사거리**:
  $$\text{AimRange}(C) = \text{Lerp}(\text{MinRange}, \text{MaxRange}, C)$$

- **화살 비행 속도 & 대미지**:
  - $\text{Speed} = \text{Lerp}(\text{BaseSpeed} \times 0.7, \text{BaseSpeed} \times 1.5, C)$
  - $\text{Damage} = \text{Lerp}(\text{BaseDamage} \times 0.5, \text{BaseDamage} \times 2.2, C)$
  - 관통 횟수: $C \ge 0.85$ 일 때 추가 관통 횟수 $+2$

## 2. 탄막 요격 및 상쇄 (Magnetic Parry / Counter-Shot)
- 플레이어의 화살은 비행 중 반경 $\text{magneticParryRadius}$ 내의 적 탄환(`EnemyBullet`)을 감지합니다.
- 화살의 관통 횟수가 남아있으면:
  1. 감지된 적 탄환의 `OnParried()`를 호출하여 탄막을 소거하고 스파크 파티클을 재생합니다.
  2. 화살의 남은 관통 횟수를 1 차감합니다.
  3. `GameManager.Instance.RegisterParry()`를 호출하여 상쇄 카운트를 기록합니다.

