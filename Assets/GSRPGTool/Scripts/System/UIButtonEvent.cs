using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGTool.Save
{
    public class UIButtonEvent : MonoBehaviour
    {
        public void StartGame()
        {
            var sceneId = SaveManager.saveManager.CurrentSceneId;

            if (sceneId == 0)
                sceneId = 1;
            SceneManager.LoadScene(sceneId);
        }
    }
}