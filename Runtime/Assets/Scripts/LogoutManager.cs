using Beamable;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _confirmationPopup;
    [SerializeField]
    private GameObject _loadingPopup;
    [SerializeField]
    private GameObject _logoutSuccessfulPopup;

    public async void OpenLoadingPopup()
    {
        _loadingPopup.SetActive(true);
        _confirmationPopup.SetActive(false);
        
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;

        await beamContext.ClearPlayerAndStop();
        
        if (beamContext.IsStopped)
        {
            await BeamContext.Default.Instance;
        }
        
        await Task.Delay(2000);

        Destroy(TournamentManager.instance);
        Destroy(CurrencyManager.instance);

        _logoutSuccessfulPopup.SetActive(true);
        _loadingPopup.SetActive(false);

        await Task.Delay(1000);

        SceneManager.LoadScene("SenetWelcome");
    }
}
