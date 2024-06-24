using Beamable.Server.Clients;
using Beamable;
using UnityEngine;
using TMPro;

public class CountdownPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _buttonText;

    [SerializeField]
    private GameObject _panel;

    async void Start()
    {
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;
        await beamContext.Accounts.OnReady;

        beamContext.Api.EventsService.Subscribe(async eventsGetResponse =>
        {
            if (eventsGetResponse.running.Count > 0)
            {
                _panel.SetActive(true);

                var account = beamContext.Accounts.Current;

                var paidTournamentId = await beamContext.Microservices().TournamentService().GetPaidTournamentById(account.GamerTag);
                if (TournamentManager.instance.eventId != "")
                {
                    var hasPaid = paidTournamentId == TournamentManager.instance.eventId;

                    if (hasPaid)
                    {
                        _buttonText.text = "Play";
                    }
                    else
                    {
                        _buttonText.text = "Join";
                    }
                }
            }
            else
            {
                _panel.SetActive(false);
            }
        });
    }
}
