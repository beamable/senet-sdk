using Beamable;
using Beamable.Server.Clients;
using UnityEngine;
using UnityEngine.UI;

public class TournamentButton : MonoBehaviour
{
    [SerializeField]
    private Text _timer;
    [SerializeField]
    private Text _playersCount;
    [SerializeField]
    private GameObject _playNow;
    [SerializeField]
    private GameObject _entryCost;
    [SerializeField]
    private Sprite _tournamentWithRank;
    [SerializeField]
    private Sprite _tournamentWithoutRank;
    [SerializeField]
    private Text _rankNumber;

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

    void HandleComponentUpdate(RunningTournament runningTournament)
    {
        if (runningTournament != null)
        {
            _playersCount.text = $"Players: {runningTournament.playerCount}";
            var rank = runningTournament.rank;

            if (rank > 0)
            {
                gameObject.transform.GetChild(0).GetComponent<Image>().sprite = _tournamentWithRank;
                _rankNumber.text = ToOrdinal(rank);
            }
            else
            {
                gameObject.transform.GetChild(0).GetComponent<Image>().sprite = _tournamentWithoutRank;
            }

            var hasUserPaidForTournament = runningTournament.hasPaid;

            if (hasUserPaidForTournament)
            {
                _playNow.SetActive(true);
                _entryCost.SetActive(false);
            }
            else
            {
                _playNow.SetActive(false);
                _entryCost.SetActive(true);
            }
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
        long lastDigit = number % 10;
        long lastTwoDigits = number % 100;

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
