using Turret_Rush.Scripts.Combat;
using UnityEngine;

namespace Turret_Rush.Scripts.UI
{
    public sealed class HealthBarView : MonoBehaviour
    {
        [SerializeField] private RectTransform fill;

        private Health _health;

        private void Awake()
        {
            _health = GetComponentInParent<Health>();
        }

        private void OnEnable()
        {
            _health.HealthChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            _health.HealthChanged -= OnHealthChanged;
        }

        private void Start()
        {
            UpdateBar(_health.CurrentHealth);
        }

        private void OnHealthChanged(float currentHealth)
        {
            UpdateBar(currentHealth);
        }

        private void UpdateBar(float currentHealth)
        {
            float normalizedHealth = Mathf.Clamp01(
                currentHealth / _health.MaxHealth
            );

            fill.anchorMax = new Vector2(
                normalizedHealth,
                fill.anchorMax.y
            );
        }
    }
}