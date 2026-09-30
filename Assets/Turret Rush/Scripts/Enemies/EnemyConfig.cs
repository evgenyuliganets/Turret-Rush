using UnityEngine;

namespace Turret_Rush.Scripts.Enemies
{
    [CreateAssetMenu(
        fileName = "EnemyConfig",
        menuName = "Turret Rush/Enemy Config")]
    public sealed class EnemyConfig : ScriptableObject
    {
        [field: SerializeField]
        public float MaxHealth { get; private set; } = 100f;

        [field: SerializeField]
        public float DetectionRange { get; private set; } = 12f;

        [field: SerializeField]
        public float AttackRange { get; private set; } = 1.5f;

        [field: SerializeField]
        public float MoveSpeed { get; private set; } = 3f;

        [field: SerializeField]
        public float RotationSpeed { get; private set; } = 8f;

        [field: SerializeField]
        public float AttackDamage { get; private set; } = 10f;

        [field: SerializeField]
        public float FrontCollisionDamage { get; private set; } = 10f;

        [field: SerializeField]
        public float RearCollisionDamage { get; private set; } = 20f;
    }
}