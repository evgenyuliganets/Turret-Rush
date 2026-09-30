using Gilzoide.LottiePlayer;
using TMPro;
using UnityEngine;

namespace Turret_Rush.Scripts.UI
{
    public sealed class ResultOverlayView : MonoBehaviour
    {
        [SerializeField] private ImageLottiePlayer lottiePlayer;
        [SerializeField] private TMP_Text resultText;

        [SerializeField] private LottieAnimationAsset winAnimation;
        [SerializeField] private LottieAnimationAsset loseAnimation;

        public void ShowWin()
        {
            Show(
                "YOU WIN",
                winAnimation
            );
        }

        public void ShowLose()
        {
            Show(
                "YOU LOSE",
                loseAnimation
            );
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Show(
            string text,
            LottieAnimationAsset animation)
        {
            resultText.text = text;

            gameObject.SetActive(true);

            lottiePlayer.SetAnimationAsset(animation);
            lottiePlayer.Play();
        }
    }
}