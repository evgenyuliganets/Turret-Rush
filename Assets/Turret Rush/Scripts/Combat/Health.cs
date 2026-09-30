using System;
using UnityEngine;

namespace Turret_Rush.Scripts.Combat
{
    public sealed class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;

        private float CurrentHealth { get; set; }

        public event Action<float> HealthChanged;
        public event Action Died;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (damage <= 0f || CurrentHealth <= 0f)
                return;

            CurrentHealth = Mathf.Max(
                CurrentHealth - damage,
                0f
            );

            HealthChanged?.Invoke(CurrentHealth);
            Debug.Log($"{gameObject.name} took {damage} damage. Current health: {CurrentHealth}");


            if (CurrentHealth <= 0f)
            {
                Died?.Invoke();
                Debug.Log($"{gameObject.name} has died.");
            }
        }
    }
}