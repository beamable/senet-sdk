using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.LeaderboardService
{
    public class LeaderBoardManager : MonoBehaviour
    {
        [SerializeField] private GameObject _board;
        [SerializeField] private GameObject _emptyBoard;
        [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;
        [SerializeField] private GameObject _currentPlayer;
        [SerializeField] private GameObject _firstPlacePlayer;
        [SerializeField] private GameObject _secondPlacePlayer;
        [SerializeField] private GameObject _thirdPlacePlayer;
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private Image _defaultProfilePicture;
        [SerializeField] private TextMeshProUGUI _prizePool;
        
        private static readonly Color NoOpacity = new Color32(255, 255, 255, 255);
        private static readonly Color TextColor = new Color32(157, 149, 172, 255);
        private static readonly Color TextColorWithOpacity = new Color32(157, 149, 172, 60);
        private static readonly Color GrayedOut = new Color32(200, 200, 200, 255);

        async void Start()
        {
            var beamContext = BeamContext.Default;
            await beamContext.OnReady;
            await beamContext.Accounts.OnReady;
            var account = beamContext.Accounts.Current;

            beamContext.Api.EventsService.Subscribe(async eventsGetResponse =>
            {
                var players = await eventsGetResponse.GetTournamentPlayers();
                UpdateLeaderboard(players, account.GamerTag);
            });
            
            var firstPlaceReward = TournamentManager.instance.GetRunningFirstPlaceReward(); 
        
            _prizePool.text = $"{firstPlaceReward}"; 
        }

        private void UpdateLeaderboard(List<PlayerModel> players, long currentPlayerId)
        {
            _board.SetActive(players.Count > 0);
            _emptyBoard.SetActive(players.Count == 0);

            ClearPreviousEntries();

            foreach (var player in players)
            {
                var place = GetPlayerSlot(player.rank);
                AssignPlayerData(place, player);

                if (player.id != currentPlayerId) continue;
                _currentPlayer.SetActive(true);
                AssignPlayerData(_currentPlayer, player, true);
            }

            CreatePlaceholders(players.Count > 0 ? (int)players[^1].rank : 0);
        }

        private void ClearPreviousEntries()
        {
            foreach (Transform child in _verticalLayoutGroup.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private GameObject GetPlayerSlot(long rank)
        {
            return rank switch
            {
                1 => _firstPlacePlayer,
                2 => _secondPlacePlayer,
                3 => _thirdPlacePlayer,
                _ => Instantiate(_playerPrefab, _verticalLayoutGroup.transform)
            };
        }

        private async void AssignPlayerData(GameObject playerObject, PlayerModel player, bool ignoreColorsAndName = false)
        {
            if (playerObject == null) return;

            playerObject.transform.Find("Rank").GetComponent<TMP_Text>().text = player.rank.ToString();
            playerObject.transform.Find("Score").GetComponent<TMP_Text>().text = player.score.ToString();

            if (!ignoreColorsAndName)
            {
                var nameText = playerObject.transform.Find("Name")?.GetComponent<TMP_Text>();
                if (nameText != null) nameText.text = player.name;

                var isPlaceholder = player.name == "Waiting for players...";
                var textColor = isPlaceholder ? TextColorWithOpacity : TextColor;
                SetTextColor(playerObject, textColor);
            }

            await SetProfileImage(playerObject, player.id);
        }

        private static void SetTextColor(GameObject playerObject, Color color)
        {
            playerObject.transform.Find("Rank").GetComponent<TMP_Text>().color = color;
            playerObject.transform.Find("Name").GetComponent<TMP_Text>().color = color;
            playerObject.transform.Find("Score").GetComponent<TMP_Text>().color = color;
        }

        private async Task SetProfileImage(GameObject playerObject, long playerId)
        {
            var profileImageTransform = playerObject.transform.Find("Logo/Profile Mask/Profile");
            if (profileImageTransform == null) return;

            var profileImage = profileImageTransform.GetComponent<Image>();
            playerObject.transform.Find("Logo/Picture Border").GetComponent<Image>().color = NoOpacity;

            var profileUrl = await ProfilePictureUtility.FetchProfilePictureUrl(playerId);
            if (!string.IsNullOrEmpty(profileUrl))
            {
                await ProfilePictureUtility.LoadImageFromUrl(profileUrl, profileImage);
            }
            else
            {
                ProfilePictureUtility.SetIconToFillParent(profileImage, _defaultProfilePicture);
            }
        }

        private void CreatePlaceholders(int lastRank)
        {
            for (var i = (lastRank < 4 ? 4 : lastRank + 1); i <= 7; i++)
            {
                var placeholder = Instantiate(_playerPrefab, _verticalLayoutGroup.transform);
                if (placeholder == null) continue;

                AssignPlaceholderData(placeholder, i);
            }
        }

        private void AssignPlaceholderData(GameObject placeholder, int rank)
        {
            placeholder.transform.Find("Rank").GetComponent<TMP_Text>().text = rank.ToString();
            placeholder.transform.Find("Name").GetComponent<TMP_Text>().text = "Waiting for players...";
            placeholder.transform.Find("Score").GetComponent<TMP_Text>().text = "";

            var icon = placeholder.transform.Find("Logo/Profile Mask/Profile")?.GetComponent<Image>();
            if (icon != null) ProfilePictureUtility.SetIconToFillParent(icon, _defaultProfilePicture);

            SetTextColor(placeholder, TextColorWithOpacity);
        }
    }
}
