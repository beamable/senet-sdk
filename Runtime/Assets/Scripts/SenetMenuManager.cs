using Beamable;
using Beamable.Server.Clients;
using System;
using System.Threading.Tasks;
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
    
    public void MoreGames()
    {
        SceneManager.LoadSceneAsync("SenetMoreGames");
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

    public void Profile()
    {
        SceneManager.LoadSceneAsync("SenetProfile");
    }
    
    public void Redeem()
    {
        SceneManager.LoadSceneAsync("SenetRedeem");
    }

     public void DepositSenet()
     {
        SceneManager.LoadSceneAsync("SenetDeposit");
     }
     
     public void SendEmailCode()
     {
        SceneManager.LoadSceneAsync("SenetSendEmailCode");
     }
     
     public void SenetNewPassword()
     {
         SceneManager.LoadSceneAsync("SenetNewPassword");
     }

    public async Task PayForTournament()
    {
        var eventId = TournamentManager.instance.runningTournament.eventId;

        if (eventId != "")
        {
            try
            {
                await _tournamentServiceClient.CheckOrCreatePayment(eventId);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }
    }

    public void StartTournament()
    {
        TournamentManager.instance.isTournament = true;
        SceneManager.LoadSceneAsync(_gameplaySceneName);
    }
}
