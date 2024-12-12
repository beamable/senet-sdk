using System.Threading.Tasks;
using TMPro;
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
    [SerializeField]
    private TMP_InputField _pasteTargetInputField;

    private string _walletAddress = "adgfjgdufg7646qbdaaaajvwt7358fjczxbMB";

    private void Start()
    {
        if (_walletAddressText == null)
        {
            Debug.LogError("Wallet Address Text is not assigned in the Inspector.");
            return;
        }
    
        _walletAddressText.text = _walletAddress;    }

    public async void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = _walletAddress;
        _copyButton.SetActive(false);
        _copied.SetActive(true);

        await Task.Delay(3000);

        _copyButton.SetActive(true);
        _copied.SetActive(false);
    }
    
    public void PasteFromClipboard()
    {
        Debug.Log("PasteFromClipboard called.");

        var clipboardText = GUIUtility.systemCopyBuffer;
        Debug.Log($"Clipboard content: '{clipboardText}'");

        if (_pasteTargetInputField != null)
        {
            _pasteTargetInputField.text = clipboardText; // Update the InputField's text
            Debug.Log($"Updated InputField text with: '{clipboardText}'");
        }
        else
        {
            Debug.LogWarning("Paste target InputField is not assigned. Unable to update UI.");
        }
    }

}
