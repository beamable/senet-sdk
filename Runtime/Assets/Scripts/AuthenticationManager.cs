using System;
using System.Collections;
using Beamable;
using Beamable.Common;
using Beamable.Server.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Web3FederationCommon;

public class AuthenticationManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField usernameField;
    [SerializeField]
    private TMP_InputField passwordField;
    [SerializeField]
    private TMP_InputField confirmPasswordField;
    [SerializeField]
    private Button loginButton;
    [SerializeField]
    private Button signUpButton;
    private BeamContext _beamContext;
    [SerializeField]
    private GameObject errorMessagePanel;
    [SerializeField]
    private TMP_Text errorMessageText;

    private async void Start()
    {
        _beamContext = BeamContext.Default;
        await _beamContext.OnReady;
        await _beamContext.Accounts.OnReady;

        if (signUpButton != null)
        {
            signUpButton.interactable = true;
        }
        else
        {
            loginButton.interactable = true;
        }
    }

    public async void LoginUser()
    {
        if (usernameField.text != "" && usernameField.text != "Anonymous" && loginButton != null)
        {
            loginButton.interactable = false;
            await Login();
        }
    }

    public async void SignUpUser()
    {
        if (usernameField.text != "" && passwordField.text != "" && signUpButton != null)
        {
            signUpButton.interactable = false;
            await SignUp();
        }
    }

    private async Promise Login()
    {
        try
        {

            await _beamContext.Api.AuthService.Login(usernameField.text, passwordField.text);
            SceneManager.LoadScene("MainMenu");
        }
        catch (Exception e)
        {
            DisplayErrorMessage(e.ToString());
            loginButton.interactable = true;
        }
    }

    private async Promise SignUp()
    {
        string email = usernameField.text;
        string password = passwordField.text;
        string confirmPassword = confirmPasswordField.text;

        if (password != confirmPassword)
        {
            DisplayErrorMessage("Passwords do not match.");
            signUpButton.interactable = true;
            return;
        }

        try
        {
            await _beamContext.Api.AuthService.RegisterDBCredentials(email, password);
            await _beamContext.Accounts.AddExternalIdentity<SuiIdentity, Web3FederationClient>("");
            await _beamContext.Api.AuthService.Login(email, password);
            Debug.Log("Sign up successful.");
            SceneManager.LoadScene("MainMenu");
        }
        catch (Exception e)
        {
            DisplayErrorMessage(e.ToString());
            signUpButton.interactable = true;
        }
    }

    public void GoToSignUpPage()
    {
        SceneManager.LoadScene("SenetSignUp");
    }

    public void GoToLogin()
    {
        SceneManager.LoadScene("SenetLogin");
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
