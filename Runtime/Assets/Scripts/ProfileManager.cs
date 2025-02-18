using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Beamable;
using Beamable.Player;
using Beamable.Server.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ProfileManager : MonoBehaviour
    {
        private BeamContext _beamContext;
        private PlayerAccount _playerAccount;
        private ImageUploadServiceClient _service;

        [Header("UI Text References")]
        [SerializeField] private TextMeshProUGUI usernameText;
        [SerializeField] private TextMeshProUGUI gidText;
        [SerializeField] private TextMeshProUGUI emailText;

        [Header("Profile Picture")]
        [SerializeField] private Button uploadButton;
        [SerializeField] private ProfilePictureFetcher profilePictureFetcher;
        [SerializeField] private Image profilePicture;
        private string _localImagePath;

        [Header("Edit Alias")]
        [SerializeField] private TMP_InputField aliasInputField;
        [SerializeField] private Button editAliasButton;

        [Header("Navigation")]
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject confirmPopup;
        [SerializeField] private GameObject warningPopup;
        [SerializeField] private Button confirmChangesButton;
        [SerializeField] private Button saveChangesButton;
        [SerializeField] private Button discardChangesButton;

        
        [Header("Loading")]
        [SerializeField] private GameObject loadingPanel;
        
        private bool _isEditingAlias;

        private async void Start()
        {
            await InitializeBeamable();
            SetupButtonListeners();
            saveChangesButton.interactable = false;
        }

        private void SetupButtonListeners()
        {
            uploadButton.onClick.AddListener(OnUploadButtonClicked);
            editAliasButton.onClick.AddListener(OnEditAliasClicked);
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            confirmChangesButton.onClick.AddListener(OnSaveChanges);
            discardChangesButton.onClick.AddListener(OnDiscardChanges);
            saveChangesButton.onClick.AddListener(OnSaveAliasButtonClicked);
        }
        
        private async Task InitializeBeamable()
        {
            try
            {
                _service = new ImageUploadServiceClient();
                _beamContext = await BeamContext.Default.Instance;
                await _beamContext.Accounts.OnReady;
                _playerAccount = _beamContext.Accounts.Current;
                await DisplayPlayerProfile();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize Beamable: {ex.Message}");
            }
        }

        private async Task DisplayPlayerProfile()
        {
            if (_playerAccount != null)
            {
                if (string.IsNullOrEmpty(_playerAccount.Alias))
                {
                    var alias = await profilePictureFetcher.FetchAliasFromStats(); 
                    await _playerAccount.SetAlias(alias);
                }
                usernameText.text = _playerAccount.Alias;
                gidText.text = ShortenGamerTag(_playerAccount.GamerTag.ToString());
                emailText.text = _playerAccount.Email ?? "Email not set";
                aliasInputField.text = _playerAccount.Alias;

                ToggleUIElements(true);
            }
        }

        private void ToggleUIElements(bool isViewing)
        {
            aliasInputField.gameObject.SetActive(!isViewing);
            usernameText.gameObject.SetActive(isViewing);
            editAliasButton.gameObject.SetActive(isViewing);
        }

        private void OnSaveAliasButtonClicked()
        {
            _ = OnSaveAliasClicked();
        }
        
        private void OnEditAliasClicked()
        {
            _isEditingAlias = true;
            ToggleUIElements(false);
            saveChangesButton.interactable = true;
        }

        private void OnCloseButtonClicked()
        {
            if (_isEditingAlias)
            {
                confirmPopup.SetActive(true);
            }
            else
            {
                NavigateToHome();
            }
        }

        private async void OnSaveChanges()
        {
            await OnSaveAliasClicked();
            ClosePopupAndNavigate();
        }

        private void OnDiscardChanges()
        {
            aliasInputField.text = usernameText.text;
            ClosePopupAndNavigate();
        }

        private void ClosePopupAndNavigate()
        {
            confirmPopup.SetActive(false);
            NavigateToHome();
        }

        private void NavigateToHome()
        {
            SceneManager.LoadSceneAsync("SenetMainMenu");
        }

        private async Task OnSaveAliasClicked()
        {
            var newAlias = aliasInputField.text;
            if (string.IsNullOrEmpty(newAlias)) return;

            try
            {
                await _playerAccount.SetAlias(newAlias);

                usernameText.text = newAlias;

                var userNameStat = new Dictionary<string, string>()
                {
                    { "alias", newAlias }
                };

                await _beamContext.Api.StatsService.SetStats("public", userNameStat);

                ToggleUIElements(true); // Revert to viewing mode
                saveChangesButton.interactable = false;
                _isEditingAlias = false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to update alias: {ex.Message}");
            }
        }

        private void OnUploadButtonClicked()
        {
            NativeGallery.GetImageFromGallery((path) =>
            {
                if (string.IsNullOrEmpty(path)) return;
                
                const int maxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB size limit
                
                _localImagePath = path;
                
                if (!File.Exists(_localImagePath))
                {
                    Debug.LogError($"File not found: {_localImagePath}");
                    return;
                }
                
                var fileInfo = new FileInfo(_localImagePath);
                if (fileInfo.Length > maxFileSizeInBytes)
                {
                    DisplayTemporaryPopup();
                    return;
                }
                
                UploadProfilePicture();
            });
        }
        
        private void DisplayTemporaryPopup()
        {
            warningPopup.SetActive(true);

            StartCoroutine(ClosePopupAfterDelay(warningPopup, 3f));
        }
        
        private static IEnumerator ClosePopupAfterDelay(GameObject popup, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(popup);
        }

        private async void UploadProfilePicture()
        {
            if (string.IsNullOrEmpty(_localImagePath)) return;

            loadingPanel.SetActive(true);

            try
            {
                var image = await File.ReadAllBytesAsync(_localImagePath);

                var md5Bytes = GetMd5Checksum(image);
                var renderChecksum = BitConverter.ToString(md5Bytes).Replace("-", "");

                var hostedUrl = await _service.UploadImage(renderChecksum, image, md5Bytes);

                var statsDictionary = new Dictionary<string, string> { { "profile_url", hostedUrl } };
                await _beamContext.Api.StatsService.SetStats("public", statsDictionary);

                await ProfilePictureUtility.LoadImageFromUrl(hostedUrl, profilePicture);
 
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to upload profile picture: {ex.Message}");
                DisplayErrorMessage("Failed to upload the profile picture. Please try again.");
            }
            finally
            {
                loadingPanel.SetActive(false);
            }
        }
        
        private void DisplayErrorMessage(string message)
        {
            warningPopup.SetActive(true);

            var messageText = warningPopup.transform
                .Find("Panel/Error Message") 
                .GetComponent<TextMeshProUGUI>();

            if (messageText != null)
            {
                messageText.text = message; 
            }
            else
            {
                Debug.LogError("Error Message TextMeshPro component not found.");
            }

            StartCoroutine(ClosePopupAfterDelay(warningPopup, 3f)); // Close the popup after 3 seconds
        }


        private static byte[] GetMd5Checksum(byte[] image)
        {
            using var md5 = MD5.Create();
            return md5.ComputeHash(image);
        }

        private static string ShortenGamerTag(string gid)
        {
            if (string.IsNullOrEmpty(gid) || gid.Length <= 12) return gid;

            return $"{gid[..9]}...{gid[^3..]}";
        }
    }
}
