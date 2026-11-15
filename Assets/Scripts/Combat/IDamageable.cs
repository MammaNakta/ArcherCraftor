using UnityEngine;

namespace ArcherCraftor.Combat
{
    public interface IDamageable
    {
        void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal);
    }
}
