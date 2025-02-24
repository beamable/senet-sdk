using Beamable;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankAfterGame : MonoBehaviour
{
    [SerializeField]
    private GameObject _currentPlayer;
    [SerializeField]
    private GameObject _rankAfterGamePanel;
    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    private TMP_Text _rank;
    [SerializeField]
    private VerticalLayoutGroup _verticalLayoutGroup;
    [SerializeField]
    private Image _defaultProfilePicture;
    
    async void Start()
    {
        if (!TournamentManager.instance) return;
        await Task.Delay(500);

        if (TournamentManager.instance.isPlacementBoardOpen)
        {
            var beamContext = BeamContext.Default;
            await beamContext.OnReady;
            await beamContext.Accounts.OnReady;
            var account = beamContext.Accounts.Current;

            var eventsGetResponse = await beamContext.Api.EventsService.GetCurrent();
            var players = await eventsGetResponse.GetTournamentPlayers();

            ShowRankPanel(players, account.GamerTag);
        }
        else
        {
            _rankAfterGamePanel.SetActive(false);
        }
    }

    private async void ShowRankPanel(List<PlayerModel> players, long currentPlayerId)
    {
        _rankAfterGamePanel.SetActive(true);

        var currentPlayer = players.FirstOrDefault(i => i.id == currentPlayerId);

        if (currentPlayer == null) return;
        {
            _rank.text = ToOrdinal(currentPlayer.rank);

            if (currentPlayer.rank is 1 or > 3)
            {
                await InstantiatePlayer(_currentPlayer, _verticalLayoutGroup.transform, currentPlayer);

                for (var i = currentPlayer.rank + 1; i <= currentPlayer.rank + 2; i++)
                {
                    var player = players.FirstOrDefault(p => p.rank == i);
                    await InstantiatePlayer(_playerPrefab, _verticalLayoutGroup.transform, player, i);
                }
            }
            else
            {
                for (var i = 1; i <= 3; i++)
                {
                    var player = players.FirstOrDefault(p => p.rank == i);
                    
                    if (player?.rank == currentPlayer.rank)
                    {
                        await InstantiatePlayer(_currentPlayer, _verticalLayoutGroup.transform, currentPlayer);
                    }
                    else
                    {
                        await InstantiatePlayer(_playerPrefab, _verticalLayoutGroup.transform, player);
                    }
                }
            }
        }
    }

    public void ClosePanel()
    {
        TournamentManager.instance.isPlacementBoardOpen = false;
        _rankAfterGamePanel.SetActive(false);
    }

    private static string ToOrdinal(long number)
    {
        if (number <= 0) return number.ToString();

        string suffix;
        var lastDigit = number % 10;
        var lastTwoDigits = number % 100;

        if (lastTwoDigits is 11 or 12 or 13)
        {
            suffix = "th";
        }
        else
        {
            suffix = lastDigit switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th",
            };
        }

        return $"{number}{suffix}";
    }
    
    private async Task SetPlayerDetails(GameObject playerObj, PlayerModel player)
    {
        var rank = playerObj.transform.Find("Rank")?.GetComponent<TMP_Text>();
        var name = playerObj.transform.Find("Name")?.GetComponent<TMP_Text>();
        var score = playerObj.transform.Find("Score")?.GetComponent<TMP_Text>();
        var icon = playerObj.transform.Find("Logo/Profile Mask/Profile")?.GetComponent<Image>();

        if (rank != null) rank.text = player.rank.ToString();
        if (name != null) name.text = player.name;
        if (score != null) score.text = player.score.ToString();

        await LoadProfilePicture(player.id, icon);
    }


    private async Task LoadProfilePicture(long playerId, Image imageComponent)
    {
        var url = await ProfilePictureUtility.FetchProfilePictureUrl(playerId);
        if (!string.IsNullOrEmpty(url))
        {
            await ProfilePictureUtility.LoadImageFromUrl(url, imageComponent);
        }
        else
        {
            ProfilePictureUtility.SetIconToFillParent(imageComponent, _defaultProfilePicture);
        }
    }

    private async Task InstantiatePlayer(GameObject prefab, Transform parent, PlayerModel player, long rankNumber = 0)
    {
        var playerInstance = Instantiate(prefab, parent);
        if (player != null)
        {
            await SetPlayerDetails(playerInstance, player);
        }
        else
        {
            SetPlaceholderDetails(playerInstance, rankNumber);
        }
    }

    private void SetPlaceholderDetails(GameObject playerObj, long rankNumber)
    {
        var rank = playerObj.transform.Find("Rank").GetComponent<TMP_Text>();
        var name = playerObj.transform.Find("Name").GetComponent<TMP_Text>();
        var score = playerObj.transform.Find("Score").GetComponent<TMP_Text>();
        var icon = playerObj.transform.Find("Logo/Profile Mask/Profile")?.GetComponent<Image>();

        rank.text = $"{rankNumber}";
        ProfilePictureUtility.SetIconToFillParent(icon, _defaultProfilePicture);
        icon.color = new Color32(200, 200, 200, 255);

        rank.color = new Color32(157, 149, 172, 60);
        name.color = new Color32(157, 149, 172, 60);
        score.color = new Color32(157, 149, 172, 60);

        name.text = "Waiting for players...";
        score.text = "";
    }
}
