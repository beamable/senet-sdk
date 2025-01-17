using Beamable;
using Beamable.Common;
using Beamable.Server.Clients;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Web3FederationCommon;

public class AuthenticationManager : MonoBehaviour
{
    [SerializeField]
    private InputField usernameInputField;
    [SerializeField]
    private InputField emailInputField;
    [SerializeField]
    private InputField passwordInputField;
    [SerializeField]
    private InputField confirmPasswordInputField;
    [SerializeField]
    private Button signInButton;
    [SerializeField]
    private Button signUpButton;
    [SerializeField]
    private Button togglePassword;
    [SerializeField]
    private Button toggleConfirmPassword;
    private BeamContext _beamContext;
    [SerializeField]
    private GameObject errorMessagePanel;
    [SerializeField]
    private TMP_Text errorMessageText;
    [SerializeField]
    private GameObject _loadingPanel;

    private async void Start()
    {
        _beamContext = BeamContext.Default;
        await _beamContext.OnReady;
        await _beamContext.Accounts.OnReady;

        if (signUpButton != null)
        {
            signUpButton.interactable = true;
            signUpButton.onClick.AddListener(SignUpUser);
        }
        else
        {
            signInButton.interactable = true;
            signInButton.onClick.AddListener(LoginUser);
        }

        togglePassword.onClick.AddListener(() => TogglePassword(false));

        if (toggleConfirmPassword != null)
        {
            toggleConfirmPassword.onClick.AddListener(() => TogglePassword(true));
        }
    }

    private async void LoginUser()
    {
        if (emailInputField.text == "" || emailInputField.text == "Anonymous" || signInButton == null) return;
        signInButton.interactable = false;
        await Login();
    }

    private async void SignUpUser()
    {
        if (emailInputField.text == "" || usernameInputField.text == "" || passwordInputField.text == "" ||
            signUpButton == null) return;
        signUpButton.interactable = false;
        await SignUp();
    }

    private async Task Login()
    {
        try
        {
            var email = emailInputField.text;
            var password = passwordInputField.text;

            var operation = await _beamContext.Accounts.RecoverAccountWithEmail(email, password);
            if (operation.isSuccess)
            {
                // Successfully recovered account, switch to the recovered account
                if (operation.account != null)
                {
                    await operation.SwitchToAccount();
                }
                else
                {
                    Debug.LogError("Recovered account is null. Cannot switch accounts.");
                    signInButton.interactable = true;
                    return;
                }

                SceneManager.LoadScene("SenetMainMenu"); 
            }
            else
            {
                // Failed to recover the account
                Debug.LogError($"Failed to recover account: {operation.error}");
                signInButton.interactable = true; 
            }
        }
        catch (Beamable.Api.PlatformRequesterException ex)
        {
            Debug.LogError($"Beamable API error: {ex.Message}");
            signInButton.interactable = true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            signInButton.interactable = true;
        }
    }


    private async Promise SignUp()
    {
        _loadingPanel.SetActive(true);
        string email = emailInputField.text;
        string userName = usernameInputField.text;
        string password = passwordInputField.text;
        string confirmPassword = confirmPasswordInputField.text;

        if (password != confirmPassword)
        {
            signUpButton.interactable = true;
            return;
        }

        try
        {
            await _beamContext.Api.AuthService.RegisterDBCredentials(email, password);
            await _beamContext.Accounts.AddExternalIdentity<SuiIdentity, Web3FederationClient>("");
            await _beamContext.Api.AuthService.Login(email, password);
            await _beamContext.Accounts.Current.SetAlias(userName);
            
            var userNameStat = new Dictionary<string, string>()
            {
                { "alias", userName }
            };

            await _beamContext.Api.StatsService.SetStats("public", userNameStat);

            //await WaitOneSecondAsync(); 

            _loadingPanel.SetActive(false);

            Debug.Log("Sign up successful.");

            SceneManager.LoadScene("SenetMainMenu");
        }
        catch (Exception e)
        {
            _loadingPanel.SetActive(false);
            Debug.LogError(e.ToString());
            signUpButton.interactable = true;
        }
    }

    private async Task WaitOneSecondAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(3));
    }

    public void GoToSignUpPage()
    {
        SceneManager.LoadScene("SenetSignUp");
    }

    public void GoToLogin()
    {
        SceneManager.LoadScene("SenetSignIn");
    }

    private void TogglePassword(bool isConfirm)
    {
        var inputField = isConfirm ? confirmPasswordInputField : passwordInputField;
        if (inputField.contentType == InputField.ContentType.Password)
        {
            inputField.contentType = InputField.ContentType.Standard;
        }
        else
        {
            inputField.contentType = InputField.ContentType.Password;
        }

        inputField.ForceLabelUpdate();
    }

    private void DisplayErrorMessage(string message)
    {
        errorMessageText.text = ParseErrorMessage(message);
        errorMessagePanel.SetActive(true); // Display the error message panel
        StartCoroutine(HideErrorMessageAfterDelay());
    }

    private IEnumerator HideErrorMessageAfterDelay()
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        errorMessagePanel.SetActive(false); // Hide the error message panel
    }

    private string ParseErrorMessage(string error)
    {
        switch (error)
        {
            case "ALREADY_HAS_CREDENTIAL":
                return "Account already exists.";
            case "UNKNOWN_ERROR":
                return "User doesn't exist";
            default:
                return error;
        }
    }
}
