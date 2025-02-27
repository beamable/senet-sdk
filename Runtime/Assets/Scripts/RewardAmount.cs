using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RewardAmount : MonoBehaviour
{
    [SerializeField]
    private Text _rewardAmount;

    private void Start()
    {
        if (ReferenceEquals(_rewardAmount, null))
        {
            Debug.LogError("RewardAmount Text is not assigned!");
            return;
        }

        StartCoroutine(CheckTournamentData());
    }

    private IEnumerator CheckTournamentData()
    {
        var tournamentManager = TournamentManager.instance;

        while (ReferenceEquals(tournamentManager, null) || ReferenceEquals(tournamentManager.doneTournament, null))
        {
            yield return null; 
        }

        UpdateRewardAmount();
    }

    private void UpdateRewardAmount()
    {
        var tournamentManager = TournamentManager.instance;

        if (ReferenceEquals(tournamentManager.doneTournament, null))
        {
            _rewardAmount.text = "No Tournament Rewards Available";
            return;
        }

        _rewardAmount.text = tournamentManager.doneTournament.rewardAmount > 0 ? $"You Win {tournamentManager.doneTournament.rewardAmount} Coins" : "No Tournament Rewards Available";
    }
}
