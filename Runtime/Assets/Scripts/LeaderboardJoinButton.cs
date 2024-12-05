using UnityEngine;
using UnityEngine.UI;

public class LeaderboardJoinButton : MonoBehaviour
{
    [SerializeField]
    private Text _buttonText;

    [SerializeField]
    private GameObject _price;

    [SerializeField]
    private GameObject _cog;

    [SerializeField]
    private Sprite _disabledBackgroundSprite;

    [SerializeField]
    private Sprite _enabledBackgroundSprite;

    [SerializeField]
    private GameObject _rules;

    [SerializeField]
    private GameObject _noTournamentRules;

    private void Start()
    {
        if (TournamentManager.instance)
        {
            HandleButtonTextChange(TournamentManager.instance.runningTournament);
            TournamentManager.instance.OnRunningTournamentChanged += HandleButtonTextChange;
        }
    }

    private void OnDisable()
    {
        if (TournamentManager.instance)
        {
            TournamentManager.instance.OnRunningTournamentChanged -= HandleButtonTextChange;
        }
    }

    void HandleButtonTextChange(RunningTournament runningTournament)
    {
        if (runningTournament != null)
        {
            gameObject.GetComponent<Image>().sprite = _enabledBackgroundSprite;
            gameObject.GetComponent<Button>().onClick.AddListener(_rules.GetComponent<ToggleVisibility>().SetActive);

            Color color = new Color32(52, 50, 62, 255);
            _buttonText.color = color;
            _cog.SetActive(false);

            var hasPaid = runningTournament.hasPaid;

            if (hasPaid)
            {
                _price.SetActive(false);
                _buttonText.text = "Play Now";
            }
            else
            {
                _price.SetActive(true);
                _buttonText.text = "Join Now";
            }
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = _disabledBackgroundSprite;
            gameObject.GetComponent<Button>().onClick.AddListener(_noTournamentRules.GetComponent<ToggleVisibility>().SetActive);
            Color color = new Color32(161, 155, 179, 255);
            _buttonText.color = color;
            _cog.SetActive(true);
            _price.SetActive(false);
        }
    }
}
