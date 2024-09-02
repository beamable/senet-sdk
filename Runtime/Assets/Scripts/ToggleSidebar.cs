using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSidebar : MonoBehaviour
{
    [SerializeField]
    private GameObject _sidebar;

    public void TurnOn()
    {
        _sidebar.SetActive(true);
    }

    public void TurnOff()
    {
        _sidebar.SetActive(false);
    }
}
