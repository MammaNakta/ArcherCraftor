using System.Collections.Generic;
using UnityEngine;
using ArcherCraftor.Combat;

namespace ArcherCraftor.Tests.Harness
{
    /// <summary>
    /// 단위 테스트 및 통합 테스트에서 테스트 객체 생성 및 격리 정리를 돕는 테스트 하네스 유틸리티
    /// </summary>
    public static class TestHarnessHelper
    {
        private static readonly List<GameObject> spawnedObjects = new List<GameObject>();

        /// <summary>
        /// 테스트용 격리된 게임오브젝트를 생성하고 정리 목록에 등록
        /// </summary>
        public static GameObject CreateTestObject(string name = "TestObject")
        {
            GameObject obj = new GameObject($"[Harness]_{name}");
            spawnedObjects.Add(obj);
            return obj;
        }

        /// <summary>
        /// IDamageable 및 Health 컴포넌트를 가진 더미 타깃 생성
        /// </summary>
        public static Health CreateDummyTarget(float maxHp = 100f)
        {
            GameObject obj = CreateTestObject("DummyTarget");
            Health health = obj.AddComponent<Health>();

            // Reflection을 통해 private maxHealth, currentHealth 초기화 (EditMode 대응)
            var maxHpField = typeof(Health).GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var curHpField = typeof(Health).GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            maxHpField?.SetValue(health, maxHp);
            curHpField?.SetValue(health, maxHp);

            return health;
        }

        /// <summary>
        /// 테스트 후 생성된 모든 임시 객체 정리
        /// </summary>
        public static void Cleanup()
        {
            for (int i = spawnedObjects.Count - 1; i >= 0; i--)
            {
                if (spawnedObjects[i] != null)
                {
                    if (Application.isPlaying)
                    {
                        Object.Destroy(spawnedObjects[i]);
                    }
                    else
                    {
                        Object.DestroyImmediate(spawnedObjects[i]);
                    }
                }
            }
            spawnedObjects.Clear();
        }
    }
}

