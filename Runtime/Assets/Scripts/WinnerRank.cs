using UnityEngine;

public class WinnerRank : MonoBehaviour
{
    [SerializeField]
    private GameObject _panel;
    [SerializeField]
    private GameObject _firstPlace;
    [SerializeField]
    private GameObject _secondPlace;
    [SerializeField]
    private GameObject _thirdPlace;


    void Update()
    {
        var rank = TournamentManager.instance.rank;

        if (rank > 0 && rank < 4)
        {
            _panel.SetActive(true);

            switch (rank)
            {
                case 1:
                    _firstPlace.SetActive(true);
                    break;
                case 2:
                    _secondPlace.SetActive(true);
                    break;
                case 3:
                    _thirdPlace.SetActive(true);
                    break;
            }
        }
        else
        {
            _panel.SetActive(false);
            _firstPlace.SetActive(false);
            _secondPlace.SetActive(false);
            _thirdPlace.SetActive(false);
        }
    }
}
