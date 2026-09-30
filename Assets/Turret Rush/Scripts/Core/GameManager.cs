using System;
using Turret_Rush.Scripts.Combat;
using Turret_Rush.Scripts.Player;
using Turret_Rush.Scripts.Turret;
using TurretRush.Input;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Turret_Rush.Scripts.Core
{
    public sealed class GameManager : MonoBehaviour
    {
        private PlayerInputReader _inputReader;
        private CarMovement _carMovement;
        private Health _carHealth;
        private Weapon _weapon;

        public GameState State { get; private set; } =
            GameState.WaitingForStart;

        public event Action<GameState> StateChanged;

        [Inject]
        private void Construct(
            PlayerInputReader inputReader,
            CarMovement carMovement,
            Health carHealth,
            Weapon weapon)
        {
            _inputReader = inputReader;
            _carMovement = carMovement;
            _carHealth = carHealth;
            _weapon = weapon;
        }

        private void Start()
        {
            _inputReader.StartGamePressed += OnStartGamePressed;
            _carHealth.Died += OnCarDied;

            if (!GameSession.StartImmediately)
                return;

            GameSession.StartImmediately = false;

            StartGame();
        }

        private void OnDestroy()
        {
            if (_inputReader is not null)
                _inputReader.StartGamePressed -= OnStartGamePressed;

            if (_carHealth is not null)
                _carHealth.Died -= OnCarDied;
        }

        private void OnStartGamePressed()
        {
            switch (State)
            {
                case GameState.WaitingForStart:
                    StartGame();
                    break;

                case GameState.Won:
                case GameState.Lost:
                    RestartGame();
                    break;

                case GameState.Playing:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StartGame()
        {
            SetState(GameState.Playing);

            _carMovement.StartMoving();
            _weapon.StartFiring();
        }

        private void OnCarDied()
        {
            if (State != GameState.Playing)
                return;

            FinishGame(GameState.Lost);
        }

        public void Win()
        {
            if (State != GameState.Playing)
                return;

            FinishGame(GameState.Won);
        }

        private void FinishGame(GameState result)
        {
            _carMovement.StopMoving();
            _weapon.StopFiring();

            SetState(result);
        }

        private void RestartGame()
        {
            GameSession.StartImmediately = true;

            Scene currentScene =
                SceneManager.GetActiveScene();

            SceneManager.LoadScene(
                currentScene.buildIndex
            );
        }

        private void SetState(GameState state)
        {
            if (State == state)
                return;

            State = state;
            StateChanged?.Invoke(state);
        }
    }
}