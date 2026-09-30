using UnityEngine;

namespace Turret_Rush.Scripts.UI
{
    public sealed class LevelProgressView : MonoBehaviour
    {
        [SerializeField] private Transform car;
        [SerializeField] private Transform finishPoint;
        [SerializeField] private RectTransform fill;

        private Vector3 _startPosition;
        private Vector3 _levelDirection;
        private float _levelLength;

        private void Start()
        {
            _startPosition = car.position;

            Vector3 toFinish =
                finishPoint.position - _startPosition;

            _levelLength = toFinish.magnitude;
            _levelDirection = toFinish.normalized;

            SetProgress(0f);
        }

        private void Update()
        {
            Vector3 travelled =
                car.position - _startPosition;

            float travelledDistance =
                Vector3.Dot(
                    travelled,
                    _levelDirection
                );

            float progress =
                Mathf.Clamp01(
                    travelledDistance / _levelLength
                );

            SetProgress(progress);
        }

        private void SetProgress(float progress)
        {
            fill.anchorMax = new Vector2(
                1f,
                progress
            );
        }
    }
}