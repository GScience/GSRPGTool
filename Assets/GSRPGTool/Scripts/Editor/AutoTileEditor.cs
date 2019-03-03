using RPGTool.Editor;
using RPGTool.Tiles;
using UnityEditor;
using UnityEngine;

namespace Assets.GSRPGTool.Scripts.Editor
{
    [CustomEditor(typeof(AutoTile))]
    public class AutoTileEditor : UnityEditor.Editor
    {
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            var autoTile = (AutoTile) target;

            if (autoTile == null)
                return null;

            return WindowCreateTileFromAutoTile.GetIcon(Sprite.Create(autoTile.autoTileTexture,
                autoTile.autoTileSpriteRect, new Vector2(0.5f, 0.5f)));
        }
    }
}