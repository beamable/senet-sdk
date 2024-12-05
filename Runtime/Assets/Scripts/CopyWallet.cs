using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CopyWallet : MonoBehaviour
{
    [SerializeField]
    private Text _walletAddressText;

    [SerializeField]
    private GameObject _copyButton;
    [SerializeField]
    private GameObject _copied;

    private string _walletAddress = "adgfjgdufg7646qbdaaaajvwt7358fjczxbMB";

    private void Start()
    {
        _walletAddressText.text = _walletAddress;
    }

    public async void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = _walletAddress;
        _copyButton.SetActive(false);
        _copied.SetActive(true);

        await Task.Delay(3000);

        _copyButton.SetActive(true);
        _copied.SetActive(false);
    }
}
