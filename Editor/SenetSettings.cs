using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SenetSettings : ScriptableObject
{
    private const string senetRootPath = "Assets/Senet";
    private static string senetSettingsPath = $"{senetRootPath}/Resources/SenetSettings.asset";

    public string androidGameId;
    public string iOSGameId;
    public bool testMode;
    public string androidAdUnitId;
    public string iOSAdUnitId;
    public bool simulatedLoading;
    public string gameplaySceneName;
    public long wheelCooldownSeconds;
    public Sprite gameLogo;

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
