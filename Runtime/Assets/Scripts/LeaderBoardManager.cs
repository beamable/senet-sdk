using Assets.Senet.Scripts.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.LeaderboardService
{
    public class LeaderBoardManager : MonoBehaviour
    {
        [SerializeField]
        private List<Text> _scores;
        [SerializeField]
        private List<Text> _names;
        [SerializeField]
        private Text _currentPlayerScore;
        [SerializeField]
        private Text _currentPlayerRanking;
        [SerializeField]

        protected async void Start()
        {
            var beamContext = BeamContext.Default;
            await beamContext.OnReady;
            beamContext.Api.EventsService.Subscribe(async eventsGetResponse =>
            {
                if (eventsGetResponse.running.Count > 0)
                {
                    var eventView = eventsGetResponse.running[0];
                    var leaderboardId = eventView.leaderboardId;

                    if (leaderboardId != "")
                    {
                        var view = await beamContext.Api.LeaderboardService.GetBoard(leaderboardId, 1, 6);
                        await beamContext.Accounts.OnReady;
                        var account = beamContext.Accounts.Current;
                        var rankings = view.rankings;

                        foreach (var (rankEntry, index) in rankings.WithIndex())
                        {
                            long userId = rankEntry.gt;
                            var stats =
                                    await beamContext.Api.StatsService.GetStats("client", "public", "player", userId);

                            stats.TryGetValue("alias", out string alias);
                            if (string.IsNullOrEmpty(alias))
                            {
                                alias = $"Player {index + 1}";
                            }

                            if (userId == account.GamerTag)
                            {
                                _currentPlayerRanking.text = alias;
                                _currentPlayerScore.text = $"{rankEntry.score}";
                            }

                            _scores[index].text = $"{rankEntry.score}";
                            _names[index].text = alias;
                        }
                    }
                }
            });
        }
    }
}