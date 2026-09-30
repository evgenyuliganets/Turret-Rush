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
            Debug.Log($"Projectile hit: {other.name}");

            IDamageable damageable =
                other.GetComponentInParent<IDamageable>();

            if (damageable == null)
            {
                Debug.Log($"No IDamageable on: {other.name}");
                Destroy(gameObject);
                return;
            }

            Debug.Log($"Damage enemy: {other.name}");

            damageable.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}