using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RPGTool.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    [ExecuteInEditMode]
    public class PixelCamera : MonoBehaviour
    {
        //单位移动距离，与屏幕大小有关
        private Vector2 _unitMovement;

        public UnityEngine.Camera Camera { get; private set; }

        void Awake()
        {
            Camera = GetComponent<UnityEngine.Camera>();

            var camerapixelRect = Camera.pixelRect;
            var cameraRect =
                Camera.ScreenToWorldPoint(new Vector3(camerapixelRect.width, camerapixelRect.height)) -
                Camera.ScreenToWorldPoint(Vector3.zero);

            _unitMovement = new Vector2(cameraRect.x / camerapixelRect.width, cameraRect.y / camerapixelRect.height);
        }
        void LateUpdate()
        {
            transform.position = new Vector3(
                transform.position.x - transform.position.x % _unitMovement.x,
                transform.position.y - transform.position.y % _unitMovement.y,
                transform.position.z);
        }
    }
}
