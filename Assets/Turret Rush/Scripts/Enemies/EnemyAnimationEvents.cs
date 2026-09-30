using UnityEngine;

namespace Turret_Rush.Scripts.Enemies
{
    public sealed class EnemyAnimationEvents : MonoBehaviour
    {
        private EnemyController _enemyController;

        private void Awake()
        {
            _enemyController =
                GetComponentInParent<EnemyController>();
        }

        public void AttackHit()
        {
            _enemyController.ApplyAttackDamage();
        }

        public void AttackFinished()
        {
            _enemyController.FinishAttack();
        }
    }
}