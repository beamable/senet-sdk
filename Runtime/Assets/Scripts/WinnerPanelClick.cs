using UnityEngine;
using UnityEngine.SceneManagement;

public class WinnerPanelClick : MonoBehaviour
{
    public void Click()
    {
        SceneManager.LoadSceneAsync("SenetCongratulate");
    }
}
