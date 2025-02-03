using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.LeaderboardService
{
    public class LeaderBoardManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _board;
        [SerializeField]
        private GameObject _emptyBoard;

        [SerializeField]
        private VerticalLayoutGroup _verticalLayoutGroup;
        [SerializeField]
        private GameObject _currentPlayer;
        [SerializeField]
        private GameObject _firstPlacePlayer;
        [SerializeField]
        private GameObject _secondPlacePlayer;
        [SerializeField]
        private GameObject _thirdPlacePlayer;
        [SerializeField]
        private GameObject _playerPrefab;

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
        }

        void UpdateLeaderboard(List<PlayerModel> players, long currentPlayerId)
        {
            Color noOpacity = new Color32(255, 255, 255, 255);
            Color opacity = new Color32(255, 255, 255, 60);

            Color textColor = new Color32(157, 149, 172, 255);
            Color textColorWithOpacity = new Color32(157, 149, 172, 60);

            if (players.Count > 0)
            {
                _board.SetActive(true);
                _emptyBoard.SetActive(false);

                foreach (var player in players)
                {
                    if (player.rank > 0 && player.rank < 4)
                    {

                        var place = player.rank == 1 ? _firstPlacePlayer : player.rank == 2 ? _secondPlacePlayer : _thirdPlacePlayer;
                        var icon = place.GetComponent<Image>();

                        var info = place.transform.GetChild(1).GetComponent<TMP_Text>();

                        icon.color = noOpacity;

                        info.text = $"{player.name}<br>{player.score}";
                    }

                    if (player.rank > 3)
                    {
                        var rank = _playerPrefab.transform.GetChild(1).GetComponent<Text>();
                        var icon = _playerPrefab.transform.GetChild(2).GetComponent<Image>();
                        var name = _playerPrefab.transform.GetChild(3).GetComponent<Text>();
                        var score = _playerPrefab.transform.GetChild(4).GetComponent<Text>();

                        icon.color = noOpacity;

                        rank.text = $"{player.rank}";
                        name.text = player.name;
                        score.text = $"{player.score}";

                        rank.color = textColor;
                        name.color = textColor;
                        score.color = textColor;

                        Instantiate(_playerPrefab, _verticalLayoutGroup.transform);
                    }

                    if (player.id == currentPlayerId)
                    {
                        _currentPlayer.SetActive(true);
                        _currentPlayer.transform.GetChild(0).GetComponent<Text>().text = player.rank.ToString();
                        _currentPlayer.transform.GetChild(3).GetComponent<Text>().text = player.score.ToString();
                    }
                }

                var lastPlayerRank = players[^1].rank;

                if (lastPlayerRank <= 10)
                {
                    for (var i = lastPlayerRank < 4 ? 4 : lastPlayerRank + 1; i <= 10; i++)
                    {
                        var rank = _playerPrefab.transform.GetChild(1).GetComponent<Text>();
                        var icon = _playerPrefab.transform.GetChild(2).GetComponent<Image>();
                        var name = _playerPrefab.transform.GetChild(3).GetComponent<Text>();
                        var score = _playerPrefab.transform.GetChild(4).GetComponent<Text>();

                        rank.text = $"{i}";
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
            else
            {
                _board.SetActive(false);
                _emptyBoard.SetActive(true);
            }
        }

    }
}