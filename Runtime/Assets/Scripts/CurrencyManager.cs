using Beamable;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using Beamable.Server.Clients;
using UnityEngine;
using Web3FederationCommon;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public double senet = 0;
    private BeamContext _beamContext;
    private const long gweiPerSenet = 1000000000;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        _beamContext = BeamContext.Default;
        await _beamContext.OnReady;
        await _beamContext.Accounts.OnReady;

        _beamContext.Api.InventoryService.Subscribe("currency", inventoryView =>
        {
            var senetAmount = inventoryView.currencies["currency.senet_currency.senet_token"];
            Debug.Log(senetAmount);
            senet = senetAmount / gweiPerSenet;
        });
    }

    public async Promise AddOrRemoveSenet(long amount = default)
    {
        var inventoryUpdateBuilder = new InventoryUpdateBuilder();
        inventoryUpdateBuilder.CurrencyChange("currency.senet_currency.senet_token", amount * gweiPerSenet);

        await _beamContext.Inventory.Update(inventoryUpdateBuilder);
        return;
    }
}
