using Turret_Rush.Scripts.Player;
using TurretRush.Input;
using UnityEngine;

namespace Turret_Rush.Scripts.Core
{
    public sealed class GameManager : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private CarMovement carMovement;

        private bool _isGameStarted;

        private void OnEnable()
        {
            inputReader.StartGamePressed += OnStartGamePressed;
        }

        private void OnDisable()
        {
            inputReader.StartGamePressed -= OnStartGamePressed;
        }

        private void OnStartGamePressed()
        {
            if (_isGameStarted)
                return;

            _isGameStarted = true;
            carMovement.StartMoving();
        }
    }
}