using UnityEngine;

namespace RPGTool.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class FollowCamera : MonoBehaviour
    {
        private Vector3 _currentVelocity;
        public Transform followTo;
        public UnityEngine.Camera Camera { get; private set; }

        private void Awake()
        {
            Camera = GetComponent<UnityEngine.Camera>();
        }

        private void Update()
        {
            transform.position = new Vector3(followTo.position.x, followTo.position.y, transform.position.z);
        }
    }
}