using NUnit.Framework;
using UnityEngine;

namespace ArcherCraftor.Tests.EditMode
{
    [TestFixture]
    public class AimChargeMathTests
    {
        // GDD 기획 수식 검증
        // 1. 조준 각도: Lerp(120, 0, chargeRatio)
        // 2. 사거리: Lerp(minRange, maxRange, chargeRatio)
        // 3. 탄속: Lerp(baseSpeed * 0.7f, baseSpeed * 1.5f, chargeRatio)
        // 4. 대미지: Lerp(baseDamage * 0.5f, baseDamage * 2.2f, chargeRatio)
        // 5. 관통력: chargeRatio >= 0.85f ? basePierce + 2 : basePierce

        private static float CalculateAimAngle(float chargeRatio)
        {
            return Mathf.Lerp(120f, 0f, Mathf.Clamp01(chargeRatio));
        }

        private static float CalculateAimRange(float chargeRatio, float minRange = 6f, float maxRange = 22f)
        {
            return Mathf.Lerp(minRange, maxRange, Mathf.Clamp01(chargeRatio));
        }

        private static (float speed, float damage, int pierce) CalculateArrowSpecs(
            float chargeRatio, float baseSpeed = 25f, float baseDamage = 30f, int basePierce = 1)
        {
            float c = Mathf.Clamp01(chargeRatio);
            float speed = Mathf.Lerp(baseSpeed * 0.7f, baseSpeed * 1.5f, c);
            float damage = Mathf.Lerp(baseDamage * 0.5f, baseDamage * 2.2f, c);
            int pierce = basePierce + (c >= 0.85f ? 2 : 0);
            return (speed, damage, pierce);
        }

        [Test]
        public void TapShot_ReturnsMaxAngleAndMinRange()
        {
            float angle = CalculateAimAngle(0f);
            float range = CalculateAimRange(0f, 6f, 22f);

            Assert.AreEqual(120f, angle, 0.001f, "속사(탭) 시 각도는 최대 120도여야 함");
            Assert.AreEqual(6f, range, 0.001f, "속사(탭) 시 사거리는 최소치여야 함");
        }

        [Test]
        public void FullCharge_ReturnsZeroAngleAndMaxRange()
        {
            float angle = CalculateAimAngle(1.0f);
            float range = CalculateAimRange(1.0f, 6f, 22f);

            Assert.AreEqual(0f, angle, 0.001f, "풀차징 시 각도는 0도(완전 직선)여야 함");
            Assert.AreEqual(22f, range, 0.001f, "풀차징 시 사거리는 최대치여야 함");
        }

        [Test]
        public void HalfCharge_ReturnsMidpointAngleAndRange()
        {
            float angle = CalculateAimAngle(0.5f);
            float range = CalculateAimRange(0.5f, 6f, 22f);

            Assert.AreEqual(60f, angle, 0.001f, "50% 차징 시 각도는 60도여야 함");
            Assert.AreEqual(14f, range, 0.001f, "50% 차징 시 사거리는 14m여야 함");
        }

        [Test]
        public void ArrowSpecs_ScaleCorrectly_AtThresholds()
        {
            // 퀵샷 (c = 0)
            var tapSpecs = CalculateArrowSpecs(0f, 20f, 40f, 1);
            Assert.AreEqual(14f, tapSpecs.speed, 0.001f); // 20 * 0.7
            Assert.AreEqual(20f, tapSpecs.damage, 0.001f); // 40 * 0.5
            Assert.AreEqual(1, tapSpecs.pierce);

            // 임계치 미만 (c = 0.84)
            var nearFullSpecs = CalculateArrowSpecs(0.84f, 20f, 40f, 1);
            Assert.AreEqual(1, nearFullSpecs.pierce, "0.85 미만에서는 관통력 보너스가 없어야 함");

            // 임계치 도달 (c = 0.85)
            var thresholdSpecs = CalculateArrowSpecs(0.85f, 20f, 40f, 1);
            Assert.AreEqual(3, thresholdSpecs.pierce, "0.85 이상에서는 관통력 +2 보너스가 부여되어야 함");

            // 풀차징 (c = 1.0)
            var fullSpecs = CalculateArrowSpecs(1.0f, 20f, 40f, 1);
            Assert.AreEqual(30f, fullSpecs.speed, 0.001f); // 20 * 1.5
            Assert.AreEqual(88f, fullSpecs.damage, 0.001f); // 40 * 2.2
            Assert.AreEqual(3, fullSpecs.pierce);
        }
    }
}

