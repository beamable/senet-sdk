using UnityEngine;
using UnityEngine.UI;

public class DisplaySenetAmount : MonoBehaviour
{
    [SerializeField]
    private Text _senetAmount;

    void Update()
    {
        if (CurrencyManager.instance)
        {
            _senetAmount.text = $"{CurrencyManager.instance.senet}";
        }
    }
}
