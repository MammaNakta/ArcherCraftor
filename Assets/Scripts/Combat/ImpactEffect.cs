using UnityEngine;

namespace ArcherCraftor.Combat
{
    public class ImpactEffect : MonoBehaviour
    {
        [SerializeField] private float lifetime = 1.5f;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }
    }
}
