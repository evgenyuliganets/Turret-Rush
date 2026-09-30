using Turret_Rush.Scripts.Core;
using UnityEngine;

namespace Turret_Rush.Scripts.UI
{
    public sealed class GameStateView : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        [SerializeField] private GameObject startOverlay;
        [SerializeField] private ResultOverlayView resultOverlay;
        [SerializeField] private GameObject hudOverlay;

        private void OnEnable()
        {
            gameManager.StateChanged += OnStateChanged;

            UpdateView(gameManager.State);
        }

        private void OnDisable()
        {
            gameManager.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(GameState state)
        {
            UpdateView(state);
        }

        private void UpdateView(GameState state)
        {
            hudOverlay.SetActive(
                state == GameState.Playing
            );
            
            startOverlay.SetActive(
                state == GameState.WaitingForStart
            );

            switch (state)
            {
                case GameState.Won:
                    resultOverlay.ShowWin();
                    break;

                case GameState.Lost:
                    resultOverlay.ShowLose();
                    break;

                default:
                    resultOverlay.Hide();
                    break;
            }
        }
    }
}