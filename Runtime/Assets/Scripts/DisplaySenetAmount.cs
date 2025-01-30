using UnityEngine;
using UnityEngine.UI;

public class DisplaySenetAmount : MonoBehaviour
{
    [SerializeField]
    private Text _senetAmount;
    [SerializeField]
    private bool isRemainingAfterPayment = false;

    private double _remainingSenetAmount;

    private void Start()
    {
        if (CurrencyManager.instance)
        {
            _remainingSenetAmount = CurrencyManager.instance.senet - 25;
        }
    }

    private void Update()
    {
        if (CurrencyManager.instance)
        {
            _senetAmount.text = isRemainingAfterPayment ? $"{_remainingSenetAmount}" : $"{CurrencyManager.instance.senet}";
        }
    }
}