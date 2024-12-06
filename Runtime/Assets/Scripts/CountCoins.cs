using DG.Tweening;
using UnityEngine;

public class CountCoins : MonoBehaviour
{
    private GameObject coins;
    [SerializeField] private GameObject counter;

    void Start()
    {
        coins = transform.GetChild(0).gameObject;
    }

    public void AddCoins()
    {
        coins.SetActive(true);


        for (int i = 0; i < coins.transform.childCount; i++)
        {
            coins.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
        }

        var delay = 0f;

        for (int i = 0; i < coins.transform.childCount; i++)
        {
            coins.transform.GetChild(i).DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);

            coins.transform.GetChild(i).GetComponent<RectTransform>().DOAnchorPos(new Vector2(400f, 900f), 0.8f)
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
            coins.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(400f, 900f);
        }

        var delay = 0f;

        for (int i = 0; i < coins.transform.childCount; i++)
        {
            coins.transform.GetChild(i).DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);

            coins.transform.GetChild(i).GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f), 0.8f)
                    .SetDelay(delay + 0.5f).SetEase(Ease.InBack);

            coins.transform.GetChild(i).DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f)
                .SetEase(Ease.Flash);

            coins.transform.GetChild(i).DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack);

            delay += 0.1f;
        }

        DOVirtual.DelayedCall(delay + 2f, () => coins.SetActive(false));
    }
}

