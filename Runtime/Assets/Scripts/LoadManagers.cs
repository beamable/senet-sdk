using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadManagers : MonoBehaviour
{
    void Start()
    {
        TournamentManager.instance.Refresh();
        CurrencyManager.instance.Refresh();
    }

}
