using Beamable;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CopyWallet : MonoBehaviour
{
    [SerializeField]
    private GameObject _copyButton;
    [SerializeField]
    private GameObject _copied;
    [SerializeField]
    private TMP_InputField _pasteTargetInputField;
    [SerializeField]
    private TMP_Text _walletAddressInput;

    private string _walletAddress = "";

    private async void Start()
    {
        if (_walletAddressInput != null)
        {
            var beamContext = BeamContext.Default;
            await beamContext.OnReady;
            await beamContext.Accounts.OnReady;

            var account = beamContext.Accounts.Current;

            var address = account.ExternalIdentities.Where(i => i.providerService == "Web3Federation").Select(i => i.userId).FirstOrDefault();
            _walletAddress = address;
            _walletAddressInput.text = address;
        }
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
