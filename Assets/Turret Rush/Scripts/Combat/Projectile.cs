using System;
using UnityEngine;

namespace Turret_Rush.Scripts.Combat
{
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 20f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private ParticleSystem impactVfxPrefab;


        private Transform _vfxParent;

        private Action<Projectile> _releaseAction;

        private Vector3 _direction;
        private float _remainingLifetime;

        private bool _isActive;

        public void Initialize(
            Action<Projectile> releaseAction, Transform vfxParent)
        {
            _releaseAction = releaseAction;

            _vfxParent = vfxParent;
        }

        public void Launch(Vector3 direction)
        {
            trailRenderer.Clear();
            trailRenderer.emitting = true;

            _direction = direction.normalized;
            _remainingLifetime = lifetime;
            _isActive = true;
        }

        private void Update()
        {
            if (!_isActive)
                return;

            transform.position +=
                _direction * (speed * Time.deltaTime);

            _remainingLifetime -= Time.deltaTime;

            if (_remainingLifetime <= 0f)
                Release();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActive)
                return;

            IDamageable damageable =
                other.GetComponentInParent<IDamageable>();

            if (damageable is null)
                return;

            damageable.TakeDamage(damage);

            Instantiate(
                impactVfxPrefab,
                transform.position,
                Quaternion.identity,
                _vfxParent
            );

            Release();
        }

        private void Release()
        {
            if (!_isActive)
                return;

            _isActive = false;

            trailRenderer.emitting = false;
            trailRenderer.Clear();

            _releaseAction?.Invoke(this);
        }
    }
}