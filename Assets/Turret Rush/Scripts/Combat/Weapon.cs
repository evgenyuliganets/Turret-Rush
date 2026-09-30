using UnityEngine;
using UnityEngine.Pool;

namespace Turret_Rush.Scripts.Combat
{
    public sealed class Weapon : MonoBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireRate = 5f;

        private ObjectPool<Projectile> _projectilePool;

        private bool _isFiring;
        private float _nextFireTime;


        private void Awake()
        {
            _projectilePool =
                new ObjectPool<Projectile>(
                    CreateProjectile,
                    OnTakeFromPool,
                    OnReturnedToPool,
                    OnDestroyProjectile,
                    collectionCheck: true,
                    defaultCapacity: 10,
                    maxSize: 30
                );
        }


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
            Projectile projectile =
                _projectilePool.Get();

            projectile.transform.SetPositionAndRotation(
                firePoint.position,
                firePoint.rotation
            );

            projectile.Launch(
                firePoint.forward
            );
        }

        private Projectile CreateProjectile()
        {
            Projectile projectile =
                Instantiate(projectilePrefab);

            projectile.Initialize(
                ReleaseProjectile
            );

            return projectile;
        }

        private void OnTakeFromPool(
            Projectile projectile)
        {
            projectile.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(
            Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
        }

        private void OnDestroyProjectile(
            Projectile projectile)
        {
            Destroy(projectile.gameObject);
        }
        
        private void ReleaseProjectile(
            Projectile projectile)
        {
            _projectilePool.Release(projectile);
        }
    }
}