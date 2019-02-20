using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGTool;
using UnityEngine;

namespace GSRPGTool.Physical
{
    public class TileCollider : MonoBehaviour
    {
        public GridTransform GridTransform { get; private set; }

        public List<Vector2Int> JointPositions { get; private set; } = new List<Vector2Int>();

        void Awake()
        {
            GridTransform = GetComponent<GridTransform>();
        }
        void Update()
        {
            if (GridTransform.IsMoving)
            {
                var posXBig = (int)Math.Ceiling(GridTransform.MovingFloatPos.x);
                var posXSmall = (int)Math.Floor(GridTransform.MovingFloatPos.x);
                var posYBig = (int)Math.Ceiling(GridTransform.MovingFloatPos.y);
                var posYSmall = (int)Math.Floor(GridTransform.MovingFloatPos.y);

                var newJointPosition = new List<Vector2Int>();

                for (var x= posXSmall; x<= posXBig;++x)
                for (var y = posYSmall; y <= posYBig; ++y)
                    newJointPosition.Add(new Vector2Int(x, y));

                UpdateJointPos(newJointPosition);
            }
            else
                UpdateJointPos(new List<Vector2Int> {GridTransform.position});
        }

        private void UpdateJointPos(List<Vector2Int> newJointPosition)
        {
            //新的站位
            foreach (var pos in newJointPosition)
                if (!JointPositions.Contains(pos))
                    SceneInfo.sceneInfo.movementInfoTilemap.SetMovable(pos, false);

            //旧的站位
            foreach (var pos in JointPositions)
                if (!newJointPosition.Contains(pos))
                    SceneInfo.sceneInfo.movementInfoTilemap.SetMovable(pos, true);

            JointPositions = newJointPosition;
        }
    }
}
