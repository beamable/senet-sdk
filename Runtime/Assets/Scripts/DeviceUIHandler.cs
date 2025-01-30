using UnityEngine;

public class DeviceUIHandler : MonoBehaviour
{
    [Header("Mobile Buttons")]
    [SerializeField]
    private GameObject tournamentButtonMobile; // Tournament button for mobile.

    [SerializeField]
    private GameObject noTournamentButtonMobile; // No tournament button for mobile.

    [Header("Tablet Buttons")]
    [SerializeField]
    private GameObject tournamentButtonTablet; // Tournament button for tablet.

    [SerializeField]
    private GameObject noTournamentButtonTablet; // No tournament button for tablet.

    [Header("Containers")]
    [SerializeField]
    private GameObject sizeContainer; // Container for mobile.

    [SerializeField]
    private GameObject tabletContainer; // Container for tablet.

    private const float TabletScreenWidthThresholdPixels = 403f;  // Tablet width threshold in pixels.
    private const float TabletScreenHeightThresholdPixels = 500f; // Tablet height threshold in pixels.

    private void Start()
    {
        HandleDeviceUI();

        if (TournamentManager.instance)
        {
            HandleButtonVariant(TournamentManager.instance.runningTournament);
            TournamentManager.instance.OnRunningTournamentChanged += HandleButtonVariant;
        }
    }

    private void OnDisable()
    {
        if (TournamentManager.instance)
        {
            TournamentManager.instance.OnRunningTournamentChanged -= HandleButtonVariant;
        }
    }

    private void HandleDeviceUI()
    {
        if (IsTablet())
        {
            // Switch containers
            if (sizeContainer != null)
            {
                sizeContainer.SetActive(false);
            }

            if (tabletContainer != null)
            {
                tabletContainer.SetActive(true);
            }
        }
        else
        {
            // Switch containers
            if (sizeContainer != null)
            {
                sizeContainer.SetActive(true);
            }

            if (tabletContainer != null)
            {
                tabletContainer.SetActive(false);
            }
        }
    }

    private bool IsTablet()
    {
        float screenWidth = Screen.width; // Screen width in pixels.
        float screenHeight = Screen.height; // Screen height in pixels.

        return screenWidth >= TabletScreenWidthThresholdPixels && screenHeight >= TabletScreenHeightThresholdPixels;
    }

    private void HandleButtonVariant(RunningTournament runningTournament)
    {
        bool isTablet = IsTablet();

        if (runningTournament != null)
        {
            // Enable the correct tournament button
            if (isTablet)
            {
                if (tournamentButtonTablet != null) tournamentButtonTablet.SetActive(true);
                if (noTournamentButtonTablet != null) noTournamentButtonTablet.SetActive(false);
            }
            else
            {
                if (tournamentButtonMobile != null) tournamentButtonMobile.SetActive(true);
                if (noTournamentButtonMobile != null) noTournamentButtonMobile.SetActive(false);
            }
        }
        else
        {
            // Enable the correct no-tournament button
            if (isTablet)
            {
                if (tournamentButtonTablet != null) tournamentButtonTablet.SetActive(false);
                if (noTournamentButtonTablet != null) noTournamentButtonTablet.SetActive(true);
            }
            else
            {
                if (tournamentButtonMobile != null) tournamentButtonMobile.SetActive(false);
                if (noTournamentButtonMobile != null) noTournamentButtonMobile.SetActive(true);
            }
        }
    }
}
