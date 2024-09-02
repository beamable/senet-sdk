using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySenetAmount : MonoBehaviour
{
    [SerializeField]
    private Text _senetAmount;

    void Update()
    {
        _senetAmount.text = $"{CurrencyManager.Instance.senet}";
    }
}
