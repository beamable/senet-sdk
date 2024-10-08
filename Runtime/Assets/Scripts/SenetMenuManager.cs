using Beamable;
using Beamable.Server.Clients;
using System;
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

    public void RegularGame()
    {
        TournamentManager.instance.isTournament = false;
        SceneManager.LoadSceneAsync(_gameplaySceneName);
    }

    public void Wheel()
    {
        SceneManager.LoadSceneAsync("SenetSpinWheel");
    }

    public void Leaderboard()
    {
        SceneManager.LoadSceneAsync("SenetLeaderboard");
    }

    public void Withdraw()
    {
        SceneManager.LoadSceneAsync("SenetWithdraw");
    }

    public void Wallet()
    {
        SceneManager.LoadSceneAsync("SenetWallet");
    }

    public void Activity()
    {
        SceneManager.LoadSceneAsync("SenetActivity");
    }

    public void ShareAndEarn()
    {
        SceneManager.LoadSceneAsync("SenetShareAndEarn");
    }

    public void Redeem()
    {
        SceneManager.LoadSceneAsync("SenetRedeem");
    }

    public async void JoinTournament()
    {
        var eventId = TournamentManager.instance.eventId;

        if (eventId != "")
        {
            try
            {
                await _tournamentServiceClient.CheckOrCreatePayment(eventId);
                StartTournament();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }

        }
    }

    private void StartTournament()
    {
        TournamentManager.instance.isTournament = true;
        SceneManager.LoadSceneAsync(_gameplaySceneName);
    }
}
