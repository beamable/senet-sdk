using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TournamentResult : MonoBehaviour
{
    [SerializeField]
    private GameObject _congratulationsPanel;
    [SerializeField]
    private GameObject _betterLuckPanel;
    [SerializeField]
    private GameObject _congratulationsClaimPanel;

    [SerializeField]
    private Sprite _firstPlace;
    [SerializeField]
    private Sprite _secondPlace;
    [SerializeField]
    private Sprite _thirdPlace;
    [SerializeField]
    private Sprite _badge;
    [SerializeField]
    private GameObject _trophy;

    private void Start()
    {
        if (TournamentManager.instance)
        {
            HandleRankUpdate(TournamentManager.instance.doneTournament);
            TournamentManager.instance.OnDoneTournamentChanged += HandleRankUpdate;
        }
    }

    public async void ClaimReward()
    {
        _congratulationsClaimPanel.SetActive(false);
        _congratulationsPanel.SetActive(true);
        await TournamentManager.instance.ClaimRewardsInTournament();

        await Task.Delay(1000);

        _congratulationsClaimPanel.SetActive(false);
        _congratulationsPanel.SetActive(false);
        _betterLuckPanel.SetActive(false);
    }

    void HandleRankUpdate(DoneTournament doneTournament)
    {
        if (doneTournament != null)
        {
            var rank = doneTournament.rank;
            var image = _trophy.GetComponent<Image>();

            if (rank > 0)
            {
                _congratulationsClaimPanel.SetActive(true);
                if (rank >= 1 && rank <= 10)
                {
                    switch (rank)
                    {
                        case 1:
                            image.sprite = _firstPlace;
                            break;
                        case 2:
                            image.sprite = _secondPlace;
                            break;
                        case 3:
                            image.sprite = _thirdPlace;
                            break;
                    }

                    if (rank >= 4 && rank <= 10)
                    {
                        image.sprite = _badge;
                        _trophy.transform.GetChild(0).GetComponent<TMP_Text>().text = rank.ToString();
                    }
                }
                else
                {
                    _betterLuckPanel.SetActive(true);
                }
            }
        }
    }

    private void OnDisable()
    {
        if (TournamentManager.instance)
        {
            TournamentManager.instance.OnDoneTournamentChanged -= HandleRankUpdate;
        }
    }
}
