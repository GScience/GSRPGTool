using UnityEngine;

namespace RPGTool
{
    public class SceneInfo : MonoBehaviour
    {
        public static SceneInfo sceneInfo;
        public MovementInfoTilemap movementInfoTilemap;

        private void Awake()
        {
            sceneInfo = this;
        }
    }
}