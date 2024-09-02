using Beamable;
using Beamable.Api.Autogenerated.Models;
using Beamable.Player;
using Beamable.Server.Clients;
using UnityEngine;
using UnityEngine.UI;

public class ShowLeagueInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject _leagueInfoMainPanel;
    [SerializeField]
    private GameObject _leagueInfoOne;
    [SerializeField]
    private GameObject _leagueInfoTwo;
    [SerializeField]
    private GameObject _welcomePanel;
    [SerializeField]
    private Toggle _toggle;
    [SerializeField]
    private Button _joinButton;
    [SerializeField]
    private Button _startTournamentButton;
    [SerializeField]
    private GameObject _disclaimer;
    private TournamentServiceClient _tournamentServiceClient;
    private PlayerAccount _account;

    private async void Start()
    {
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;
        await beamContext.Accounts.OnReady;

        _account = beamContext.Accounts.Current;
        _tournamentServiceClient = new TournamentServiceClient();

        var hasParticipated = await _tournamentServiceClient.HasUserParticipated();
        var eventId = TournamentManager.instance.eventId;

        if (eventId != "" && !hasParticipated)
        {
            ShowInfo();
        }
    }

    public void DisableInfo()
    {
        _leagueInfoMainPanel.SetActive(false);
        _leagueInfoOne.SetActive(false);
        _leagueInfoTwo.SetActive(false);
    }

    public void ShowInfo()
    {
        _disclaimer.SetActive(true);
        _joinButton.gameObject.SetActive(true);
        _leagueInfoMainPanel.SetActive(true);
        _leagueInfoOne.SetActive(true);
        _leagueInfoTwo.SetActive(false);
    }

    public void ShowInfoWithoutDisclaimer()
    {
        _disclaimer.SetActive(false);
        _joinButton.gameObject.SetActive(false);
        _leagueInfoMainPanel.SetActive(true);
        _leagueInfoOne.SetActive(true);
        _leagueInfoTwo.SetActive(false);
    }

    public void ShowSecondInfo()
    {
        _leagueInfoOne.SetActive(false);
        _leagueInfoTwo.SetActive(true);
    }

    public void ShowTournamentWelcome()
    {
        _welcomePanel.SetActive(true);
    }

    public void JoinTournament()
    {
        var senetMenuManager = gameObject.GetComponent<SenetMenuManager>();
        senetMenuManager.JoinTournament();
    }

    private void Update()
    {
        if (_toggle.isOn)
        {
            _joinButton.interactable = true;
        }
        else
        {
            _joinButton.interactable = false;
        }

        var eventId = TournamentManager.instance.eventId;

        if (eventId == "")
        {
            _startTournamentButton.interactable = false;
        }
        else
        {
            _startTournamentButton.interactable = true;
        }
    }
}