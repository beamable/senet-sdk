using UnityEngine;
using UnityEngine.UI;

public class GameLogo : MonoBehaviour
{
    void Start()
    {
        var settings = Resources.Load<SenetSettings>("SenetSettings");
        var gameLogo = settings.gameLogo;

        gameObject.GetComponent<Image>().sprite = gameLogo;
    }
}
