using Beamable;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryConnection : MonoBehaviour
{
    public async void Retry()
    {
        Debug.Log(Application.internetReachability);
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            var beamContext = BeamContext.Default;
            await beamContext.OnReady;
            await beamContext.Accounts.OnReady;

            var account = beamContext.Accounts.Current;

            if (account != null && !string.IsNullOrEmpty(account.Email))
            {
                SceneManager.LoadScene("SenetMainMenu");
            }
            else
            {
                SceneManager.LoadScene("SenetSignUp");
            }
        }
    }
}
