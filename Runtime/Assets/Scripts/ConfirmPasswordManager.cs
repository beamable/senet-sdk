using Beamable;
using Beamable.Common.Api.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfirmPasswordManager : MonoBehaviour
{
    public TMP_InputField codeInputField;
    public TMP_InputField newPasswordInputField;
    public TMP_InputField confirmPasswordInputField;
    public TextMeshProUGUI confirmUpdateMessage;
    public GameObject confirmationPopup; 

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

            // Clear the fields after a successful password update
            codeInputField.text = string.Empty;
            newPasswordInputField.text = string.Empty;
            confirmPasswordInputField.text = string.Empty;
        }
        catch (System.Exception ex)
        {
            confirmUpdateMessage.text = $"Invalid Confirmation Code";
        }
    }

    public void OnCloseButtonClicked()
    {
        if (!string.IsNullOrEmpty(newPasswordInputField.text) || 
            !string.IsNullOrEmpty(confirmPasswordInputField.text))
        {
            confirmationPopup.SetActive(true);
        }
        else
        {
            NavigateToProfile();
        }
    }

   

    private void NavigateToProfile()
    {
        SceneManager.LoadSceneAsync("SenetProfile");
    }
}
