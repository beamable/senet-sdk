using UnityEngine;

public class TournamentResult : MonoBehaviour
{
    [SerializeField]
    private GameObject _congratulationPanel;
    [SerializeField]
    private GameObject _betterLuckPanel;

    void Update()
    {
        var rank = TournamentManager.instance.rank;

        if (rank == 1)
        {
            _congratulationPanel.SetActive(true);
        }
        else if (rank > 1)
        {
            _betterLuckPanel.SetActive(true);
        }
        else
        {
            _congratulationPanel.SetActive(false);
            _betterLuckPanel.SetActive(false);
        }
    }
}
