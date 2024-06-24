using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEnabled : MonoBehaviour
{
    [SerializeField]
    private Button button;

    [SerializeField]
    private long _cooldownSeconds = 15;

    void Update()
    {
        var timeUnit = 10000000;
        var ticks = _cooldownSeconds * timeUnit;
        var lastSpunDate = long.Parse(PlayerPrefs.GetString("LastDateSpun", "0"));

        if (DateTime.Now.Ticks - ticks > lastSpunDate)
            button.interactable = true;
        else
            button.interactable = false;
    }
}
