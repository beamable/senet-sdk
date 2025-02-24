using Beamable;
using Beamable.Common;
using Beamable.Common.Api.Events;
using Beamable.Server.Clients;
using System;
using UnityEngine;
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
    public long firstPlaceReward; 
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

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        _beamContext.Api.EventsService.Subscribe(eventsGetResponse =>
        {
            var done = eventsGetResponse.GetSenetGameTournament(EventStatusType.Done);
            var running = eventsGetResponse.GetSenetGameTournament(EventStatusType.Running);
    
            if (running.Count > 0)
            {
                var eventView = running[0];
                SetRunningTournament(eventView);
            }
            else
            {
                ResetTournamentData();
            }

            if (done.Count <= 0) return;
            var lastDoneEvent = done[^1];
            var lastEventEarned = lastDoneEvent.rankRewards.Find(i => i.earned);
    
            if (lastEventEarned == null || lastEventEarned.claimed) return;
    
            var doneTournamentData = new DoneTournament
            {
                rank = lastDoneEvent.rank,
                rewardAmount = lastEventEarned.currencies[0].amount
            };
    
            OnDoneTournamentChanged?.Invoke(doneTournamentData);
            doneTournament = doneTournamentData;
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

                var newEventsGetResponse = await _beamContext.Api.EventsService.GetCurrent();

                var running = newEventsGetResponse.GetSenetGameTournament(EventStatusType.Running);
                if (running.Count > 0)
                {
                    SetRunningTournament(running[0]);
                }
                else
                {
                    ResetTournamentData();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
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
        var tournamentServiceClient = new TournamentServiceClient();
        var hasPaid = await tournamentServiceClient.HasUserPaidForTournament(eventView.id);

        var runningTournamentData = new RunningTournament
        {
            eventId = eventView.id,
            hasPaid = hasPaid
        };

        var view = await _beamContext.Api.LeaderboardService.GetLeaderboard(eventView.id);

        var firstPlaceReward = eventView.rankRewards.FirstOrDefault()?.currencies[0].amount ?? 0;
        runningTournamentData.firstPlaceReward = firstPlaceReward;
        
        var rankings = view.rankings;
        runningTournamentData.playerCount = rankings.Count;
        
        await _beamContext.Accounts.OnReady;
        var account = _beamContext.Accounts.Current;

        var myRank = rankings.Where(i => i.gt == account.GamerTag).FirstOrDefault();

        if (myRank != null)
        {
            runningTournamentData.rank = myRank.rank;
        }
        
        OnDoneTournamentChanged?.Invoke(null);
        doneTournament = null;

        runningTournament = runningTournamentData;
        OnRunningTournamentChanged?.Invoke(runningTournamentData);
        
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
    
    public long GetRunningFirstPlaceReward()
    {
        return runningTournament?.firstPlaceReward ?? 0;
    }
}
