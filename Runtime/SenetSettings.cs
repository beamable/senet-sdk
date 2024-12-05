#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SenetSettings : ScriptableObject
{
    private static readonly string senetRootPath = "Assets/Senet";
    private static readonly string senetSettingsPath = $"{senetRootPath}/Resources/SenetSettings.asset";

    public string androidGameId;
    public string iOSGameId;
    public bool testMode;
    public string androidAdUnitId;
    public string iOSAdUnitId;
    public bool simulatedLoading;
    public string gameplaySceneName;
    public string senetGameName;
    public long wheelCooldownSeconds;
    public Sprite gameLogo;

#if UNITY_EDITOR
    internal static SenetSettings GetOrCreateSettings()
    {
        if (!AssetDatabase.IsValidFolder(senetRootPath))
        {
            AssetDatabase.CreateFolder("Assets", "Senet");
            AssetDatabase.CreateFolder(senetRootPath, "Resources");
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
#endif
}
