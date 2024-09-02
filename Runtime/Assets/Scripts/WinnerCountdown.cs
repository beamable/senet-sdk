using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinnerCountdown : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _hours;
    [SerializeField]
    private TextMeshProUGUI _minutes;
    [SerializeField]
    private TextMeshProUGUI _seconds;

    [SerializeField]
    private Text _timer;

    private void Update()
    {
        var countdownTime = TournamentManager.instance.countdownTime;
        if (_timer)
        {
            _timer.text = $"NEXT REWARD IN {countdownTime.Days:D2}D : {countdownTime.Hours:D2}H : {countdownTime.Minutes:D2}M";
        }
        else
        {
            _hours.text = $"{countdownTime.Hours:D2} :";
            _minutes.text = $"{countdownTime.Minutes:D2} :";
            _seconds.text = $"{countdownTime.Seconds:D2} :";
        }
    }
}