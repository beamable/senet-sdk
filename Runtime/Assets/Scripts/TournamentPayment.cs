using Beamable;
using Beamable.Server.Clients;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TournamentPayment : MonoBehaviour
{
    private BeamContext _beamContext;
    private TournamentServiceClient _tournamentServiceClient;

    private async void Start()
    {
        _beamContext = BeamContext.Default;
        await _beamContext.OnReady;
        await _beamContext.Accounts.OnReady;
        _tournamentServiceClient = new TournamentServiceClient();
    }

    public async void PaySenet()
    {
        await CurrencyManager.Instance.AddOrRemoveSenet(-10);
        var account = _beamContext.Accounts.Current;

        await _tournamentServiceClient.UpdatePaidTournamentRecord(account.GamerTag, TournamentManager.instance.eventId);

        SceneManager.LoadScene("SenetPaymentSuccess");
    }
}