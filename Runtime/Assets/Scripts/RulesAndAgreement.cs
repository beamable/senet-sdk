using System.Threading.Tasks;
using Beamable.Server.Clients;
using UnityEngine;
using UnityEngine.UI;

public class RulesAndAgreement : MonoBehaviour
{
    [SerializeField]
    private GameObject _terms;
    [SerializeField]
    private GameObject _entryCost;
    [SerializeField]
    private GameObject _welcome;
    [SerializeField]
    private Button _tournamentButton;
    [SerializeField]
    private Text _tournamentButtonText;
    [SerializeField]
    private GameObject _rulesAndAgreement;
    [SerializeField]
    private GameObject _confirmation;
    [SerializeField]
    private GameObject _scroll;
    [SerializeField]
    private GameObject _notEnoughTokens;
    [SerializeField]
    private Toggle _toggle;

    private bool _hasPaid;
    private SenetMenuManager _senetMenuManager;
    private TournamentServiceClient _tournamentServiceClient;

    private async void Start()
    {
        _senetMenuManager = gameObject.GetComponent<SenetMenuManager>();
        _tournamentServiceClient = new TournamentServiceClient();

        await CheckTournamentPaymentStatus();
    }

    private async Task CheckTournamentPaymentStatus()
    {
        var runningTournament = TournamentManager.instance.runningTournament;

        if (runningTournament == null)
        {
            DisplayNoTokensMessage();
            return;
        }

        _hasPaid = await _tournamentServiceClient.HasUserPaidForTournament(runningTournament.eventId);

        if (_hasPaid)
        {
            DisplayPaidState();
            return;
        }

        CheckUserTokenBalance();
    }

    private void CheckUserTokenBalance()
    {
        var senetAmount = CurrencyManager.instance.senet;

        if (senetAmount < 25)
        {
            DisplayNoTokensMessage();
        }
        else
        {
            DisplayPaymentRequiredState();
        }
    }

    private void DisplayPaidState()
    {
        _notEnoughTokens.SetActive(false);
        _rulesAndAgreement.SetActive(true);

        _scroll.GetComponent<RectTransform>().sizeDelta = new Vector2(_scroll.GetComponent<RectTransform>().rect.width, 1110);
        _terms.SetActive(false);
        _entryCost.SetActive(false);
        _tournamentButtonText.text = "Play Now";
        _tournamentButton.onClick.AddListener(JoinTournament);
    }

    private void DisplayNoTokensMessage()
    {
        _notEnoughTokens.SetActive(true);
        _rulesAndAgreement.SetActive(false);
    }

    private void DisplayPaymentRequiredState()
    {
        _notEnoughTokens.SetActive(false);
        _rulesAndAgreement.SetActive(true);

        _scroll.GetComponent<RectTransform>().sizeDelta = new Vector2(_scroll.GetComponent<RectTransform>().rect.width, 740);
        _tournamentButton.onClick.AddListener(() =>
        {
            _confirmation.SetActive(true);
        });
    }

    public void JoinTournament()
    {
        _senetMenuManager.StartTournament();
    }

    public async void PayForTournament()
    {
        await _senetMenuManager.PayForTournament();

        var countCoins = FindObjectOfType<CountCoins>();
        countCoins.RemoveCoins();

        await Task.Delay(2000);
        _hasPaid = true;

        _confirmation.SetActive(false);
        _rulesAndAgreement.SetActive(false);
        _welcome.SetActive(true);
    }

    private void Update()
    {
        _tournamentButton.interactable = _hasPaid || _toggle.isOn;
    }
}
