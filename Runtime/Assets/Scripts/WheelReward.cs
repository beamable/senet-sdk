using Beamable;
using Beamable.Server.Clients;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class WheelReward : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField]
    private Button _spinButton;
    [SerializeField]
    private GameObject _rewardScreen;
    [SerializeField]
    private GameObject _betterLuckScreen;
    private string _adUnitId = null;
    public float StopPower;
    private long _amount;
    [SerializeField]
    private Rigidbody2D rbody;
    [SerializeField]
    private TMP_Text _reward;
    int inRotate;
    float t;

    void Awake()
    {
        var settings = Resources.Load<SenetSettings>("SenetSettings");
        var iOSAdUnitId = settings.iOSAdUnitId;
        var androidAdUnitId = settings.androidAdUnitId;
#if UNITY_IOS
        _adUnitId = iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = androidAdUnitId;
#elif UNITY_EDITOR
        _adUnitId = androidAdUnitId; //Only for testing the functionality in the Editor
#endif

        _spinButton.interactable = false;
        LoadAd();
    }

    public void LoadAd()
    {
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_adUnitId))
        {
            _spinButton.onClick.AddListener(ShowAd);
            _spinButton.interactable = true;
        }
    }

    public void ShowAd()
    {
        Debug.Log("Current Orientation: " + Screen.orientation);
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = false;

        //_showAdButton.interactable = false;
        Advertisement.Show(_adUnitId, this);
    }

    private void Update()
    {
        if (rbody.angularVelocity > 0)
        {
            rbody.angularVelocity -= StopPower * Time.deltaTime;
            rbody.angularVelocity = Mathf.Clamp(rbody.angularVelocity, 0, 1440);
        }

        if (rbody.angularVelocity == 0 && inRotate == 1)
        {
            t += 1 * Time.deltaTime;
            if (t >= 0.5f)
            {
                GetReward();

                inRotate = 0;
                t = 0;
            }
        }
    }

    public void Rotate()
    {
        if (inRotate == 0)
        {
            PlayerPrefs.SetString("LastDateSpun", DateTime.Now.Ticks.ToString());
            rbody.AddTorque(UnityEngine.Random.Range(10000f, 20000f));
            inRotate = 1;
        }
    }

    private async void GetReward()
    {
        var _beamContext = BeamContext.Default;
        await _beamContext.OnReady;
        await _beamContext.Accounts.OnReady;

        float rot = rbody.gameObject.transform.eulerAngles.z;
        var rewardData = await _beamContext.Microservices().TournamentService().CalculateReward(rot);

        long rewardAmount = rewardData.rewardAmount;
        float newRotationAngle = rewardData.rotationAngle;

        RewardHelper(new Vector3(0, 0, newRotationAngle), rewardAmount);
    }

    public async void ClaimReward()
    {
        _spinButton.interactable = true;
        _rewardScreen.SetActive(false);
        var countCoins = FindObjectOfType<CountCoins>();

        countCoins.AddCoins();
        await Task.Delay(1000);
        await CurrencyManager.instance.AddOrRemoveSenet(_amount);

    }

    public void SpinAgain()
    {
        _betterLuckScreen.SetActive(false);
        ShowAd();
    }

    private void RewardHelper(Vector3 vector3, long amount)
    {
        rbody.gameObject.transform.eulerAngles = vector3;
        _amount = amount;
        _reward.text = $"+{amount}";

        if (amount == 0)
        {
            _betterLuckScreen.SetActive(true);
        }
        else
        {
            _rewardScreen.SetActive(true);
        }
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            _spinButton.interactable = false;
            Rotate();
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
}