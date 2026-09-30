using System;
using Turret_Rush.Scripts.Combat;
using Turret_Rush.Scripts.Player;
using Turret_Rush.Scripts.Turret;
using TurretRush.Input;
using UnityEngine;

namespace Turret_Rush.Scripts.Core
{
    public sealed class GameManager : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private CarMovement carMovement;
        [SerializeField] private Weapon weapon;
        [SerializeField] private Health carHealth;

        [SerializeField] private CarController carController;

        private void Awake()
        {
            carController.Initialize(inputReader);
        }

        public GameState State { get; private set; } = GameState.WaitingForStart;

        public event Action<GameState> StateChanged;

        private void OnEnable()
        {
            inputReader.StartGamePressed += OnStartGamePressed;
            carHealth.Died += OnCarDied;
        }

        private void OnDisable()
        {
            inputReader.StartGamePressed -= OnStartGamePressed;
            carHealth.Died -= OnCarDied;
        }

        private void OnStartGamePressed()
        {
            if (State != GameState.WaitingForStart)
                return;

            SetState(GameState.Playing);

            carMovement.StartMoving();
            weapon.StartFiring();
        }

        private void OnCarDied()
        {
            if (State != GameState.Playing)
                return;

            carMovement.StopMoving();
            weapon.StopFiring();

            SetState(GameState.Lost);
        }


        public void Win()
        {
            if (State != GameState.Playing)
                return;

            carMovement.StopMoving();
            weapon.StopFiring();

            SetState(GameState.Won);
        }


        private void SetState(GameState state)
        {
            State = state;
            StateChanged?.Invoke(state);
        }
    }
}