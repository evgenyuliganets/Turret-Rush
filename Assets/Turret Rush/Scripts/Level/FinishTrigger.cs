using Turret_Rush.Scripts.Core;
using UnityEngine;

namespace Turret_Rush.Scripts.Level
{
    public sealed class FinishTrigger : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            gameManager.Win();
        }
    }
}