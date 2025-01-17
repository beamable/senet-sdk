using System;
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
        private string _localImagePath;

        [Header("Edit Alias")]
        [SerializeField] private TMP_InputField aliasInputField;
        [SerializeField] private Button editAliasButton;

        [Header("Navigation")]
        [SerializeField] private Button closeButton; 
        [SerializeField] private GameObject confirmPopup; 
        [SerializeField] private Button confirmChangesButton; 
        [SerializeField] private Button saveChangesButton; 
        [SerializeField] private Button discardChangesButton; 

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
                    var alias = await FetchAliasFromStats();
                    await _playerAccount.SetAlias(alias);
                }
                usernameText.text = _playerAccount.Alias;
                gidText.text = ShortenGamerTag(_playerAccount.GamerTag.ToString());
                emailText.text = _playerAccount.Email ?? "Email not set";
                aliasInputField.text = _playerAccount.Alias;

                ToggleUIElements(true);
            }
        }
        
        private async Task<string> FetchAliasFromStats()
        {
            try
            {
                var playerId = BeamContext.Default.PlayerId;
                const string access = "public";
                const string domain = "client";
                const string type = "player";

                var stats = await _beamContext.Api.StatsService.GetStats(domain, access, type, playerId);

                if (stats.TryGetValue("alias", out var alias))
                {
                    return alias;
                }
                else
                {
                    Debug.Log("Alias not found in stats.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error fetching alias from stats: {ex.Message}");
            }

            return "Unknown";
        }

        private void ToggleUIElements(bool isViewing)
        {
            aliasInputField.gameObject.SetActive(!isViewing); 
            usernameText.gameObject.SetActive(isViewing); 
            editAliasButton.gameObject.SetActive(isViewing);
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
            Debug.Log("Navigating to home...");
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
                    { "alias", newAlias}
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
                if (!string.IsNullOrEmpty(path))
                {
                    _localImagePath = path;
                    UploadProfilePicture();
                }
            });
        }

        private async void UploadProfilePicture()
        {
            if (string.IsNullOrEmpty(_localImagePath)) return;

            try
            {
                if (!File.Exists(_localImagePath))
                {
                    Debug.LogError($"File not found: {_localImagePath}");
                    return;
                }

                var image = await File.ReadAllBytesAsync(_localImagePath);
                
                var md5Bytes = GetMd5Checksum(image);
                var renderChecksum = BitConverter.ToString(md5Bytes).Replace("-", "");
                
                var hostedUrl = await _service.UploadImage(renderChecksum, image, md5Bytes);
                Debug.Log($"Profile picture uploaded successfully. Hosted URL: {hostedUrl}");

                // Write the hosted URL to stats
                var statsDictionary = new Dictionary<string, string> { { "profile_url", hostedUrl } };
                await _beamContext.Api.StatsService.SetStats("public", statsDictionary);

                // Trigger the ProfilePictureFetcher to load the new picture
                await profilePictureFetcher.LoadImageFromUrl(hostedUrl);

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to upload profile picture: {ex.Message}");
            }
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
