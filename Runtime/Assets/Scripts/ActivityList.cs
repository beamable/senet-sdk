using Beamable;
using Beamable.Server.Clients;
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
        var paidTournamentList = await _tournamentServiceClient.GetUserActivities();

        foreach (var item in paidTournamentList)
        {
            if (item.rank > 0)
            {
                var tournamentName = activityPrefab.transform.GetChild(3).GetComponent<Text>();
                tournamentName.text = item.tournamentName;

                activityPrefab.transform.GetChild(4).GetComponent<Text>().text = $"You paid {item.paymentAmount}";

                var rank = item.rank;
                var hasWon = rank == 1;

                var youWonContainer = activityPrefab.transform.GetChild(5).gameObject;
                var betterLuckContainer = activityPrefab.transform.GetChild(6).gameObject;

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

                Instantiate(activityPrefab, verticalLayout.transform);
            }
        }
    }
}
