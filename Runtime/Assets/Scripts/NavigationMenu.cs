using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _logout;
    
    public void OpenLogout()
    {
        _logout.SetActive(true);
    }

    public void CloseLogout()
    {
        _logout.SetActive(false);
    }
}
