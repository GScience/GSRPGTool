using UnityEngine;

namespace RPGTool
{
    [ExecuteInEditMode]
    public class SceneInfo : MonoBehaviour
    {
        public static SceneInfo sceneInfo;
        public InfoTilemap infoTilemap;

        private void Awake()
        {
            sceneInfo = this;
        }
    }
}