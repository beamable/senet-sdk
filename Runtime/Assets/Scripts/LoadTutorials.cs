using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTutorials : MonoBehaviour
{
    public void LoadTutorialScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        string sceneName = currentScene.name;
        switch(sceneName)
        {
            case "SenetTutorial1":
                SceneManager.LoadSceneAsync("SenetTutorial2");
                break;
            case "SenetTutorial2":
                SceneManager.LoadSceneAsync("SenetTutorial3");
                break;
            default:
                SceneManager.LoadSceneAsync("SenetTutorial1");
                break;
        }
    }
}
