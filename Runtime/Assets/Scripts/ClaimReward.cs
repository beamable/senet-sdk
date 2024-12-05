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
        await TournamentManager.instance.ClaimRewardsInTournament();
    }
}
