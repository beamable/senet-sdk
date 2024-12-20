using Beamable;
using Beamable.Common.Api.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPasswordManager : MonoBehaviour
{
    public TMP_InputField codeInputField;
    public TMP_InputField newPasswordInputField;
    public TMP_InputField confirmPasswordInputField;
    public TextMeshProUGUI confirmUpdateMessage;

    private BeamContext _beamContext;

    private async void Start()
    {
        _beamContext = await BeamContext.Default.Instance;
    }

    public async void UpdatePassword()
    {
        var code = codeInputField.text;
        var newPassword = newPasswordInputField.text;
        var confirmPassword = confirmPasswordInputField.text;

        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
        {
            confirmUpdateMessage.text = "All fields are required.";
            return;
        }

        if (newPassword != confirmPassword)
        {
            confirmUpdateMessage.text = "Passwords do not match.";
            return;
        }

        try
        {
            await _beamContext.Api.AuthService.ConfirmPasswordUpdate(code, newPassword);
            confirmUpdateMessage.text = "Password successfully updated!";
        }
        catch (System.Exception ex)
        {
            confirmUpdateMessage.text = $"Error updating password: {ex.Message}";
        }
    }
}