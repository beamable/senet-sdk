using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
using Unity.Notifications.Android;
#endif

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

        var channel = new AndroidNotificationChannel()
        {
            Id = "tournament_channel",
            Name = "Tournament Channel",
            Importance = Importance.Default,
            Description = "Tournament notifications",
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif
    }
}
