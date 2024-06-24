using Beamable;
using Beamable.Server.Clients;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WithdrawToken : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _addressField;
    [SerializeField]
    private TMP_InputField _amountField;
    [SerializeField]
    private GameObject _notificationPanel;
    [SerializeField]
    private Button _withdrawalButton;
    [SerializeField]
    private TMP_Text _notificationText;
    private Web3FederationClient _web3FederationClient;


    private async void Start()
    {
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;
        _web3FederationClient = beamContext.Microservices().Web3Federation();
    }

    public async void WithdrawSenet()
    {
        var address = _addressField.text;
        var amountText = _amountField.text;

        if (address == "" && amountText == "")
        {
            DisplayNotificationPanel("Address and amount are empty", true);
            return;
        }
        else if (address == "")
        {
            DisplayNotificationPanel("Address is empty", true);
            return;
        }
        else if (amountText == "")
        {
            DisplayNotificationPanel("Amount is empty", true);
            return;
        }

        try
        {
            _withdrawalButton.interactable = false;
            long amount = Convert.ToInt64(_amountField.text);
            var guid = Guid.NewGuid();
            const long gweiPerSenet = 1000000000;
            long withdrawalAmount = (amount * gweiPerSenet);

            await _web3FederationClient.Withdrawal(guid.ToString(), address, "currency.senet_currency.senet_token", withdrawalAmount);
            DisplayNotificationPanel("Withdrawal Successfull");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            DisplayNotificationPanel(ex.ToString(), true);
        }
        finally
        {
            _withdrawalButton.interactable = true;
            _addressField.text = "";
            _amountField.text = "";
        }
    }

    private void DisplayNotificationPanel(string message, bool isError = false)
    {
        if (!isError)
        {
            _notificationPanel.GetComponent<Image>().color = Color.green;
        }
        else
        {
            _notificationPanel.GetComponent<Image>().color = Color.red;
        }
        _notificationText.text = message;
        _notificationPanel.SetActive(true);

        StartCoroutine(HidePanelAfterDelay());
    }

    private IEnumerator HidePanelAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        _notificationPanel.SetActive(false);
    }
}
