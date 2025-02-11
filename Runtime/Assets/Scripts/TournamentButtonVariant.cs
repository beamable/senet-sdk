using UnityEngine;

public class TournamentButtonVariant : MonoBehaviour
{
    [SerializeField]
    private GameObject _noTournamentAvailable;

    private void Start()
    {
        if (TournamentManager.instance)
        {
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
        }
        else
        {
            _noTournamentAvailable.SetActive(true);
        }
    }
}