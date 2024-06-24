using TMPro;
using UnityEngine;

public class DisplaySenetAmount : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _senetAmount;

    void Update()
    {
        _senetAmount.text = $"{CurrencyManager.Instance.senet}";
    }
}
