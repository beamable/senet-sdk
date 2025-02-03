using System.Threading.Tasks;
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

    private void Start()
    {
        _senetMenuManager = gameObject.GetComponent<SenetMenuManager>();
        var runningTournament = TournamentManager.instance.runningTournament;
        
        if (runningTournament is { hasPaid: true })
        {
            _hasPaid = true;
            _notEnoughTokens.SetActive(false);
            _rulesAndAgreement.SetActive(true);

            _scroll.GetComponent<RectTransform>().sizeDelta = new Vector2(_scroll.GetComponent<RectTransform>().rect.width, 1110);
            _terms.SetActive(false);
            _entryCost.SetActive(false);
            _tournamentButtonText.text = "Play Now";
            _tournamentButton.onClick.AddListener(JoinTournament);

            return;
        }

        var senetAmount = CurrencyManager.instance.senet;

        if (senetAmount < 25)
        {
            Debug.Log("Not enough tokens to join the tournament.");
            _notEnoughTokens.SetActive(true);
            _rulesAndAgreement.SetActive(false);
        }
        else
        {
            Debug.Log("User has enough tokens, but needs to pay tournament fee.");
            _notEnoughTokens.SetActive(false);
            _rulesAndAgreement.SetActive(true);

            _scroll.GetComponent<RectTransform>().sizeDelta = new Vector2(_scroll.GetComponent<RectTransform>().rect.width, 740);
            _tournamentButton.onClick.AddListener(() =>
            {
                Debug.Log("Opening confirmation popup for tournament payment.");
                _confirmation.SetActive(true);
            });
        }
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

        await Task.Delay(1000);
        _hasPaid = true;
        Debug.Log(_hasPaid);

        _confirmation.SetActive(false);
        _rulesAndAgreement.SetActive(false);
        _welcome.SetActive(true);
    }

    void Update()
    {
        if (_hasPaid || _toggle.isOn)
        {
            _tournamentButton.interactable = true;
        }
        else
        {
            _tournamentButton.interactable = false;
        }
    }
}
