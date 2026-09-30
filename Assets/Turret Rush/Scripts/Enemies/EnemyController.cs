using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Turret_Rush.Scripts.Combat;
using UnityEngine;

namespace Turret_Rush.Scripts.Enemies
{
    [RequireComponent(typeof(Health))]
    public sealed class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyConfig enemyConfig;

        [SerializeField] private EnemyMovement movement;

        [SerializeField] private EnemyCombat combat;

        [SerializeField] private EnemyAnimator enemyAnimator;

        [SerializeField] private ParticleSystem deathVfxPrefab;

        private Health _health;
        private Collider _collider;

        public EnemyState State { get; private set; } =
            EnemyState.Idle;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _collider = GetComponent<Collider>();

            _health.Initialize(
                enemyConfig.MaxHealth
            );
        }

        private void OnEnable()
        {
            _health.Died += OnDied;
            combat.CollidedWithCar += OnCollidedWithCar;
        }

        private void OnDisable()
        {
            _health.Died -= OnDied;
            combat.CollidedWithCar -= OnCollidedWithCar;
        }

        private void Update()
        {
            switch (State)
            {
                case EnemyState.Idle:
                    if (movement.IsTargetDetected)
                        SetState(EnemyState.Chasing);

                    break;

                case EnemyState.Chasing:
                    if (combat.IsTargetInAttackRange &&
                        combat.IsBehindTarget)
                    {
                        StartRearAttackDeath();
                    }

                    break;

                case EnemyState.Attacking:
                case EnemyState.Dying:
                case EnemyState.Dead:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Initialize(Transform target)
        {
            movement.Initialize(target);
            combat.Initialize(target);

            movement.StartIdle();
        }

        public void ApplyAttackDamage()
        {
            if (State != EnemyState.Attacking)
                return;

            combat.ApplyAttackDamage();
        }

        public void FinishAttack()
        {
            if (State != EnemyState.Attacking)
                return;

            StartDying();
        }

        private void OnCollidedWithCar(bool hitFrontHalf)
        {
            if (hitFrontHalf)
            {
                StartDying();
                return;
            }

            StartRearAttackDeath();
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

            Instantiate(
                deathVfxPrefab,
                _collider.bounds.center,
                Quaternion.identity
            );

            SetState(EnemyState.Dead);

            Destroy(gameObject);
        }

        private void SetState(
            EnemyState newState)
        {
            if (State == newState)
                return;

            State = newState;

            switch (newState)
            {
                case EnemyState.Idle:
                    movement.StartIdle();
                    break;

                case EnemyState.Chasing:
                    movement.StartChasing();
                    break;

                case EnemyState.Attacking:
                    movement.Stop();
                    combat.BeginAttack();
                    enemyAnimator.PlayAttack(
                        enemyConfig.AttackAnimationSpeed
                    );
                    break;

                case EnemyState.Dying:
                case EnemyState.Dead:
                    movement.Stop();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(newState),
                        newState,
                        null
                    );
            }
        }


        private void StartRearAttackDeath()
        {
            if (State is EnemyState.Dying or EnemyState.Dead)
                return;

            SetState(EnemyState.Dying);

            combat.BeginAttack();
            combat.ApplyAttackDamage();

            enemyAnimator.PlayAttack(
                enemyConfig.AttackAnimationSpeed
            );

            FinishRearAttackDeathAsync(
                destroyCancellationToken
            ).Forget();
        }

        private async UniTask FinishRearAttackDeathAsync(
            CancellationToken cancellationToken)
        {
            bool cancelled = await UniTask
                .Delay(
                    TimeSpan.FromSeconds(
                        enemyConfig.RearHitAnimationDuration
                    ),
                    cancellationToken: cancellationToken
                )
                .SuppressCancellationThrow();

            if (cancelled)
                return;

            FinishDeath();
        }

        private void FinishDeath()
        {
            if (State == EnemyState.Dead)
                return;

            Instantiate(
                deathVfxPrefab,
                _collider.bounds.center,
                Quaternion.identity
            );

            SetState(EnemyState.Dead);

            Destroy(gameObject);
        }
    }
}