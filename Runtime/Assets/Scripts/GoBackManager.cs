using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoBackManager : MonoBehaviour
{
    [SerializeField] Button _button;

    private void Start()
    {
        _button.onClick.AddListener(GoBack);
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
