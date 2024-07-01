using Beamable;
using System.Collections;
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

        var beamContext = BeamContext.Default;
        await beamContext.OnReady;
        await beamContext.Accounts.OnReady;

        var account = beamContext.Accounts.Current;
        string level;

        Debug.Log(account.Email);
        Debug.Log(account.Alias);

        if (account != null && account.Email != "")
        {
            level = "MainMenu";
        }
        else
        {
            level = "SenetLogin";
        }
        Debug.Log(level);

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
