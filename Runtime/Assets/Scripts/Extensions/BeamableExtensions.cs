using Assets.Senet.Scripts.Extensions;
using Beamable;
using Beamable.Api;
using Beamable.Api.Leaderboard;
using Beamable.Common.Api.Events;
using Beamable.Common.Api.Leaderboards;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public enum EventStatusType
{
    Running,
    Done
}

public static class BeamableExtensions
{
    public static List<EventView> GetSenetGameTournament(this EventsGetResponse eventsGetResponse, EventStatusType eventStatusType)
    {
        var settings = Resources.Load<SenetSettings>("SenetSettings");
        var senetGameName = settings.senetGameName;

        var events = eventStatusType == EventStatusType.Running ? eventsGetResponse.running : eventsGetResponse.done;
        var currentGameEvents = events.Select(i => i).Where(i =>
        {
            var currentGameName = i.id.Split('.')[1];
            return currentGameName == senetGameName;
        }).ToList();

        return currentGameEvents;
    }

    public static async Task<LeaderBoardView> GetLeaderboard(this LeaderboardService leaderboardService, string eventId)
    {
        var view = new LeaderBoardView();
        var leaderboardId = $"event_{eventId}.1";

        try
        {
            view = await leaderboardService.GetBoard(leaderboardId, 1, 500);
        }
        catch (PlatformRequesterException e)
        {
            if (e.Message.Contains(leaderboardId))
            {
                view = await leaderboardService.GetBoard($"event_{eventId}", 1, 500);
            }
        }

        return view;
    }

    public static async Task<List<PlayerModel>> GetTournamentPlayers(this EventsGetResponse eventsGetResponse)
    {
        var running = eventsGetResponse.GetSenetGameTournament(EventStatusType.Running);
        var players = new List<PlayerModel>();

        var beamContext = BeamContext.Default;
        await beamContext.OnReady;

        if (running.Count > 0)
        {
            var eventView = running[0];

            var view = await beamContext.Api.LeaderboardService.GetLeaderboard(eventView.id);

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

                var player = new PlayerModel
                {
                    id = userId,
                    name = alias,
                    rank = rankEntry.rank,
                    score = rankEntry.score,
                };

                players.Add(player);
            }
        }

        return players;
    }
}
