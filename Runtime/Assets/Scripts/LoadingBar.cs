using Beamable;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    [SerializeField]
    private GameObject _betaPopup;
    private Slider _slider;

    private void Start()
    {
        _slider = transform.GetChild(3).GetChild(2).GetComponent<Slider>();
    }

    public async void Continue()
    {
        _betaPopup.SetActive(false);
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
            _slider.value = loadingOperation.progress;
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

        if (account != null && !string.IsNullOrEmpty(account.Email))
        {
            return "SenetMainMenu";
        }

        return "SenetSignIn";
    }

    IEnumerator SimulatedLoading(string level)
    {
        float i = 0;
        while (i < 1)
        {
            _slider.value = i;
            i += (float)0.0005;
            yield return new WaitForEndOfFrame();
        }

        yield return SceneManager.LoadSceneAsync(level);
    }
}
