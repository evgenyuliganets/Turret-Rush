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

        private static readonly int Death =
            Animator.StringToHash("Death");

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

        public void PlayAttack()
        {
            animator.SetTrigger(Attack);
        }

        public void PlayDeath()
        {
            animator.SetTrigger(Death);
        }
    }
}