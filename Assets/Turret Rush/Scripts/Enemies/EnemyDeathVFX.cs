using System.Collections;
using UnityEngine;

namespace Turret_Rush.Scripts.Enemies
{
    public sealed class EnemyDeathVFX : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particles;

        public void Play()
        {
            particles.Play(true);
            StartCoroutine(DestroyWhenFinished());
        }

        private IEnumerator DestroyWhenFinished()
        {
            while (particles.IsAlive(true))
                yield return null;

            Destroy(gameObject);
        }
    }
}