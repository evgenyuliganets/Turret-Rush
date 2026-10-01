using Turret_Rush.Scripts.Input;
using Turret_Rush.Scripts.Turret;
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