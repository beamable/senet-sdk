using Beamable;
using Beamable.Common;
using Beamable.Common.Api.Auth;
using System.Threading.Tasks;
using Beamable.Api;
using Beamable.Api.Sessions;
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

    public void Cancel()
    {
        _confirmationPopup.SetActive(false);
    }

    public async void OpenLoadingPopup()
    {
        _loadingPopup.SetActive(true);
        _confirmationPopup.SetActive(false);
        
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;

        beamContext.ClearPlayerAndStop();
        
        if (beamContext.IsStopped)
        {
            await BeamContext.Default.Instance;
        }
        
        await Task.Delay(2000);

        _logoutSuccessfulPopup.SetActive(true);
        _loadingPopup.SetActive(false);

        await Task.Delay(1000);

        SceneManager.LoadScene("SenetWelcome");
    }
}
