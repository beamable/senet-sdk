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
        Debug.Log($"Total Tournaments: {paidTournamentList.Count}");

        foreach (var item in paidTournamentList)
        {
            if (item.rank > 0)
            {
                GameObject newActivity = Instantiate(activityPrefab, verticalLayout.transform);

                var tournamentName = newActivity.transform.GetChild(2).GetComponent<Text>();
                tournamentName.text = item.tournamentName;

                newActivity.transform.GetChild(3).GetComponent<Text>().text = $"You paid {item.paymentAmount}";

                var rank = item.rank;
                var hasWon = rank == 1;

                var youWonContainer = newActivity.transform.GetChild(4).gameObject;
                var betterLuckContainer = newActivity.transform.GetChild(5).gameObject;

                youWonContainer.SetActive(false);
                betterLuckContainer.SetActive(false);

                if (hasWon)
                {
                    youWonContainer.SetActive(true);
                    var winAmount = youWonContainer.transform.GetChild(1).GetComponent<Text>();
                    winAmount.text = $"{item.wonAmount}";
                }
                else
                {
                    betterLuckContainer.SetActive(true);
                }

                Debug.Log($"Tournament: {item.tournamentName}, Rank: {item.rank}, Won: {item.wonAmount}, Paid: {item.paymentAmount}");
            }
        }
    }
}
