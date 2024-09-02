using UnityEngine;
using UnityEngine.UI;

public class RewardAmount : MonoBehaviour
{
    [SerializeField]
    private Text _rewardAmount;

    void Update()
    {
        _rewardAmount.text = $"You Win {TournamentManager.instance.rewardAmount} Coins";
    }
}
