using Turret_Rush.Scripts.Combat;
using UnityEngine;

namespace Turret_Rush.Scripts.Feedback
{
    [RequireComponent(typeof(Health))]
    public sealed class DamageNumberEmitter : MonoBehaviour
    {
        [SerializeField] private DamageNumberView damageNumberPrefab;

        [SerializeField] private float randomOffset = 0.4f;

        [SerializeField] private Vector3 spawnOffset =
            new(0f, 2f, 0f);

        [SerializeField] private float moveSpeed = 1.5f;

        private Health _health;
        private Camera _camera;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            _health.Damaged += OnDamaged;
        }

        private void OnDisable()
        {
            _health.Damaged -= OnDamaged;
        }

        private void OnDamaged(float damage)
        {
            Vector3 randomScreenOffset =
                _camera.transform.right *
                Random.Range(-randomOffset, randomOffset);

            Vector3 finalOffset =
                spawnOffset + randomScreenOffset;

            DamageNumberView damageNumber =
                Instantiate(
                    damageNumberPrefab,
                    transform.position + finalOffset,
                    Quaternion.identity
                );

            damageNumber.Initialize(
                damage,
                _camera,
                transform,
                finalOffset,
                moveSpeed
            );
        }
    }
}