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

        private async void Start()
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
               if (_playerAccount == null)
               {
                   Debug.LogWarning("No player account found after account switch.");
                   return;
               }
       
               Debug.Log($"Player Account GamerTag: {_playerAccount.GamerTag}");
       
               if (string.IsNullOrEmpty(_playerAccount.Alias))
               {
                   Debug.Log("Alias is empty. Fetching alias from stats...");
                   var alias = await FetchAliasFromStats();
                   await _playerAccount.SetAlias(alias);
               }
       
               if (usernameText != null)
               {
                   usernameText.text = _playerAccount.Alias;
                   Debug.Log($"Alias displayed in UI: {_playerAccount.Alias}");
               }
       
               if (userEmail != null)
               {
                   userEmail.text = _playerAccount.Email;
                   Debug.Log($"Email displayed in UI: {_playerAccount.Email}");
               }
       
               await FetchAndDisplayProfilePicture();
           }
           catch (Exception ex)
           {
               Debug.LogError($"Error initializing Beamable or fetching profile: {ex.Message}");
               Debug.LogError($"Stack Trace: {ex.StackTrace}");
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

        private async Task FetchAndDisplayProfilePicture()
        {
            try
            {
                var playerId = BeamContext.Default.PlayerId;
                const string access = "public";
                const string domain = "client";
                const string type = "player";

                var stats = await _beamContext.Api.StatsService.GetStats(domain, access, type, playerId);

                if (stats.TryGetValue("profile_url", out var profileUrl))
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
