using Beamable;
using Beamable.Server.Clients;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SenetMenuManager : MonoBehaviour
{
    public GameObject popup; 
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

    public void Home()
    {
        SceneManager.LoadSceneAsync("SenetMainMenu");
    }

    public void Redeem()
    {
        SceneManager.LoadSceneAsync("SenetRedeem");
    }

    public void LoadDepositSenetScene()
    {
        SceneManager.LoadSceneAsync("SenetDeposit");
    }

    public async void JoinTournament()
    {
        var eventId = TournamentManager.instance.runningTournament.eventId;

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
    
    public void OpenPopup()
    {
        if (popup != null)
        {
            popup.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Popup with name ConfirmationPopup not found!");
        }
    }
    
    public void ClosePopup()
    {
        if (popup != null)
        {
            popup.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"Popup with name ConfirmationPopup not found!");
        }
    }
}
