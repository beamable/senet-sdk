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
    private GameObject _scroll;
    [SerializeField]
    private Toggle _toggle;
    private bool _hasPaid;

    void Start()
    {
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
                    _rulesAndAgreement.SetActive(false);
                    _welcome.SetActive(true);
                });
            }
        }
    }

    public void JoinTournament()
    {
        var senetMenuManager = gameObject.GetComponent<SenetMenuManager>();
        senetMenuManager.JoinTournament();
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
