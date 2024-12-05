using Beamable;
using Beamable.Common;
using Beamable.Common.Api.Events;
using Beamable.Server.Clients;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class DoneTournament
{
    public long rank = 0;
    public long rewardAmount;
}

public class RunningTournament
{
    public long rank = 0;
    public string eventId;
    public bool hasPaid;
    public long playerCount;
}

public class TournamentManager : MonoBehaviour
{
    public static TournamentManager instance;
    public bool isTournament;
    private BeamContext _beamContext;

    private DateTime daysLeft;
    public TimeSpan countdownTime;

    public DoneTournament doneTournament;
    public RunningTournament runningTournament;
    public event Action<DoneTournament> OnDoneTournamentChanged;
    public event Action<RunningTournament> OnRunningTournamentChanged;
    public bool isPlacementBoardOpen = false;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {

        _beamContext = BeamContext.Default;
        await _beamContext.OnReady;

        _beamContext.Api.EventsService.Subscribe(eventsGetResponse =>
        {
            var done = eventsGetResponse.GetSenetGameTournament(EventStatusType.Done);
            var running = eventsGetResponse.GetSenetGameTournament(EventStatusType.Running);

            if (done.Count > 0)
            {
                var lastDoneEvent = done[^1];
                var lastEventEarned = lastDoneEvent.rankRewards.Find(i => i.earned);

                if (lastEventEarned != null && !lastEventEarned.claimed)
                {
                    var doneTournamentData = new DoneTournament
                    {
                        rank = lastDoneEvent.rank,
                        rewardAmount = lastDoneEvent.rankRewards.Find(i => i.earned).currencies[0].amount
                    };

                    OnDoneTournamentChanged?.Invoke(doneTournamentData);
                    doneTournament = doneTournamentData;
                }
            }
            else if (running.Count > 0)
            {
                var eventView = running[0];
                SetRunningTournament(eventView);
            }
            else
            {
                ResetTournamentData();
            }
        });
    }

    void OnApplicationPause(bool isGamePause)
    {
        if (!isGamePause)
        {
            UpdateCountdown();
        }
    }

    private void Update()
    {
        Countdown();
    }

    private void UpdateCountdown()
    {
        countdownTime = daysLeft - DateTime.Now;
    }

    private void Countdown()
    {
        countdownTime -= TimeSpan.FromSeconds(Time.deltaTime);

        countdownTime = (countdownTime < TimeSpan.Zero) ? TimeSpan.Zero : countdownTime;
    }

    public async Promise ClaimRewardsInTournament()
    {
        var eventsGetResponse = await _beamContext.Api.EventsService.GetCurrent();

        var lastEvent = eventsGetResponse.done[^1];
        var userReward = lastEvent.rankRewards.Find(i => i.earned);

        if (userReward != null && !userReward.claimed)
        {
            try
            {
                var rewardAmount = userReward.currencies[0].amount;
                await _beamContext.Microservices().TournamentService().ClaimTournamentRewards(lastEvent.id, lastEvent.name, rewardAmount, doneTournament.rank);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                var newEventsGetResponse = await _beamContext.Api.EventsService.GetCurrent();

                var running = newEventsGetResponse.GetSenetGameTournament(EventStatusType.Running);
                if (running.Count > 0)
                {
                    Debug.Log($"After claim: {running[0].id}");
                    SetRunningTournament(running[0]);
                }
                else
                {
                    ResetTournamentData();
                }
                SceneManager.LoadSceneAsync("SenetMainMenu");
            }
        }
    }

    public async Promise SetScoreInEvents(int totalScore)
    {
        var eventId = runningTournament.eventId;
        await _beamContext.Microservices().TournamentService().SetScore(eventId, totalScore);
        _beamContext.Api.EventsService.Subscribable.ForceRefresh();
        isTournament = false;

        if (totalScore > 0) isPlacementBoardOpen = true;
    }

    private async void SetRunningTournament(EventView eventView)
    {
        var _tournamentServiceClient = new TournamentServiceClient();
        var hasPaid = await _tournamentServiceClient.HasUserPaidForTournament(eventView.id);

        var runningTournamentData = new RunningTournament
        {
            eventId = eventView.id,
            hasPaid = hasPaid
        };

        var leaderboardId = eventView.leaderboardId;

        if (leaderboardId != "")
        {
            var view = await _beamContext.Api.LeaderboardService.GetBoard(leaderboardId, 1, 500);

            var rankings = view.rankings;
            runningTournamentData.playerCount = rankings.Count;

            await _beamContext.Accounts.OnReady;
            var account = _beamContext.Accounts.Current;

            var myRank = rankings.Where(i => i.gt == account.GamerTag).FirstOrDefault();

            runningTournamentData.rank = myRank.rank;
        }

        OnRunningTournamentChanged?.Invoke(runningTournamentData);
        runningTournament = runningTournamentData;

        var localTimeZone = TimeZoneInfo.Local;
        var localTime = TimeZoneInfo.ConvertTimeFromUtc(eventView.GetEndDate(), localTimeZone);

#if UNITY_ANDROID
        var notification = new AndroidNotification
        {
            Title = "Tournament End",
            Text = "Daily tournament just ended!",
            FireTime = localTime,
            SmallIcon = "icon_0",
            LargeIcon = "icon_1",
        };

        AndroidNotificationCenter.SendNotification(notification, "tournament_channel");
#endif
        daysLeft = localTime;
        UpdateCountdown();
    }

    private void ResetTournamentData()
    {
        doneTournament = null;
        runningTournament = null;

        OnDoneTournamentChanged?.Invoke(null);
        OnRunningTournamentChanged?.Invoke(null);
    }
}
