using Beamable;
using Beamable.Server.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TournamentButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timer;
    [SerializeField]
    private TextMeshProUGUI _playersCount;
    [SerializeField]
    private GameObject _playNow;
    [SerializeField]
    private GameObject _entryCost;
    [SerializeField]
    private TextMeshProUGUI _rankNumber;
    [SerializeField]
    private TextMeshProUGUI _prizePool;

    void Start()
    {
        if (TournamentManager.instance)
        {
            HandleComponentUpdate(TournamentManager.instance.runningTournament);
            TournamentManager.instance.OnRunningTournamentChanged += HandleComponentUpdate;
        }
    }

    void Update()
    {
        var countdownTime = TournamentManager.instance.countdownTime;
        _timer.text = $"Next In {countdownTime.Days:D2}D : {countdownTime.Hours:D2}H : {countdownTime.Minutes:D2}M";
    }

    private void HandleComponentUpdate(RunningTournament runningTournament)
    {
        if (runningTournament == null) return;
        var hasUserPaidForTournament = runningTournament.hasPaid;
        var firstPlaceReward = TournamentManager.instance.GetRunningFirstPlaceReward(); 
        
        _prizePool.text = $"{firstPlaceReward}"; 

        if (hasUserPaidForTournament)
        {
            var rank = runningTournament.rank;
            _playNow.SetActive(true);
            _entryCost.SetActive(false);
            _rankNumber.text = rank > 0 ? $"Your Rank: {ToOrdinal(rank)}" : "";
        }
        else
        {
            _playNow.SetActive(false);
            _entryCost.SetActive(true);
            _playersCount.text = $"Players: {runningTournament.playerCount}";

        }
    }

    private void OnDisable()
    {
        if (TournamentManager.instance)
        {
            TournamentManager.instance.OnRunningTournamentChanged -= HandleComponentUpdate;
        }
    }

    private static string ToOrdinal(long number)
    {
        if (number <= 0) return number.ToString();

        string suffix;
        var lastDigit = number % 10;
        var lastTwoDigits = number % 100;

        if (lastTwoDigits == 11 || lastTwoDigits == 12 || lastTwoDigits == 13)
        {
            suffix = "th";
        }
        else
        {
            suffix = lastDigit switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th",
            };
        }

        return $"{number} {suffix}";
    }
}
