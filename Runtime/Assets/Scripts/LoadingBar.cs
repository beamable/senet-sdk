using Beamable;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    [SerializeField]
    private Image _progressBar;

    async void Start()
    {
        var settings = Resources.Load<SenetSettings>("SenetSettings");
        var simulated = settings.simulatedLoading;

        var isFirstLaunch = PlayerPrefs.GetInt("IsFirstLaunch", 1);
        string level = await DetermineLevel(isFirstLaunch);

        if (simulated)
        {
            StartCoroutine(SimulatedLoading(level));
        }
        else
        {
            StartCoroutine(LoadLevelAsync(level));
        }
    }

    IEnumerator LoadLevelAsync(string level)
    {
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(level);

        while (loadingOperation.isDone == false)
        {
            _progressBar.fillAmount = loadingOperation.progress;
            yield return new WaitForEndOfFrame();
        }
    }

    private async Task<string> DetermineLevel(int isFirstLaunch)
    {
        if (isFirstLaunch != 0)
        {
            return "SenetWelcome";
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return "SenetNoInternet";
        }

        var beamContext = BeamContext.Default;
        await beamContext.OnReady;
        await beamContext.Accounts.OnReady;

        var account = beamContext.Accounts.Current;

        Debug.Log($"Player Email: {account.Email},\n Player GamerTag: {account.GamerTag}, \n Cid: {beamContext.Cid}");

        if (account != null && !string.IsNullOrEmpty(account.Email))
        {
            return "SenetMainMenu";
        }

        return "SenetSignUp";
    }

    IEnumerator SimulatedLoading(string level)
    {
        float i = 0;
        while (i < 1)
        {
            _progressBar.fillAmount = i;
            i += (float)0.0005;
            yield return new WaitForEndOfFrame();
        }

        yield return SceneManager.LoadSceneAsync(level);
    }
}
