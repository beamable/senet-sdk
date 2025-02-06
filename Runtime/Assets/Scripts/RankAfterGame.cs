using Beamable;
using System.Collections.Generic;
using System.Linq;
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


    async void Start()
    {
        if (TournamentManager.instance)
        {
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
    }

    private void ShowRankPanel(List<PlayerModel> players, long currentPlayerId)
    {
        _rankAfterGamePanel.SetActive(true);

        var currentPlayer = players.Where(i => i.id == currentPlayerId).FirstOrDefault();

        _rank.text = ToOrdinal(currentPlayer.rank);

        Color noOpacity = new Color32(255, 255, 255, 255);
        Color opacity = new Color32(255, 255, 255, 60);

        Color textColor = new Color32(157, 149, 172, 255);
        Color textColorWithOpacity = new Color32(157, 149, 172, 60);

        if (currentPlayer.rank == 1 || currentPlayer.rank > 3)
        {
            _currentPlayer.transform.GetChild(0).GetComponent<Text>().text = currentPlayer.rank.ToString();
            _currentPlayer.transform.GetChild(3).GetComponent<Text>().text = currentPlayer.score.ToString();
            Instantiate(_currentPlayer, _verticalLayoutGroup.transform);

            for (long i = currentPlayer.rank + 1; i <= currentPlayer.rank + 2; i++)
            {
                var rank = _playerPrefab.transform.GetChild(1).GetComponent<Text>();
                var icon = _playerPrefab.transform.GetChild(2).GetComponent<Image>();
                var name = _playerPrefab.transform.GetChild(3).GetComponent<Text>();
                var score = _playerPrefab.transform.GetChild(4).GetComponent<Text>();

                var player = players.Where(p => p.rank == i).FirstOrDefault();
                rank.text = $"{i}";

                if (player != null)
                {
                    icon.color = noOpacity;
                    score.text = player.score.ToString();
                    name.text = player.name;

                    rank.color = textColor;
                    name.color = textColor;
                    score.color = textColor;
                }
                else
                {
                    icon.color = opacity;
                    score.text = "";
                    name.text = "Waiting for players...";

                    rank.color = textColorWithOpacity;
                    name.color = textColorWithOpacity;
                    score.color = textColorWithOpacity;
                }

                Instantiate(_playerPrefab, _verticalLayoutGroup.transform);
            }
        }
        else
        {
            for (int i = 1; i <= 3; i++)
            {
                var player = players.Where(p => p.rank == i).FirstOrDefault();

                var rank = _playerPrefab.transform.GetChild(1).GetComponent<Text>();
                var icon = _playerPrefab.transform.GetChild(2).GetComponent<Image>();
                var name = _playerPrefab.transform.GetChild(3).GetComponent<Text>();
                var score = _playerPrefab.transform.GetChild(4).GetComponent<Text>();

                rank.text = $"{i}";

                if (player != null)
                {
                    if (player.rank == currentPlayer.rank)
                    {
                        _currentPlayer.transform.GetChild(0).GetComponent<Text>().text = currentPlayer.rank.ToString();
                        _currentPlayer.transform.GetChild(3).GetComponent<Text>().text = currentPlayer.score.ToString();
                        Instantiate(_currentPlayer, _verticalLayoutGroup.transform);
                    }
                    else
                    {
                        icon.color = noOpacity;
                        score.text = player.score.ToString();
                        name.text = player.name;

                        rank.color = textColor;
                        name.color = textColor;
                        score.color = textColor;
                        Instantiate(_playerPrefab, _verticalLayoutGroup.transform);
                    }
                }
                else
                {
                    icon.color = opacity;
                    score.text = "";
                    name.text = "Waiting for players...";

                    rank.color = textColorWithOpacity;
                    name.color = textColorWithOpacity;
                    score.color = textColorWithOpacity;
                    Instantiate(_playerPrefab, _verticalLayoutGroup.transform);
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
        long lastDigit = number % 10;
        long lastTwoDigits = number % 100;

        if (lastTwoDigits == 11 || lastTwoDigits == 12 || lastTwoDigits == 13)
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
}
