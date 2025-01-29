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

    void Start()
    {
        _senetMenuManager = gameObject.GetComponent<SenetMenuManager>();

        var senetAmount = CurrencyManager.instance.senet;

        if (senetAmount < 25)
        {
            _notEnoughTokens.SetActive(true);
            _rulesAndAgreement.SetActive(false);
        }
        else
        {
            _notEnoughTokens.SetActive(false);
            _rulesAndAgreement.SetActive(true);
            var runningTournament = TournamentManager.instance.runningTournament;
            if (runningTournament != null)
            {
                var hasPaid = runningTournament.hasPaid;

                _hasPaid = hasPaid;

                if (hasPaid)
                {
                    _scroll.GetComponent<RectTransform>().sizeDelta = new Vector2(_scroll.GetComponent<RectTransform>().rect.width, 1110);
                    _terms.SetActive(false);
                    _entryCost.SetActive(false);
                    _tournamentButtonText.text = "Play Now";
                    _tournamentButton.onClick.AddListener(JoinTournament);
                }
                else
                {
                    _scroll.GetComponent<RectTransform>().sizeDelta = new Vector2(_scroll.GetComponent<RectTransform>().rect.width, 740);
                    _tournamentButton.onClick.AddListener(() =>
                    {
                        _confirmation.SetActive(true);
                    });
                }
            }
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

        await Task.Delay(2000);
        await CurrencyManager.instance.AddOrRemoveSenet(-25);

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
