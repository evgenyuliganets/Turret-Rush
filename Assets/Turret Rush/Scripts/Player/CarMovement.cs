using UnityEngine;

namespace Turret_Rush.Scripts.Player
{
    public sealed class CarMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 6f;

        private bool _isMoving;

        public void StartMoving()
        {
            _isMoving = true;
        }

        public void StopMoving()
        {
            _isMoving = false;
        }

        private void Update()
        {
            if (!_isMoving)
                return;

            transform.position +=
                transform.forward * (speed * Time.deltaTime);
        }
        
        public bool IsMoving => _isMoving;
    }
}