using UnityEngine;

namespace Turret_Rush.Scripts.Combat
{
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 25f;
        [SerializeField] private float lifetime = 3f;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            transform.position +=
                transform.forward * (speed * Time.deltaTime);
        }
    }
}