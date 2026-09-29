using UnityEngine;

namespace Turret_Rush.Scripts.Combat
{
    public sealed class Weapon : MonoBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireRate = 5f;

        private bool _isFiring;
        private float _nextFireTime;

        public void StartFiring()
        {
            _isFiring = true;
        }

        public void StopFiring()
        {
            _isFiring = false;
        }

        private void Update()
        {
            if (!_isFiring)
                return;

            if (Time.time < _nextFireTime)
                return;

            Fire();

            _nextFireTime = Time.time + 1f / fireRate;
        }

        private void Fire()
        {
            Instantiate(
                projectilePrefab,
                firePoint.position,
                firePoint.rotation
            );
        }
    }
}