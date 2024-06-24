using TMPro;
using UnityEngine;

public class WinnerCountdown : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _hours;
    [SerializeField]
    private TextMeshProUGUI _minutes;
    [SerializeField]
    private TextMeshProUGUI _seconds;

    private void Update()
    {
        var countdownTime = TournamentManager.instance.countdownTime;
        _hours.text = $"{countdownTime.Hours:D2} :";
        _minutes.text = $"{countdownTime.Minutes:D2} :";
        _seconds.text = $"{countdownTime.Seconds:D2} :";
    }
}