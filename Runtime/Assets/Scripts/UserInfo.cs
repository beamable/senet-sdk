using Beamable;
using UnityEngine;
using UnityEngine.UI;

public class UserInfo : MonoBehaviour
{
    [SerializeField]
    private Text userEmail;
    [SerializeField]
    private Text username;

    async void Start()
    {
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;
        await beamContext.Accounts.OnReady;

        var account = beamContext.Accounts.Current;
        var stats = await beamContext.Api.StatsService.GetStats("client", "public", "player", account.GamerTag);
        stats.TryGetValue("alias", out string alias);

        userEmail.text = account.Email;
        username.text = alias;
    }
}
