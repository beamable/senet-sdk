using UnityEngine;

public class TournamentButtonVariant : MonoBehaviour
{
    [SerializeField]
    private GameObject _noTournamentAvailable;

    private void Start()
    {
        Debug.Log("TournamentButtonVariant initialized.");

        if (TournamentManager.instance)
        {
            Debug.Log("TournamentManager instance found.");
            HandleButtonVariant(TournamentManager.instance.runningTournament);
            TournamentManager.instance.OnRunningTournamentChanged += HandleButtonVariant;
        }
        else
        {
            Debug.LogWarning("TournamentManager instance not found.");
        }
    }

    private void OnDisable()
    {
        if (TournamentManager.instance)
        {
            TournamentManager.instance.OnRunningTournamentChanged -= HandleButtonVariant;
            Debug.Log("Removed HandleButtonVariant listener.");
        }
    }

    void HandleButtonVariant(RunningTournament runningTournament)
    {
        if (runningTournament != null)
        {
            _noTournamentAvailable.SetActive(false);
            Debug.Log("Tournament found. Hiding no tournament available message.");
        }
        else
        {
            _noTournamentAvailable.SetActive(true);
            Debug.Log("No running tournament. Showing no tournament available message.");
        }
    }
}