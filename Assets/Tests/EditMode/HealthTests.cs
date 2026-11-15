using NUnit.Framework;
using UnityEngine;
using ArcherCraftor.Combat;
using ArcherCraftor.Tests.Harness;

namespace ArcherCraftor.Tests.EditMode
{
    [TestFixture]
    public class HealthTests
    {
        private Health health;

        [SetUp]
        public void SetUp()
        {
            health = TestHarnessHelper.CreateDummyTarget(100f);
        }

        [TearDown]
        public void TearDown()
        {
            TestHarnessHelper.Cleanup();
        }

        [Test]
        public void TakeDamage_ReducesCurrentHealth()
        {
            health.TakeDamage(30f, Vector3.zero, Vector3.up);

            Assert.AreEqual(70f, health.CurrentHealth, 0.001f);
            Assert.IsFalse(health.IsDead);
        }

        [Test]
        public void TakeDamage_ExcessDamage_ClampsToZero_AndTriggersDeath()
        {
            bool diedTriggered = false;
            health.OnDied += () => diedTriggered = true;

            health.TakeDamage(150f, Vector3.zero, Vector3.up);

            Assert.AreEqual(0f, health.CurrentHealth, 0.001f);
            Assert.IsTrue(health.IsDead);
            Assert.IsTrue(diedTriggered);
        }

        [Test]
        public void TakeDamage_WhenDead_IgnoresFurtherDamage()
        {
            health.TakeDamage(100f, Vector3.zero, Vector3.up);
            Assert.IsTrue(health.IsDead);

            int diedCallbackCount = 0;
            health.OnDied += () => diedCallbackCount++;

            health.TakeDamage(50f, Vector3.zero, Vector3.up);

            Assert.AreEqual(0f, health.CurrentHealth, 0.001f);
            Assert.AreEqual(0, diedCallbackCount, "사망 후 추가 피격 시 OnDied가 다시 발생하지 않아야 함");
        }

        [Test]
        public void Heal_IncreasesHealth_ClampedToMax()
        {
            health.TakeDamage(40f, Vector3.zero, Vector3.up);
            Assert.AreEqual(60f, health.CurrentHealth, 0.001f);

            health.Heal(20f);
            Assert.AreEqual(80f, health.CurrentHealth, 0.001f);

            health.Heal(50f);
            Assert.AreEqual(100f, health.CurrentHealth, 0.001f, "최대 체력을 초과하여 회복되지 않아야 함");
        }

        [Test]
        public void Heal_WhenDead_DoesNotRevive()
        {
            health.TakeDamage(100f, Vector3.zero, Vector3.up);
            Assert.IsTrue(health.IsDead);

            health.Heal(50f);
            Assert.AreEqual(0f, health.CurrentHealth, 0.001f);
            Assert.IsTrue(health.IsDead, "사망한 상태에서는 단순 힐로 부활할 수 없음");
        }
    }
}

