using NUnit.Framework;
using UnityEngine;

namespace ArcherCraftor.Tests.EditMode
{
    [TestFixture]
    public class ArrowParryMathTests
    {
        // 탄막 상쇄(Counter-Shot) 핵심 거리 판정 수식 검증
        // 화살과 탄환 간의 중심 거리 <= (화살 자석 반경 + 탄환 히트박스 반경)

        private static bool IsInParryRange(Vector3 arrowPos, float arrowParryRadius, Vector3 bulletPos, float bulletRadius)
        {
            float maxDistance = arrowParryRadius + bulletRadius;
            float sqrDist = (arrowPos - bulletPos).sqrMagnitude;
            return sqrDist <= (maxDistance * maxDistance);
        }

        [Test]
        public void BulletInsideMagneticRadius_IsIntercepted()
        {
            Vector3 arrowPos = new Vector3(5f, 0f, 5f);
            float arrowParryRadius = 0.8f;

            Vector3 bulletPos = new Vector3(5.5f, 0f, 5.2f); // 거리 약 0.538m
            float bulletRadius = 0.3f;

            bool parried = IsInParryRange(arrowPos, arrowParryRadius, bulletPos, bulletRadius);
            Assert.IsTrue(parried, "자석 반경(0.8) + 탄환 반경(0.3) = 1.1m 내의 탄환은 상쇄되어야 함");
        }

        [Test]
        public void BulletOutsideMagneticRadius_IsNotIntercepted()
        {
            Vector3 arrowPos = new Vector3(5f, 0f, 5f);
            float arrowParryRadius = 0.8f;

            Vector3 bulletPos = new Vector3(7f, 0f, 5f); // 거리 2.0m
            float bulletRadius = 0.3f;

            bool parried = IsInParryRange(arrowPos, arrowParryRadius, bulletPos, bulletRadius);
            Assert.IsFalse(parried, "범위 밖의 탄환은 상쇄 판정을 받지 않아야 함");
        }

        [Test]
        public void VerticalDifference_MaintainsQuarterViewPlaneCheck()
        {
            Vector3 arrowPos = new Vector3(0f, 1f, 0f);
            float arrowParryRadius = 0.8f;

            // 쿼터뷰 2D 평면 투영 거리 체크 (Y축 무시 시 동일 위치)
            Vector3 bulletPos = new Vector3(0.2f, 1.1f, 0.3f);
            float bulletRadius = 0.2f;

            bool parried = IsInParryRange(arrowPos, arrowParryRadius, bulletPos, bulletRadius);
            Assert.IsTrue(parried);
        }
    }
}

