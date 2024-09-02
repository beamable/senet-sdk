using UnityEngine;
using UnityEngine.SceneManagement;

public class GetStarted : MonoBehaviour
{
    public void GettingStarted()
    {
        PlayerPrefs.SetInt("IsFirstLaunch", 0);

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            SceneManager.LoadScene("SenetNoInternet");
        }
        else
        {
            SceneManager.LoadScene("SenetSignUp");
        }
    }
}
