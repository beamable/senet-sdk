using UnityEditor;
using UnityEngine;

public class SenetSettings : ScriptableObject
{
    public const string senetSettingsPath = "Assets/Senet/Resources/SenetSettings.asset";

    [SerializeField]
    public string androidGameId;
    [SerializeField]
    public string iOSGameId;
    [SerializeField]
    public bool testMode;
    [SerializeField]
    public string androidAdUnitId;
    [SerializeField]
    public string iOSAdUnitId;
    [SerializeField]
    public bool simulatedLoading;
    [SerializeField]
    public string gameplaySceneName;
    [SerializeField]
    public long wheelCooldownSeconds;

    internal static SenetSettings GetOrCreateSettings()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Senet"))
        {
            AssetDatabase.CreateFolder("Assets", "Senet");
            AssetDatabase.CreateFolder("Assets/Senet", "Resources");
        }

        var settings = AssetDatabase.LoadAssetAtPath<SenetSettings>(senetSettingsPath);

        if (settings == null)
        {
            settings = CreateInstance<SenetSettings>();
            AssetDatabase.CreateAsset(settings, senetSettingsPath);
            AssetDatabase.SaveAssets();
        }
        return settings;
    }

    public static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }
}
