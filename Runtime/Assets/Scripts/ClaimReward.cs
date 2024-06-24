using UnityEngine;

public class ClaimReward : MonoBehaviour
{
    public async void ClaimRewardsInTournament()
    {
        await TournamentManager.instance.ClaimRewardsInTournament();
    }
}
