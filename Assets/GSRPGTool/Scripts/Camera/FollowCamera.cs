using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RPGTool.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class FollowCamera : MonoBehaviour
    {
        public UnityEngine.Camera Camera { get; private set; }
        public Transform followTo;

        void Awake()
        {
            Camera = GetComponent<UnityEngine.Camera>();
        }

        private Vector3 _currentVelocity;

        void Update()
        {
            transform.position = new Vector3(followTo.position.x, followTo.position.y, transform.position.z);
        }
    }
}
