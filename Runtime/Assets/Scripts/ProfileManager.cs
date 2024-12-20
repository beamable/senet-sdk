using System;
using System.Collections.Generic;
using Beamable;
using Beamable.Player;
using Beamable.Server.Clients;
using TMPro;
using UnityEngine;
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
        [SerializeField] private Image profileImage;
        [SerializeField] private Button uploadButton;
        [SerializeField] private string localImagePath;

        [Header("Edit Alias")]
        [SerializeField] private TMP_InputField aliasInputField;
        [SerializeField] private Button saveAliasButton;
        [SerializeField] private Button editAliasButton; 

        private bool isEditingAlias = false;

        private void Start()
        {
            InitializeBeamable();
            uploadButton.onClick.AddListener(OnUploadButtonClicked);
            saveAliasButton.onClick.AddListener(OnSaveAliasClicked);
            editAliasButton.onClick.AddListener(OnEditAliasClicked);
        }

        private async void InitializeBeamable()
        {
            try
            {
                _service = new ImageUploadServiceClient();
                _beamContext = await BeamContext.Default.Instance;
                await _beamContext.Accounts.OnReady;

                _playerAccount = _beamContext.Accounts.Current;
                DisplayPlayerProfile();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize Beamable: {ex.Message}");
            }
        }

        private void DisplayPlayerProfile()
        {
            if (_playerAccount != null)
            {
                var username = _playerAccount.Alias;
                var gid = _playerAccount.GamerTag.ToString();
                var shortenedGid = ShortenGamerTag(gid);
                var email = _playerAccount.Email ?? "Email not set";

                usernameText.text = username;
                gidText.text = shortenedGid;
                emailText.text = email;
                aliasInputField.text = username;

                aliasInputField.gameObject.SetActive(false); // Initially hide input field
                usernameText.gameObject.SetActive(true); // Show username text
            }
        }

        private void OnEditAliasClicked()
        {
            isEditingAlias = true;
            aliasInputField.gameObject.SetActive(true); // Show input field
            usernameText.gameObject.SetActive(false); // Hide username text
            editAliasButton.gameObject.SetActive(false); // Hide edit button
        }

        private async void OnSaveAliasClicked()
        {
            var newAlias = aliasInputField.text;

            if (string.IsNullOrEmpty(newAlias))
            {
                return;
            }

            try
            {
                var aliasStat = new Dictionary<string, string>
                {
                    { "alias", newAlias }
                };

                await _beamContext.Api.StatsService.SetStats("public", aliasStat);

                usernameText.text = newAlias;

                // Toggle UI back to view mode
                aliasInputField.gameObject.SetActive(false);
                usernameText.gameObject.SetActive(true);
                saveAliasButton.gameObject.SetActive(false);
                editAliasButton.gameObject.SetActive(true);

                isEditingAlias = false;
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
                    localImagePath = path;
                    UploadProfilePicture();
                }
            });
        }

        public async void UploadProfilePicture()
        {
            try
            {
                if (string.IsNullOrEmpty(localImagePath)) return;

                var hostedUrl = await _service.UploadImage(localImagePath);
                Debug.Log($"Profile picture uploaded successfully. Hosted URL: {hostedUrl}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to upload profile picture: {ex.Message}");
            }
        }

        private string ShortenGamerTag(string gid)
        {
            if (string.IsNullOrEmpty(gid) || gid.Length <= 12) return gid;

            var firstPart = gid[..9];
            var lastPart = gid[^3..];

            return $"{firstPart}...{lastPart}";
        }
    }
}
