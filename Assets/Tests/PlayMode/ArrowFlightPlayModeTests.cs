using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ArcherCraftor.Combat;
using ArcherCraftor.Tests.Harness;

namespace ArcherCraftor.Tests.PlayMode
{
    public class ArrowFlightPlayModeTests
    {
        private GameObject arrowObject;
        private ArrowProjectile arrowProjectile;

        [TearDown]
        public void TearDown()
        {
            TestHarnessHelper.Cleanup();
        }

        [UnityTest]
        public IEnumerator ArrowMovesForward_AlongDirection()
        {
            arrowObject = TestHarnessHelper.CreateTestObject("TestArrow");
            arrowObject.AddComponent<SphereCollider>();
            arrowProjectile = arrowObject.AddComponent<ArrowProjectile>();

            Vector3 startPos = Vector3.zero;
            Vector3 shootDirection = Vector3.forward;
            float baseSpeed = 20f;

            arrowObject.transform.position = startPos;
            arrowProjectile.Initialize(shootDirection, 0.5f, baseSpeed, 30f);

            // 0.2초 대기
            yield return new WaitForSeconds(0.2f);

            // Z축으로 전진했는지 확인
            Assert.Greater(arrowObject.transform.position.z, startPos.z + 1.0f, "화살이 발사 방향(Forward)으로 정상 이동해야 함");
            Assert.AreEqual(startPos.y, arrowObject.transform.position.y, 0.05f, "Y축(수평 쿼터뷰) 높이가 일정하게 유지되어야 함");
        }
    }
}

