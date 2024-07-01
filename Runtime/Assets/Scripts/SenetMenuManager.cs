using Beamable;
using Beamable.Server.Clients;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SenetMenuManager : MonoBehaviour
{
    private string _gameplaySceneName;
    private BeamContext _beamContext;
    private TournamentServiceClient _tournamentServiceClient;

    private async void Start()
    {
        var settings = Resources.Load<SenetSettings>("SenetSettings");
        _gameplaySceneName = settings.gameplaySceneName;

        _beamContext = BeamContext.Default;
        await _beamContext.OnReady;
        await _beamContext.Accounts.OnReady;
        _tournamentServiceClient = new TournamentServiceClient();
    }

    public void Tournament()
    {
        var account = _beamContext.Accounts.Current;
        Debug.Log($"GamerTag: {account.GamerTag}");

        _beamContext.Api.EventsService.Subscribe(async eventsGetResponse =>
        {
            if (eventsGetResponse.running.Count > 0)
            {
                var paidTournamentId = await _tournamentServiceClient.GetPaidTournamentById(account.GamerTag);
                if (TournamentManager.instance.eventId != "")
                {
                    var hasPaid = paidTournamentId == TournamentManager.instance.eventId;

                    if (hasPaid)
                    {
                        StartTournament();
                    }
                    else
                    {
                        LoadSceneBasedOnBalance();
                    }
                }
            }
            else
            {
                SceneManager.LoadSceneAsync("SenetNoTournament");
            }
        });
    }

    public void Wheel()
    {
        SceneManager.LoadSceneAsync("SenetWheel");
    }

    public void Leaderboard()
    {
        SceneManager.LoadSceneAsync("SenetLeaderboard");
    }

    public void Withdraw()
    {
        SceneManager.LoadSceneAsync("SenetWithdraw");
    }

    private void LoadSceneBasedOnBalance()
    {
        if (CurrencyManager.Instance.senet < 10)
        {
            SceneManager.LoadScene("SenetLowBalance");
        }
        else
        {
            SceneManager.LoadScene("SenetPayEntryFee");
        }
    }

    private void StartTournament()
    {
        TournamentManager.instance.isTournament = true;
        SceneManager.LoadSceneAsync(_gameplaySceneName);
    }
}
