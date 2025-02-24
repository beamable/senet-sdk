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
        [SerializeField] private TextMeshProUGUI gid;
        [SerializeField] private TextMeshProUGUI userEmail;

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

                if (string.IsNullOrEmpty(_playerAccount.Alias))
                {
                    var alias = await FetchAliasFromStats();
                    await _playerAccount.SetAlias(alias);
                }

                if (usernameText != null)
                {
                    usernameText.text = _playerAccount.Alias;
                }

                if (userEmail != null)
                {
                    userEmail.text = _playerAccount.Email;
                }

                if (gid != null)
                {
                    gid.text = $"GID: {_playerAccount.GamerTag.ToString()}";
                }

                await FetchAndDisplayProfilePicture();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error initializing Beamable or fetching profile: {ex.Message}");
                Debug.LogError($"Stack Trace: {ex.StackTrace}");
            }
        }

        public async Task<string> FetchAliasFromStats()
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
                var profileUrl = await ProfilePictureUtility.FetchProfilePictureUrl(playerId);

                if (!string.IsNullOrEmpty(profileUrl))
                {
                    await ProfilePictureUtility.LoadImageFromUrl(profileUrl, profileImage);

                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error fetching profile picture: {ex.Message}");
            }
        }
    }
}
