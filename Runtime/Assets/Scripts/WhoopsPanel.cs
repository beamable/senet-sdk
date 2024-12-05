using UnityEngine;

public class WhoopsPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject _whoopsPanel;

    public void OpenWhoopsPanel()
    {
        _whoopsPanel.SetActive(true);
    }

    public void CloseWhoopsPanel()
    {
        _whoopsPanel.SetActive(false);
    }
}
