using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGTool;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RPGTool
{
    public class SceneInfo : MonoBehaviour
    {
        public static SceneInfo sceneInfo;
        public MovementInfoTilemap movementInfoTilemap;

        void Awake()
        {
            sceneInfo = this;
        }
    }
}
