using UnityEngine;
using Random = UnityEngine.Random;

namespace Turret_Rush.Scripts.Enemies
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private EnemyConfig enemyConfig;
        [SerializeField] private EnemyAnimator enemyAnimator;

        private Rigidbody _rigidbody;
        private Transform _target;

        private Vector3 _spawnPosition;
        private Vector3 _idleTargetPosition;

        private float _idleWaitUntil;
        private bool _isWandering;

        private MovementMode _mode = MovementMode.Stopped;

        public bool IsTargetDetected =>
            _target != null &&
            Vector3.Distance(
                _rigidbody.position,
                _target.position
            ) <= enemyConfig.DetectionRange;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            switch (_mode)
            {
                case MovementMode.Idle:
                    UpdateIdleMovement();
                    break;

                case MovementMode.Chasing:
                    if (_target != null)
                    {
                        MoveTowards(
                            _target.position,
                            enemyConfig.MoveSpeed
                        );
                    }

                    break;

                case MovementMode.Stopped:
                    break;
            }
        }

        public void Initialize(Transform target)
        {
            _target = target;
            _spawnPosition = _rigidbody.position;
        }

        public void StartIdle()
        {
            _mode = MovementMode.Idle;
            _isWandering = false;

            StopVelocity();
            enemyAnimator.SetIdle();

            BeginIdleWait();
        }

        public void StartChasing()
        {
            _mode = MovementMode.Chasing;
            _isWandering = false;

            enemyAnimator.SetRunning();
        }

        public void Stop()
        {
            _mode = MovementMode.Stopped;
            _isWandering = false;

            StopVelocity();
        }

        private void UpdateIdleMovement()
        {
            if (!_isWandering)
            {
                if (Time.time >= _idleWaitUntil)
                    StartWandering();

                return;
            }

            float distance = Vector3.Distance(
                _rigidbody.position,
                _idleTargetPosition
            );

            if (distance <= enemyConfig.IdlePointReachedDistance)
            {
                _isWandering = false;

                StopVelocity();
                enemyAnimator.SetIdle();

                BeginIdleWait();

                return;
            }

            MoveTowards(
                _idleTargetPosition,
                enemyConfig.IdleMoveSpeed
            );
        }

        private void StartWandering()
        {
            Vector2 randomPoint =
                Random.insideUnitCircle *
                enemyConfig.IdleWanderRadius;

            _idleTargetPosition = new Vector3(
                _spawnPosition.x + randomPoint.x,
                _spawnPosition.y,
                _spawnPosition.z + randomPoint.y
            );

            _isWandering = true;

            enemyAnimator.SetWalking();
        }

        private void BeginIdleWait()
        {
            _idleWaitUntil =
                Time.time +
                Random.Range(
                    enemyConfig.IdleWaitMin,
                    enemyConfig.IdleWaitMax
                );
        }

        private void MoveTowards(
            Vector3 targetPosition,
            float speed)
        {
            targetPosition.y = _rigidbody.position.y;

            Vector3 direction =
                targetPosition - _rigidbody.position;

            if (direction.sqrMagnitude < 0.001f)
            {
                StopVelocity();
                return;
            }

            direction.Normalize();

            _rigidbody.linearVelocity = new Vector3(
                direction.x * speed,
                0f,
                direction.z * speed
            );

            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            Quaternion nextRotation =
                Quaternion.Slerp(
                    _rigidbody.rotation,
                    targetRotation,
                    enemyConfig.RotationSpeed *
                    Time.fixedDeltaTime
                );

            _rigidbody.MoveRotation(nextRotation);
        }

        private void StopVelocity()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        private enum MovementMode
        {
            Stopped,
            Idle,
            Chasing
        }
    }
}