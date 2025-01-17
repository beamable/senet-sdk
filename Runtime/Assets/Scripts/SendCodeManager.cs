using Beamable;
using Beamable.Common.Api.Auth;
using Beamable.Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SendCodeManager : MonoBehaviour
{
    public TextMeshProUGUI sendCodeMessage;
    public Button sendCodeButton;

    private BeamContext _beamContext;
    private PlayerAccount _playerAccount;

    private async void Start()
    {
        _beamContext = await BeamContext.Default.Instance;
        await _beamContext.Accounts.OnReady;
        _playerAccount = _beamContext.Accounts.Current;

        sendCodeMessage.text =
            $"In order to change your password, we need to verify your identity.\n\nA verification code will be sent to your registered email address: {_playerAccount.Email}.";
        
        sendCodeButton.onClick.AddListener(SendResetCode);
    }

    private async void SendResetCode()
    {
        var email = _playerAccount.Email;

        if (string.IsNullOrEmpty(email))
        {
            sendCodeMessage.text = "Email address is not available.";
            return;
        }

        try
        {
            await _beamContext.Api.AuthService.IssuePasswordUpdate(email);
            sendCodeMessage.text = "A verification code has been sent to your email.";
            SceneManager.LoadSceneAsync("SenetNewPassword");
        }
        catch (System.Exception ex)
        {
            sendCodeMessage.text = $"Error sending code: {ex.Message}";
        }
    }
}
