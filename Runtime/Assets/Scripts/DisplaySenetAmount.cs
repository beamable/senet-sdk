using UnityEngine;
using UnityEngine.UI;

public class DisplaySenetAmount : MonoBehaviour
{
    [SerializeField]
    private Text _senetAmount;
    [SerializeField]
    private bool isRemainingAfterPayment = false;

    void Update()
    {
        if (CurrencyManager.instance)
        {
            if (isRemainingAfterPayment) _senetAmount.text = $"{CurrencyManager.instance.senet - 25}";
            else _senetAmount.text = $"{CurrencyManager.instance.senet}";
        }
    }
}
