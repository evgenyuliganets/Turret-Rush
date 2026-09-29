using UnityEngine;

namespace Turret_Rush.Scripts.Combat
{
    [RequireComponent(typeof(Health))]
    public class EnemyController : MonoBehaviour
    {
        private Health _health;


        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.Died += OnDied;
        }


        private void OnDisable()
        {
            _health.Died -= OnDied;
        }

        private void OnDied()
        {
            Destroy(gameObject);
        }
    }
}