using Assets.Scripts.Constants;
using Beamable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField]
    private GameObject errorMessagePanel;
    [SerializeField]
    private TMP_Text errorMessageText;
    [SerializeField]
    private GameObject loadingPanel;
    [SerializeField]
    private GameObject betaPopup;
    [SerializeField]
    private Button betaPopupButton;

    private BeamContext _beamContext;

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
            betaPopupButton.onClick.AddListener(NavigateToMainMenu);
        }

        togglePassword.onClick.AddListener(() => TogglePassword(false));

        if (toggleConfirmPassword != null)
        {
            toggleConfirmPassword.onClick.AddListener(() => TogglePassword(true));
        }
    }

    private async void LoginUser()
    {
        if (string.IsNullOrWhiteSpace(emailInputField.text) || string.IsNullOrWhiteSpace(passwordInputField.text))
        {
            DisplayErrorMessage(ErrorMessages.FieldsMustBeFilled);
            return;
        }
        
        signInButton.interactable = false;
        await Login();
    }

    private async void SignUpUser()
    {
        if (string.IsNullOrWhiteSpace(emailInputField.text) || string.IsNullOrWhiteSpace(usernameInputField.text) || 
            string.IsNullOrWhiteSpace(passwordInputField.text) || string.IsNullOrWhiteSpace(confirmPasswordInputField.text))
        {
            DisplayErrorMessage(ErrorMessages.FieldsMustBeFilled);
            return;
        }
        
        signUpButton.interactable = false;
        await SignUp();
    }

    private async Task Login()
    {
        try
        {
            var email = emailInputField.text.Trim();
            var password = passwordInputField.text;

            if (!IsValidEmail(email))
            {
                DisplayErrorMessage(ErrorMessages.InvalidEmailFormat);
                signInButton.interactable = true;
                return;
            }

            var operation = await _beamContext.Accounts.RecoverAccountWithEmail(email, password);
            if (operation.isSuccess)
            {
                if (operation.account != null)
                {
                    await operation.SwitchToAccount();
                }
                else
                {
                    Debug.LogError(ErrorMessages.AccountSwitchError);
                    DisplayErrorMessage(ErrorMessages.LoginError);
                    signInButton.interactable = true;
                    return;
                }

                ShowBetaPopup();
            }
            else
            {
                switch (operation.error.ToString())
                {
                    case "UNKNOWN_ERROR":
                        DisplayErrorMessage(ErrorMessages.AccountDoesNotExist);
                        break;
                    case "UNKNOWN_CREDENTIALS":
                        DisplayErrorMessage(ErrorMessages.IncorrectPassword);
                        break;
                    default:
                        DisplayErrorMessage(ErrorMessages.FailedToRecoverAccount);
                        break;
                }

                Debug.LogError($"Failed to recover account: {operation.error}");
                signInButton.interactable = true;
            }
        }
        catch (Exception e)
        {
            DisplayErrorMessage(ErrorMessages.UnableToLogin);
            Debug.LogError(e.ToString());
            signInButton.interactable = true;
        }
    }

    private void ShowBetaPopup()
    {
        betaPopup.SetActive(true);
    }

    private void NavigateToMainMenu()
    {
        betaPopup.SetActive(false);
        SceneManager.LoadScene("SenetMainMenu");
    }

    private static bool IsValidEmail(string email)
    {
        return email.Contains("@") && email.Contains(".");
    }

    private async Task SignUp()
    {
        loadingPanel.SetActive(true);
        var email = emailInputField.text.Trim();
        var userName = usernameInputField.text.Trim();
        var password = passwordInputField.text;
        var confirmPassword = confirmPasswordInputField.text;

        if (!IsValidEmail(email))
        {
            loadingPanel.SetActive(false);
            DisplayErrorMessage(ErrorMessages.InvalidEmailFormat);
            signUpButton.interactable = true;
            return;
        }

        if (password != confirmPassword)
        {
            loadingPanel.SetActive(false);
            DisplayErrorMessage(ErrorMessages.PasswordsDoNotMatch);
            signUpButton.interactable = true;
            return;
        }

        try
        {
            await _beamContext.Api.AuthService.RegisterDBCredentials(email, password);
            // await _beamContext.Accounts.AddExternalIdentity<SuiIdentity, Web3FederationClient>("");
            await _beamContext.Api.AuthService.Login(email, password);
            await _beamContext.Accounts.Current.SetAlias(userName);

            var userNameStat = new Dictionary<string, string>()
            {
                { "alias", userName }
            };

            await _beamContext.Api.StatsService.SetStats("public", userNameStat);

            loadingPanel.SetActive(false);

            Debug.Log("Sign up successful.");
            SceneManager.LoadScene("SenetMainMenu");
        }
        catch (Beamable.Api.PlatformRequesterException ex)
        {
            loadingPanel.SetActive(false);
            DisplayErrorMessage(ex.Message.Contains("EmailAlreadyRegisteredError")
                ? ErrorMessages.EmailAlreadyRegistered
                : ErrorMessages.SignUpError);
            Debug.LogError($"Beamable API error: {ex.Message}");
            signUpButton.interactable = true;
        }
        catch (Exception e)
        {
            loadingPanel.SetActive(false);
            DisplayErrorMessage(ErrorMessages.SignUpError);
            Debug.LogError(e.ToString());
            signUpButton.interactable = true;
        }
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
        inputField.contentType = inputField.contentType == InputField.ContentType.Password
            ? InputField.ContentType.Standard
            : InputField.ContentType.Password;

        inputField.ForceLabelUpdate();
    }

    private void DisplayErrorMessage(string message)
    {
        errorMessageText.text = ParseErrorMessage(message);
        errorMessagePanel.SetActive(true);
        StartCoroutine(HideErrorMessageAfterDelay());
    }

    private IEnumerator HideErrorMessageAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        errorMessagePanel.SetActive(false);
    }

    private string ParseErrorMessage(string error)
    {
        return error switch
        {
            "ALREADY_HAS_CREDENTIAL" => "Account already exists.",
            "UNKNOWN_ERROR" => "User doesn't exist",
            _ => error
        };
    }
}
