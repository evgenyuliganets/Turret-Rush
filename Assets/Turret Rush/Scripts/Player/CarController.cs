using Turret_Rush.Scripts.Turret;
using TurretRush.Input;
using UnityEngine;
using VContainer;

namespace Turret_Rush.Scripts.Player
{
    public sealed class CarController : MonoBehaviour
    {
        [SerializeField] private TurretController turretController;

        [Inject]
        public void Initialize(PlayerInputReader inputReader)
        {
            turretController.Initialize(
                inputReader
            );
        }
    }
}