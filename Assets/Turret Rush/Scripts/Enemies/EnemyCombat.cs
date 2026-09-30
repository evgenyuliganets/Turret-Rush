using System;
using Turret_Rush.Scripts.Combat;
using UnityEngine;

namespace Turret_Rush.Scripts.Enemies
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class EnemyCombat : MonoBehaviour
    {
        [SerializeField] private EnemyConfig enemyConfig;

        private Rigidbody _rigidbody;

        private Transform _target;
        private IDamageable _targetDamageable;
        private Collider _targetCollider;

        private bool _attackDamageApplied;
        private bool _hasCollidedWithCar;

        public event Action<bool> CollidedWithCar;

        public bool IsTargetInAttackRange
        {
            get
            {
                if (_target is null ||
                    _targetCollider is null)
                {
                    return false;
                }

                Vector3 closestPoint =
                    _targetCollider.ClosestPoint(
                        _rigidbody.position
                    );

                float distance = Vector3.Distance(
                    _rigidbody.position,
                    closestPoint
                );

                return distance <=
                       enemyConfig.AttackRange;
            }
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Initialize(Transform target)
        {
            _target = target;

            _targetDamageable =
                target.GetComponent<IDamageable>();

            _targetCollider =
                target.GetComponent<Collider>();

            if (_targetDamageable is null)
            {
                Debug.LogError(
                    $"{target.name} has no IDamageable."
                );
            }

            if (_targetCollider is null)
            {
                Debug.LogError(
                    $"{target.name} has no Collider."
                );
            }
        }

        public void BeginAttack()
        {
            _attackDamageApplied = false;
        }

        public void ApplyAttackDamage()
        {
            if (_attackDamageApplied)
                return;

            _attackDamageApplied = true;

            _targetDamageable?.TakeDamage(
                enemyConfig.AttackDamage
            );
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_target is null ||
                _hasCollidedWithCar)
            {
                return;
            }

            if (collision.transform.root != _target.root)
                return;

            _hasCollidedWithCar = true;

            bool hitFrontHalf =
                IsFrontHalfCollision(collision);

            float damage = hitFrontHalf
                ? enemyConfig.FrontCollisionDamage
                : enemyConfig.RearCollisionDamage;

            _targetDamageable?.TakeDamage(damage);

            CollidedWithCar?.Invoke(hitFrontHalf);
        }
        
        
        private bool IsFrontHalfCollision(
            Collision collision)
        {
            Vector3 contactPoint =
                collision.GetContact(0).point;

            Vector3 localContactPoint =
                _target.InverseTransformPoint(
                    contactPoint
                );

            Vector3 localColliderCenter =
                _target.InverseTransformPoint(
                    _targetCollider.bounds.center
                );

            return localContactPoint.z >=
                   localColliderCenter.z;
        }
        
        public bool IsBehindTarget
        {
            get
            {
                if (_target is null ||
                    _targetCollider is null)
                {
                    return false;
                }

                Vector3 localEnemyPosition =
                    _target.InverseTransformPoint(
                        transform.position
                    );

                Vector3 localColliderCenter =
                    _target.InverseTransformPoint(
                        _targetCollider.bounds.center
                    );

                return localEnemyPosition.z <
                       localColliderCenter.z;
            }
        }
    }
}