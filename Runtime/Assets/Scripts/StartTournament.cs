using UnityEngine;
using UnityEngine.SceneManagement;

public class StartTournament : MonoBehaviour
{
    [SerializeField]
    private string _tournamentScene = "";
    public void GoToTournament()
    {
        TournamentManager.instance.IsTournament = true;
        SceneManager.LoadSceneAsync(_tournamentScene);
    }
}
