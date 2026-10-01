using System;
using UnityEngine;

namespace Turret_Rush.Scripts.Combat
{
    public sealed class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;

        public float CurrentHealth { get; private set; }
        public float MaxHealth => maxHealth;

        public event Action<float> HealthChanged;
        public event Action Died;

        public event Action<float> Damaged;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void Initialize(float newMaxHealth)
        {
            maxHealth = newMaxHealth;
            CurrentHealth = newMaxHealth;

            HealthChanged?.Invoke(CurrentHealth);
        }

        public void TakeDamage(float damage)
        {
            if (damage <= 0f || CurrentHealth <= 0f)
                return;

            float previousHealth = CurrentHealth;

            CurrentHealth = Mathf.Max(
                0f,
                CurrentHealth - damage
            );

            float actualDamage =
                previousHealth - CurrentHealth;

            Damaged?.Invoke(actualDamage);

            HealthChanged?.Invoke(
                CurrentHealth
            );

            if (CurrentHealth <= 0f)
                Died?.Invoke();
        }
    }
}