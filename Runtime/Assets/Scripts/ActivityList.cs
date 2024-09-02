using Beamable;
using Beamable.Server.Clients;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ActivityList : MonoBehaviour
{
    [SerializeField]
    private VerticalLayoutGroup verticalLayout;
    [SerializeField]
    private GameObject activityPrefab;
    private TournamentServiceClient _tournamentServiceClient;

    private async void Start()
    {
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;
        await beamContext.Accounts.OnReady;
        _tournamentServiceClient = new TournamentServiceClient();

        var account = beamContext.Accounts.Current;
        Debug.Log($"Gamertag {account.GamerTag}");
        var paidTournamentList = await _tournamentServiceClient.GetUserActivities();
        Debug.Log(paidTournamentList.Count);

        foreach (var item in paidTournamentList)
        {
            if (item.rank > 0)
            {
                var tournamentName = activityPrefab.transform.GetChild(2).GetComponent<Text>();
                tournamentName.text = item.tournamentName;

                activityPrefab.transform.GetChild(3).GetComponent<Text>().text = $"You paid {item.paymentAmount}";

                var rank = item.rank;
                var hasWon = rank == 1;

                var youWonContainer = activityPrefab.transform.GetChild(4);
                var betterLuckContainer = activityPrefab.transform.GetChild(5);

                if (hasWon)
                {
                    youWonContainer.gameObject.SetActive(true);
                    var winAmount = youWonContainer.transform.GetChild(1);

                    var winAmountText = $"{item.wonAmount}";
                    winAmount.transform.GetComponent<Text>().text = winAmountText;
                }
                else betterLuckContainer.gameObject.SetActive(true);

                Debug.Log($"{item.tournamentName} {item.rank} {item.wonAmount} {item.paymentAmount}");

                Instantiate(activityPrefab, verticalLayout.transform);
            }
        }
    }
}
