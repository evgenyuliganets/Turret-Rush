using Turret_Rush.Scripts.Combat;
using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Turret_Rush.Scripts.Enemies
{
    [RequireComponent(typeof(Health))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float detectionRange = 12f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float rotationSpeed = 8f;

        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackInterval = 1f;

        [SerializeField] private Transform debugTarget;

        private Health _health;
        private Transform _target;
        private IDamageable _targetDamageable;

        private CancellationTokenSource _attackCancellation;

        public EnemyState State { get; private set; } = EnemyState.Idle;


        private void Awake()
        {
            _health = GetComponent<Health>();
        }


        private void Start()
        {
            if (debugTarget != null)
                Initialize(debugTarget);
        }

        private void OnEnable()
        {
            _health.Died += OnDied;
        }


        private void OnDisable()
        {
            _health.Died -= OnDied;
            StopAttacking();
        }

        private void Update()
        {
            if (_target is null || State == EnemyState.Dead)
                return;

            var distance = Vector3.Distance(transform.position, _target.position);


            switch (State)
            {
                case EnemyState.Idle:
                    if (distance <= detectionRange)
                    {
                        State = EnemyState.Chasing;
                    }

                    break;

                case EnemyState.Chasing:
                    if (distance <= attackRange)
                    {
                        State = EnemyState.Attacking;
                        StartAttacking();
                        break;
                    }

                    ChaseTarget();
                    break;

                case EnemyState.Attacking:
                    if (distance > attackRange)
                    {
                        StopAttacking();
                        State = EnemyState.Chasing;
                    }

                    break;
                
                case EnemyState.Dead:
                    return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDied()
        {
            State = EnemyState.Dead;
            StopAttacking();
            Destroy(gameObject);
        }

        private void ChaseTarget()
        {
            var targetPosition = _target.position;
            targetPosition.y = transform.position.y;

            var direction =
                targetPosition - transform.position;

            if (direction.sqrMagnitude < 0.001f)
                return;

            var targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }


        private void StartAttacking()
        {
            if (_attackCancellation != null)
                return;

            _attackCancellation = new CancellationTokenSource();

            AttackLoopAsync(
                _attackCancellation.Token
            ).Forget();
        }

        private void StopAttacking()
        {
            if (_attackCancellation == null)
                return;

            _attackCancellation.Cancel();
            _attackCancellation.Dispose();
            _attackCancellation = null;
        }

        private async UniTask AttackLoopAsync(
            CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _targetDamageable?.TakeDamage(attackDamage);

                await UniTask.Delay(
                    TimeSpan.FromSeconds(attackInterval),
                    cancellationToken: cancellationToken
                );
            }
        }


        public void Initialize(Transform target)
        {
            _target = target;

            _targetDamageable =
                target.GetComponent<IDamageable>();
        }
    }
}