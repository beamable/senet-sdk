using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    private string _gameId;

    void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
        var settings = Resources.Load<SenetSettings>("SenetSettings");

        var iOSGameId = settings.iOSGameId;
        var androidGameId = settings.androidGameId;
        var testMode = settings.testMode;

#if UNITY_IOS
            _gameId = iOSGameId;
#elif UNITY_ANDROID
        _gameId = androidGameId;
#elif UNITY_EDITOR
        _gameId = androidGameId; //Only for testing the functionality in the Editor
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error} - {message}");
    }
}