using System.Collections.Generic;
using NUnit.Framework;

namespace ArcherCraftor.Tests.EditMode
{
    [TestFixture]
    public class FieldCraftingMathTests
    {
        // GDD 4.0 & 5.0 현장 즉석 제작 로직 단위 테스트

        public enum ArrowType
        {
            Standard,
            Impact,
            Blast,
            Frost,
            Spark
        }

        public enum ResourceType
        {
            FlintStone,     // 충격석 / 부싯돌
            FlameBerry,     // 화염 열매
            FrostMushroom,  // 서리 버섯
            SparkCrystal    // 정전기 결정
        }

        private static readonly Dictionary<ArrowType, ResourceType> RecipeTable = new Dictionary<ArrowType, ResourceType>
        {
            { ArrowType.Impact, ResourceType.FlintStone },
            { ArrowType.Blast, ResourceType.FlameBerry },
            { ArrowType.Frost, ResourceType.FrostMushroom },
            { ArrowType.Spark, ResourceType.SparkCrystal }
        };

        private static bool TryCraft(
            ArrowType arrowType,
            Dictionary<ResourceType, int> inventory,
            Dictionary<ArrowType, int> quiver,
            int maxPerSlot = 10)
        {
            if (arrowType == ArrowType.Standard) return true; // 기본 화살은 무한

            if (!RecipeTable.TryGetValue(arrowType, out var requiredResource))
                return false;

            if (!inventory.TryGetValue(requiredResource, out var availableCount) || availableCount < 1)
                return false;

            quiver.TryGetValue(arrowType, out var currentArrows);
            if (currentArrows >= maxPerSlot)
                return false;

            inventory[requiredResource] = availableCount - 1;
            quiver[arrowType] = currentArrows + 1;
            return true;
        }

        [Test]
        public void StandardArrow_AlwaysAvailableWithoutResource()
        {
            var inventory = new Dictionary<ResourceType, int>();
            var quiver = new Dictionary<ArrowType, int>();

            bool canCraft = TryCraft(ArrowType.Standard, inventory, quiver);
            Assert.IsTrue(canCraft, "기본 화살은 재료 소모 없이 언제나 무한이어야 함");
        }

        [Test]
        public void SpecialArrow_CraftsWhenMaterialPresent()
        {
            var inventory = new Dictionary<ResourceType, int> { { ResourceType.FlameBerry, 3 } };
            var quiver = new Dictionary<ArrowType, int> { { ArrowType.Blast, 0 } };

            bool crafted = TryCraft(ArrowType.Blast, inventory, quiver);

            Assert.IsTrue(crafted);
            Assert.AreEqual(2, inventory[ResourceType.FlameBerry], "재료가 1개 차감되어야 함");
            Assert.AreEqual(1, quiver[ArrowType.Blast], "화살통에 화염 화살 1발이 생성되어야 함");
        }

        [Test]
        public void SpecialArrow_FailsWhenMaterialMissing()
        {
            var inventory = new Dictionary<ResourceType, int> { { ResourceType.FrostMushroom, 0 } };
            var quiver = new Dictionary<ArrowType, int> { { ArrowType.Frost, 0 } };

            bool crafted = TryCraft(ArrowType.Frost, inventory, quiver);

            Assert.IsFalse(crafted, "재료가 없으면 제작 실패해야 함");
            Assert.AreEqual(0, quiver[ArrowType.Frost]);
        }

        [Test]
        public void QuiverCapacity_CapsAtMaxSlots()
        {
            var inventory = new Dictionary<ResourceType, int> { { ResourceType.FlintStone, 15 } };
            var quiver = new Dictionary<ArrowType, int> { { ArrowType.Impact, 10 } }; // 슬롯 최대 10발

            bool crafted = TryCraft(ArrowType.Impact, inventory, quiver, maxPerSlot: 10);

            Assert.IsFalse(crafted, "슬롯 상한(10발)에 도달하면 추가 제작이 차단되어야 함");
            Assert.AreEqual(15, inventory[ResourceType.FlintStone], "재료가 차감되지 않아야 함");
        }
    }
}

