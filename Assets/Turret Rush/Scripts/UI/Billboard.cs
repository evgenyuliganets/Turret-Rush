using UnityEngine;

namespace Turret_Rush.Scripts.UI
{
    public sealed class Billboard : MonoBehaviour
    {
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_camera is null)
                return;

            transform.rotation = _camera.transform.rotation;
        }
    }
}