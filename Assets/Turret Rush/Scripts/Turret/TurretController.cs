using TurretRush.Input;
using UnityEngine;
using VContainer;

namespace Turret_Rush.Scripts.Turret
{
    public sealed class TurretController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float rotationSpeed = 10f;

        private Plane _aimPlane;
        private PlayerInputReader _inputReader;

        [Inject]
        public void Initialize(PlayerInputReader inputReader)
        {
            _inputReader = inputReader;
        }

        private void Awake()
        {
            _aimPlane = new Plane(
                Vector3.up,
                new Vector3(0f, 0.5f, 0f)
            );
        }

        private void Update()
        {
            Vector2 screenPosition = _inputReader.AimPosition;

            Ray ray = mainCamera.ScreenPointToRay(screenPosition);

            if (!_aimPlane.Raycast(ray, out float distance))
                return;

            Vector3 targetPosition = ray.GetPoint(distance);

            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}