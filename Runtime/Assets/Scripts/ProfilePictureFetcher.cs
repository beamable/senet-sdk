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
                    if (string.IsNullOrEmpty(_playerAccount.Alias))
                    {
                        var alias = await FetchAliasFromStats();
                        _playerAccount.SetAlias(alias);
                    }

                    if (usernameText != null)
                    {
                        usernameText.text = _playerAccount.Alias;
                        Debug.Log($"Alias displayed in UI: {_playerAccount.Alias}");
                    }
                    else
                    {
                        Debug.LogWarning("usernameText is not assigned in the Inspector.");
                    }

                    if (userEmail != null)
                    {
                        userEmail.text = _playerAccount.Email;
                        Debug.Log($"Email displayed in UI: {_playerAccount.Email}");
                    }
                    else
                    {
                        Debug.LogWarning("userEmail is not assigned in the Inspector.");
                    }
                }
                else
                {
                    Debug.LogWarning("No player account found.");
                }

                await FetchAndDisplayProfilePicture();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error initializing Beamable or fetching profile: {ex.Message}");
            }
        }

        private async Task<string> FetchAliasFromStats()
        {
            Debug.Log("Fetching alias from stats...");
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

        private async Task FetchAndDisplayProfilePicture()
        {
            Debug.Log("Fetching profile picture...");
            try
            {
                var playerId = BeamContext.Default.PlayerId;
                const string access = "public";
                const string domain = "client";
                const string type = "player";

                var stats = await _beamContext.Api.StatsService.GetStats(domain, access, type, playerId);

                if (stats.TryGetValue("ProfileUrl", out var profileUrl))
                {
                    await LoadImageFromUrl(profileUrl);
                }
                else
                {
                    Debug.Log("No profile picture URL found.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error fetching profile picture: {ex.Message}");
            }
        }

        public async Task LoadImageFromUrl(string url)
        {
            try
            {
                using var webRequest = UnityWebRequestTexture.GetTexture(url);
                var operation = webRequest.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    var texture = DownloadHandlerTexture.GetContent(webRequest);
                    if (texture != null)
                    {
                        var sprite = Sprite.Create(
                            texture,
                            new Rect(0, 0, texture.width, texture.height),
                            new Vector2(0.5f, 0.5f)
                        );

                        profileImage.sprite = sprite;
                        profileImage.preserveAspect = true;
                    }
                }
                else
                {
                    Debug.LogError($"Failed to load image from URL: {url}, Error: {webRequest.error}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading image from URL: {ex.Message}");
            }
        }
    }
}
