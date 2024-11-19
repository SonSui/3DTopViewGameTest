using UnityEngine;

namespace MatthewAssets
{


    public class CameraOrbit : MonoBehaviour
    {
        public Transform target; // The object around which the camera will rotate
        public float distance = 10.0f; // Distance from object
        public float orbitSpeed = 10.0f; // Orbit speed
        public Vector3 orbitAxis = Vector3.up; // Axis around which the camera will rotate

        private float currentAngle = 0.0f;

        void Update()
        {
            if (target)
            {
                // Increase angle based on time and orbit speed
                currentAngle += orbitSpeed * Time.deltaTime;

                // Calculate the new camera position
                float x = Mathf.Cos(currentAngle) * distance;
                float z = Mathf.Sin(currentAngle) * distance;
                Vector3 newPosition = new Vector3(x, 0, z) + target.position;

                // Keep camera height
                newPosition.y = transform.position.y;

                // Update camera position and rotation
                transform.position = newPosition;
                transform.LookAt(target);
            }
        }
    }
}

