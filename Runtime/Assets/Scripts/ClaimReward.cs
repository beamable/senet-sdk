using System;
using System.Threading.Tasks;
using UnityEngine;

public class ClaimReward : MonoBehaviour
{
    [SerializeField]
    private GameObject _congratulationsPanel;
    [SerializeField]
    private GameObject _congratulationsClaimPanel;

    public async void OpenCongratulationsPanel()
    {
        _congratulationsClaimPanel.SetActive(false);
        _congratulationsPanel.SetActive(true);
        await ClaimRewardsInTournament();
    }
    public async Task ClaimRewardsInTournament()
    {
        _congratulationsClaimPanel.SetActive(false);
        await Delay();
        await TournamentManager.instance.ClaimRewardsInTournament();
    }

    private async Task Delay()
    {
        await Task.Delay(TimeSpan.FromSeconds(3));
        Debug.Log("Finished waiting.");
    }
}
