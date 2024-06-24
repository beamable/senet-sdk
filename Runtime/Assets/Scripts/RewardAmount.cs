using TMPro;
using UnityEngine;

public class RewardAmount : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _rewardAmount;

    void Update()
    {
        _rewardAmount.text = $"You win {TournamentManager.instance.rewardAmount} senet coins";
    }
}
