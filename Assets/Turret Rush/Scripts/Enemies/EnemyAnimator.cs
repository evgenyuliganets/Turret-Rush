    using UnityEngine;

namespace Turret_Rush.Scripts.Enemies
{
    public sealed class EnemyAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private static readonly int Speed =
            Animator.StringToHash("Speed");

        private static readonly int Attack =
            Animator.StringToHash("Attack");

        private static readonly int AttackSpeed =
            Animator.StringToHash("AttackSpeed");

        public void SetIdle()
        {
            animator.SetFloat(Speed, 0f);
        }

        public void SetWalking()
        {
            animator.SetFloat(Speed, 0.5f);
        }

        public void SetRunning()
        {
            animator.SetFloat(Speed, 1f);
        }

        public void PlayAttack(float speed = 1f)
        {
            animator.SetFloat(AttackSpeed, speed);
            animator.SetTrigger(Attack);
        }
    }
}