using DG.Tweening;
using UnityEngine;

public class CountCoins : MonoBehaviour
{
    private GameObject coins;

    [SerializeField] private GameObject counter;

    [SerializeField] private Vector2 startPosition = new Vector2(400f, 900f); 
    [SerializeField] private Vector2 endPosition = new Vector2(0f, 0f);  
    void Start()
    {
        coins = transform.GetChild(0).gameObject;

        //default values if none are set in the inspector
        if (startPosition == Vector2.zero)
        {
            startPosition = new Vector2(400f, 900f); 
        }

        if (endPosition == Vector2.zero)
        {
            endPosition = new Vector2(0f, 0f); 
        }
    }

    public void AddCoins()
    {
        coins.SetActive(true);

        for (int i = 0; i < coins.transform.childCount; i++)
        {
            coins.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = endPosition;
        }

        var delay = 0f;

        for (int i = 0; i < coins.transform.childCount; i++)
        {
            coins.transform.GetChild(i).DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);

            coins.transform.GetChild(i).GetComponent<RectTransform>().DOAnchorPos(startPosition, 0.8f)
                    .SetDelay(delay + 0.5f).SetEase(Ease.InBack);

            coins.transform.GetChild(i).DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f)
                .SetEase(Ease.Flash);

            coins.transform.GetChild(i).DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack);

            delay += 0.1f;

            if (counter != null)
                counter.transform.DOScale(1.1f, 0.1f).SetLoops(6, LoopType.Yoyo).SetEase(Ease.InOutSine).SetDelay(1.2f);
        }

        DOVirtual.DelayedCall(delay + 2f, () => coins.SetActive(false));
    }

    public void RemoveCoins()
    {
        coins.SetActive(true);

        for (int i = 0; i < coins.transform.childCount; i++)
        {
            coins.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = startPosition;
        }

        var delay = 0f;

        for (int i = 0; i < coins.transform.childCount; i++)
        {
            coins.transform.GetChild(i).DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);

            coins.transform.GetChild(i).GetComponent<RectTransform>().DOAnchorPos(endPosition, 0.8f)
                    .SetDelay(delay + 0.5f).SetEase(Ease.InBack);

            coins.transform.GetChild(i).DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f)
                .SetEase(Ease.Flash);

            coins.transform.GetChild(i).DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack);

            delay += 0.1f;
        }

        DOVirtual.DelayedCall(delay + 2f, () => coins.SetActive(false));
    }
}
