using UnityEngine;

namespace Turret_Rush.Scripts.Combat
{
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 25f;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private float damage = 25f;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            transform.position +=
                transform.forward * (speed * Time.deltaTime);
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}