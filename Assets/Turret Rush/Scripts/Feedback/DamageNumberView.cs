using TMPro;
using UnityEngine;

namespace Turret_Rush.Scripts.Feedback
{
    public sealed class DamageNumberView : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private float lifetime = 0.7f;

        private Camera _worldCamera;
        private Transform _target;

        private Vector3 _spawnOffset;
        private Vector3 _anchorPosition;

        private float _moveSpeed;
        private float _moveOffset;
        private float _timeLeft;

        private Color _initialColor;

        public void Initialize(
            float damage,
            Camera worldCamera,
            Transform target,
            Vector3 spawnOffset,
            float speed)
        {
            _worldCamera = worldCamera;
            _target = target;

            _spawnOffset = spawnOffset;
            _moveSpeed = speed;

            _moveOffset = 0f;
            _timeLeft = lifetime;

            _initialColor = text.color;

            text.text =
                $"-{Mathf.RoundToInt(damage)}";

            _anchorPosition =
                target.position + spawnOffset;
        }

        private void Update()
        {
            if (_worldCamera is null)
                return;

            FaceCamera();
            UpdatePosition();
            Fade();

            if (_timeLeft <= 0f)
                Destroy(gameObject);
        }

        private void UpdatePosition()
        {
            if (_target)
            {
                _anchorPosition =
                    _target.position + _spawnOffset;
            }

            _moveOffset +=
                _moveSpeed * Time.deltaTime;

            transform.position =
                _anchorPosition -
                _worldCamera.transform.up *
                _moveOffset;
        }

        private void FaceCamera()
        {
            transform.rotation =
                _worldCamera.transform.rotation;
        }

        private void Fade()
        {
            _timeLeft -= Time.deltaTime;

            float alpha =
                Mathf.Clamp01(
                    _timeLeft / lifetime
                );

            Color color = _initialColor;
            color.a = alpha;

            text.color = color;
        }
    }
}