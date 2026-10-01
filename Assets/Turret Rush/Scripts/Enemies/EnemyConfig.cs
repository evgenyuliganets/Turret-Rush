using UnityEngine;

namespace Turret_Rush.Scripts.Enemies
{
    [CreateAssetMenu(
        fileName = "EnemyConfig",
        menuName = "Turret Rush/Enemy Config")]
    public sealed class EnemyConfig : ScriptableObject
    {
        [field: SerializeField] public float MaxHealth { get; private set; } = 100f;

        [field: SerializeField] public float DetectionRange { get; private set; } = 12f;

        [field: SerializeField] public float AttackRange { get; private set; } = 0.8f;

        [field: SerializeField] public float MoveSpeed { get; private set; } = 3f;

        [field: SerializeField] public float RotationSpeed { get; private set; } = 8f;
        
        [field: SerializeField] public float FrontCollisionDamage { get; private set; } = 10f;

        [field: SerializeField] public float RearAttackDamage { get; private set; } = 20f;

        [field: SerializeField] public float IdleMoveSpeed { get; private set; } = 1f;

        [field: SerializeField] public float IdleWanderRadius { get; private set; } = 2.5f;

        [field: SerializeField] public float IdlePointReachedDistance { get; private set; } = 0.2f;

        [field: SerializeField] public float IdleWaitMin { get; private set; } = 0.5f;

        [field: SerializeField] public float IdleWaitMax { get; private set; } = 1.5f;


        [field: SerializeField, Min(0f)] public float RearHitAnimationDuration { get; private set; } = 0.05f;


        [field: SerializeField, Min(0.1f)] public float AttackAnimationSpeed { get; private set; } = 2.5f;
    }
}