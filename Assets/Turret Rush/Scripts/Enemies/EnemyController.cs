using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Turret_Rush.Scripts.Combat;
using Turret_Rush.Scripts.Core;
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
        private GameManager _gameManager;

        private Transform _vfxParent;

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
            if (!_gameManager.IsPlaying)
                return;

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

                case EnemyState.Dying:
                case EnemyState.Dead:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Initialize(Transform target, GameManager gameManager, Transform vfxParent)
        {
            movement.Initialize(target, enemyConfig);
            combat.Initialize(target, enemyConfig);
            _gameManager = gameManager;
            _vfxParent = vfxParent;

            _gameManager.StateChanged += OnGameStateChanged;


            movement.StartIdle();
        }

        private void OnDestroy()
        {
            if (_gameManager is not null)
                _gameManager.StateChanged -= OnGameStateChanged;
        }

        private void OnCollidedWithCar(bool hitFrontHalf)
        {
            if (hitFrontHalf)
            {
                Die();
                return;
            }

            StartRearAttackDeath();
        }

        private void OnDied()
        {
            Die();
        }

        private void SetState(EnemyState newState)
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

            Die();
        }

        private void Die()
        {
            if (State == EnemyState.Dead)
                return;

            if (State != EnemyState.Dying)
                SetState(EnemyState.Dying);

            Instantiate(
                deathVfxPrefab,
                _collider.bounds.center,
                Quaternion.identity,
                _vfxParent
            );

            SetState(EnemyState.Dead);

            Destroy(gameObject);
        }

        private void OnGameStateChanged(
            GameState gameState)
        {
            if (gameState != GameState.Playing)
            {
                SetState(EnemyState.Idle);
            }
        }
    }
}