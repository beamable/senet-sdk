using UnityEngine;

public class ToggleSidebar : MonoBehaviour
{
    [SerializeField]
    private GameObject _sidebar;

    public void TurnOn()
    {
        if (_sidebar.activeSelf == true)
        {

            _sidebar.SetActive(false);
        }
        else
        {
           _sidebar.SetActive(true);
        }
    }

    public void TurnOff()
    {
        _sidebar.SetActive(false);
    }
}
