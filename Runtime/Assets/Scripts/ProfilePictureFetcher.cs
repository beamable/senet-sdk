using System;
using System.Threading.Tasks;
using Beamable;
using Beamable.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ProfilePictureFetcher : MonoBehaviour
    {
        [Header("Profile Picture UI")]
        [SerializeField] private Image profileImage;

        [Header("UI Text References")]
        [SerializeField] private TextMeshProUGUI usernameText;
        [SerializeField] private Text userEmail;

        private BeamContext _beamContext;
        private PlayerAccount _playerAccount;

        private async void OnEnable()
        {
            await InitializeBeamableAndFetchProfilePicture();
        }

        private async Task InitializeBeamableAndFetchProfilePicture()
        {
            try
            {
                _beamContext = await BeamContext.Default.Instance;
                await _beamContext.Accounts.OnReady;
                _playerAccount = _beamContext.Accounts.Current;

                if (_playerAccount != null)
                {
                    usernameText.text = _playerAccount.Alias;
                    userEmail.text = _playerAccount.Email;
                }

                await FetchAndDisplayProfilePicture();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize Beamable or fetch profile picture: {ex.Message}");
            }
        }

        private async Task FetchAndDisplayProfilePicture()
        {
            try
            {
                var playerId = BeamContext.Default.PlayerId;
                const string access = "public";
                const string domain = "client";
                const string type = "player";

                var stats = await _beamContext.Api.StatsService.GetStats(domain, access, type, playerId);

                if (stats.TryGetValue("ProfileUrl", out var profileUrl))
                {
                    Debug.Log($"Profile URL retrieved: {profileUrl}");
                    await LoadImageFromUrl(profileUrl);
                }
                else
                {
                    Debug.Log("ProfileUrl stat not found.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to fetch profile URL: {ex.Message}");
            }
        }

        private async Task LoadImageFromUrl(string url)
        {
            Debug.Log($"Starting image load from URL: {url}");

            profileImage.sprite = null;

            using var webRequest = UnityWebRequestTexture.GetTexture(url);
            var operation = webRequest.SendWebRequest();

            Debug.Log("Waiting for web request to complete...");
            while (!operation.isDone)
                await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Web request completed successfully.");

                var texture = DownloadHandlerTexture.GetContent(webRequest);
                if (texture != null)
                {
                    Debug.Log($"Texture loaded successfully. Dimensions: {texture.width}x{texture.height}");

                    var sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    // Apply the new sprite and force UI update.
                    profileImage.sprite = sprite;
                    profileImage.preserveAspect = true;

                    Debug.Log("Profile image updated successfully.");
                }
                else
                {
                    Debug.LogError("Failed to create texture from downloaded content.");
                }
            }
            else
            {
                Debug.LogError($"Failed to load image from URL: {url}. Error: {webRequest.error}");
            }
        }
    }
}
