using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class WheelManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public float StopPower;
    [SerializeField]
    private Button _spinButton;
    [SerializeField]
    private GameObject _rewardScreen;
    [SerializeField]
    private GameObject _betterLuckScreen;
    private string _adUnitId = null;

    void Awake()
    {
        var settings = Resources.Load<SenetSettings>("SenetSettings");
        var iOSAdUnitId = settings.iOSAdUnitId;
        Debug.Log(iOSAdUnitId);
        var androidAdUnitId = settings.androidAdUnitId;
        Debug.Log(androidAdUnitId);
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
        //_spinButton.interactable = false;
        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("closed");
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
