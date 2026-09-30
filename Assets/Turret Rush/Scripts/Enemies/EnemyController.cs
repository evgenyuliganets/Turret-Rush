using System;
using Turret_Rush.Scripts.Combat;
using UnityEngine;

namespace Turret_Rush.Scripts.Enemies
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyConfig enemyConfig;
        [SerializeField] private EnemyAnimator enemyAnimator;

        private Health _health;
        private Rigidbody _rigidbody;

        private Transform _target;
        private IDamageable _targetDamageable;
        private Collider _targetCollider;

        private bool _hasCollidedWithCar;

        public EnemyState State { get; private set; } =
            EnemyState.Idle;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _rigidbody = GetComponent<Rigidbody>();

            _health.Initialize(enemyConfig.MaxHealth);
        }

        private void Start()
        {
            enemyAnimator.SetIdle();
        }

        private void OnEnable()
        {
            _health.Died += OnDied;
        }

        private void OnDisable()
        {
            _health.Died -= OnDied;
        }

        private void Update()
        {
            if (_target is null)
                return;

            switch (State)
            {
                case EnemyState.Idle:
                    UpdateIdle();
                    break;

                case EnemyState.Chasing:
                    UpdateChasing();
                    break;

                case EnemyState.Attacking:
                case EnemyState.Dying:
                case EnemyState.Dead:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void FixedUpdate()
        {
            if (_target is null ||
                State != EnemyState.Chasing)
                return;

            ChaseTarget();
        }

        private void UpdateIdle()
        {
            float distance = Vector3.Distance(
                _rigidbody.position,
                _target.position
            );

            if (distance <= enemyConfig.DetectionRange)
                SetState(EnemyState.Chasing);
        }

        private void UpdateChasing()
        {
            float distance = Vector3.Distance(
                _rigidbody.position,
                _target.position
            );

            if (distance <= enemyConfig.AttackRange)
                StartAttack();
        }

        private void ChaseTarget()
        {
            Vector3 targetPosition = _target.position;
            targetPosition.y = _rigidbody.position.y;

            Vector3 direction =
                targetPosition - _rigidbody.position;

            if (direction.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            Quaternion nextRotation = Quaternion.Slerp(
                _rigidbody.rotation,
                targetRotation,
                enemyConfig.RotationSpeed *
                Time.fixedDeltaTime
            );

            Vector3 nextPosition = Vector3.MoveTowards(
                _rigidbody.position,
                targetPosition,
                enemyConfig.MoveSpeed *
                Time.fixedDeltaTime
            );

            _rigidbody.MoveRotation(nextRotation);
            _rigidbody.MovePosition(nextPosition);
        }

        private void StartAttack()
        {
            if (State != EnemyState.Chasing)
                return;

            SetState(EnemyState.Attacking);
        }

        public void ApplyAttackDamage()
        {
            if (State != EnemyState.Attacking)
                return;

            _targetDamageable?.TakeDamage(
                enemyConfig.AttackDamage
            );
        }

        public void FinishAttack()
        {
            if (State != EnemyState.Attacking)
                return;

            StartDying();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_target is null ||
                State == EnemyState.Dead ||
                _hasCollidedWithCar)
            {
                return;
            }

            if (collision.transform.root != _target.root)
                return;

            _hasCollidedWithCar = true;

            float collisionDamage =
                CalculateCollisionDamage(collision);

            _targetDamageable?.TakeDamage(
                collisionDamage
            );

            StopMovement();

            // Якщо атака вже почалась,
            // не перебиваємо її death animation.
            if (State == EnemyState.Attacking)
                return;

            if (State == EnemyState.Dying)
                return;

            StartDying();
        }

        private float CalculateCollisionDamage(
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

            bool hitFrontHalf =
                localContactPoint.z >=
                localColliderCenter.z;

            return hitFrontHalf
                ? enemyConfig.FrontCollisionDamage
                : enemyConfig.RearCollisionDamage;
        }

        private void OnDied()
        {
            StartDying();
        }

        private void StartDying()
        {
            if (State is EnemyState.Dying
                or EnemyState.Dead)
            {
                return;
            }

            SetState(EnemyState.Dying);
        }

        public void FinishDeath()
        {
            if (State == EnemyState.Dead)
                return;

            SetState(EnemyState.Dead);

            Destroy(gameObject);
        }

        private void SetState(EnemyState newState)
        {
            if (State == newState)
                return;

            State = newState;

            switch (newState)
            {
                case EnemyState.Idle:
                    enemyAnimator.SetIdle();
                    break;

                case EnemyState.Chasing:
                    enemyAnimator.SetRunning();
                    break;

                case EnemyState.Attacking:
                    StopMovement();
                    enemyAnimator.PlayAttack();
                    break;

                case EnemyState.Dying:
                    StopMovement();
                    enemyAnimator.PlayDeath();
                    break;

                case EnemyState.Dead:
                    StopMovement();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(newState),
                        newState,
                        null
                    );
            }
        }

        private void StopMovement()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
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
    }
}