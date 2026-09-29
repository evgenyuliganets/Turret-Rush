using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TurretRush.Input
{
    public sealed class PlayerInputReader : MonoBehaviour
    {
        [SerializeField] private InputActionReference startGameAction;
        [SerializeField] private InputActionReference aimPositionAction;

        public event Action StartGamePressed;

        public Vector2 AimPosition =>
            aimPositionAction.action.ReadValue<Vector2>();

        private void OnEnable()
        {
            startGameAction.action.Enable();
            aimPositionAction.action.Enable();

            startGameAction.action.performed += OnStartGamePerformed;
        }

        private void OnDisable()
        {
            startGameAction.action.performed -= OnStartGamePerformed;

            startGameAction.action.Disable();
            aimPositionAction.action.Disable();
        }

        private void OnStartGamePerformed(InputAction.CallbackContext context)
        {
            StartGamePressed?.Invoke();
        }
    }
}