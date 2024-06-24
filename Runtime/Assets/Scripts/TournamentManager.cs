using Beamable;
using Beamable.Common;
using Beamable.Common.Api.Events;
using Beamable.Server.Clients;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class TournamentManager : MonoBehaviour
{
    public static TournamentManager instance;
    public bool IsTournament;
    public string eventId;
    private BeamContext _beamContext;

    private DateTime daysLeft;
    public TimeSpan countdownTime;

    public long rank;
    public long rewardAmount;

    private async void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);

        _beamContext = BeamContext.Default;
        await _beamContext.OnReady;

        _beamContext.Api.EventsService.Subscribe(eventsGetResponse =>
        {
            if (eventsGetResponse.done.Count > 0)
            {
                var lastDoneEvent = eventsGetResponse.done[^1];
                var lastEventEarned = lastDoneEvent.rankRewards.Find(i => i.earned);

                if (lastEventEarned != null && !lastEventEarned.claimed)
                {
                    rank = lastDoneEvent.rank;
                    rewardAmount = lastDoneEvent.rankRewards.Find(i => i.earned).currencies[0].amount;
                }
            }
            else if (eventsGetResponse.running.Count > 0)
            {
                var eventView = eventsGetResponse.running[0];
                SetRunningTournament(eventView);
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

        Debug.Log(lastEvent.id);

        var userReward = lastEvent.rankRewards.Find(i => i.earned);

        if (userReward != null && !userReward.claimed)
        {
            try
            {
                await _beamContext.Microservices().TournamentService().ClaimTournamentRewards(lastEvent.id);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                var newEventsGetResponse = await _beamContext.Api.EventsService.GetCurrent();

                if (newEventsGetResponse.running.Count > 0)
                {
                    Debug.Log($"After claim: {newEventsGetResponse.running[0].id}");
                    SetRunningTournament(newEventsGetResponse.running[0]);
                }
                else
                {
                    ResetTournamentData();
                }
                SceneManager.LoadSceneAsync("MainMenu");
            }
        }
    }

    public async Promise SetScoreInEvents(int totalScore)
    {
        await _beamContext.Microservices().TournamentService().SetScore(eventId, totalScore);
        _beamContext.Api.EventsService.Subscribable.ForceRefresh();
    }

    private void SetRunningTournament(EventView eventView)
    {
        eventId = eventView.id;

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
        eventId = "";
        rank = 0;
        rewardAmount = 0;
    }
}
