using UnityEngine;

public class TournamentButtonVariant : MonoBehaviour
{
    [SerializeField]
    private GameObject _tournamentButton;

    [SerializeField]
    private GameObject _noTournamentButton;

    void Start()
    {
        if (TournamentManager.instance)
        {
            HandleButtonVariant(TournamentManager.instance.runningTournament);
            TournamentManager.instance.OnRunningTournamentChanged += HandleButtonVariant;
        }
    }

    private void OnDisable()
    {
        if (TournamentManager.instance)
        {
            TournamentManager.instance.OnRunningTournamentChanged -= HandleButtonVariant;
        }
    }

    void HandleButtonVariant(RunningTournament runningTournament)
    {
        if (runningTournament != null)
        {
            _tournamentButton.SetActive(true);
            _noTournamentButton.SetActive(false);
        }
        else
        {
            _tournamentButton.SetActive(false);
            _noTournamentButton.SetActive(true);
        }
    }
}
