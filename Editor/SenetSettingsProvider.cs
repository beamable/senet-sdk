using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

class SenetSettingsProvider : SettingsProvider
{
    private static SerializedObject m_SenetSettings;
    private const string adConfiguration = "Ad Configuration";
    private const string core = "Core";

    public SenetSettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
        : base(path, scope) { }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        m_SenetSettings = SenetSettings.GetSerializedSettings();
    }

    [SettingsProvider]
    public static SettingsProvider CreateMainSettingsProvider()
    {
        var provider = new SenetSettingsProvider("Project/Senet", SettingsScope.Project)
        {
            label = "Senet",
            guiHandler = (searchContext) =>
            {
                GUILayout.Space(10);
                GUILayout.Label(adConfiguration, EditorStyles.boldLabel);
                DrawAdConfigurationSettings();

                GUILayout.Space(10);
                GUILayout.Label(core, EditorStyles.boldLabel);
                DrawCoreSettings();
            },
            keywords = new[] { "Senet" }
        };

        return provider;
    }

    [SettingsProvider]
    public static SettingsProvider CreateAdConfigurationProvider()
    {
        var provider = new SenetSettingsProvider("Project/Senet/AdConfiguration", SettingsScope.Project)
        {
            label = adConfiguration,
            guiHandler = (searchContext) =>
            {
                DrawAdConfigurationSettings();
            },
            keywords = new[] { "Ad", "Configuration" }
        };

        return provider;
    }

    [SettingsProvider]
    public static SettingsProvider CreateCoreProvider()
    {
        var provider = new SenetSettingsProvider("Project/Senet/Core", SettingsScope.Project)
        {
            guiHandler = (searchContext) =>
            {
                DrawCoreSettings();
            },
            keywords = new[] { "Core" }
        };

        return provider;
    }

    private static void DrawAdConfigurationSettings()
    {
        EditorGUILayout.PropertyField(m_SenetSettings.FindProperty("androidGameId"));
        EditorGUILayout.PropertyField(m_SenetSettings.FindProperty("iOSGameId"));
        EditorGUILayout.PropertyField(m_SenetSettings.FindProperty("testMode"));
        EditorGUILayout.PropertyField(m_SenetSettings.FindProperty("androidAdUnitId"));
        EditorGUILayout.PropertyField(m_SenetSettings.FindProperty("iOSAdUnitId"));
        m_SenetSettings.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void DrawCoreSettings()
    {
        EditorGUILayout.PropertyField(m_SenetSettings.FindProperty("simulatedLoading"));
        EditorGUILayout.PropertyField(m_SenetSettings.FindProperty("gameplaySceneName"));
        EditorGUILayout.PropertyField(m_SenetSettings.FindProperty("wheelCooldownSeconds"));
        m_SenetSettings.ApplyModifiedPropertiesWithoutUndo();
    }
}
