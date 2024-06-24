using Assets.Senet.Scripts.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Beamable.Examples.Services.LeaderboardService
{
    public class LeaderBoardManager : MonoBehaviour
    {
        [SerializeField]
        private List<TextMeshProUGUI> _scores;
        [SerializeField]
        private List<TextMeshProUGUI> _names;
        [SerializeField]
        private TextMeshProUGUI _currentPlayerScore;
        [SerializeField]
        private TextMeshProUGUI _currentPlayerRanking;
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

                            if (userId == account.GamerTag)
                            {
                                _currentPlayerRanking.text = $"{rankEntry.rank}.";
                                _currentPlayerScore.text = $"{rankEntry.score}";
                            }

                            stats.TryGetValue("alias", out string alias);
                            if (string.IsNullOrEmpty(alias))
                            {
                                alias = $"Player {index + 1}";
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