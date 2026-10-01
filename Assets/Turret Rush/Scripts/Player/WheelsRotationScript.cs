using UnityEngine;

namespace Turret_Rush.Scripts.Player
{
    public class WheelsRotationScript : MonoBehaviour
    {
        [SerializeField] private Transform[] wheels;
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private CarMovement carMovement;

        private void Update()
        {
            RotateWheels();
        }

        private void RotateWheels()
        {
            if (!carMovement.IsMoving)
                return;

            float rotation =
                rotationSpeed * Time.deltaTime;

            foreach (Transform wheel in wheels)
            {
                wheel.Rotate(
                    Vector3.right,
                    rotation,
                    Space.Self
                );
            }
        }
    }
}