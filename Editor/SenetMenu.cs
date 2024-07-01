using UnityEditor;
using UnityEngine;

public class SenetMenu : MonoBehaviour
{
    [MenuItem("Window/Senet/Open Configuration Manager")]
    private static void OpenSenetConfigurationManager()
    {
        SettingsService.OpenProjectSettings("Project/Senet");
    }
}
