using Beamable;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public double senet = 0;
    private BeamContext _beamContext;
    private const long gweiPerSenet = 1000000000;
    private string senetToken = "currency.senet_currency.senet_token";

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
            if (inventoryView.currencies.ContainsKey(senetToken))
            {
                var senetAmount = inventoryView.currencies[senetToken];
                senet = senetAmount / gweiPerSenet;
            }
        });
    }

    public async Promise AddOrRemoveSenet(long amount = default)
    {
        var inventoryUpdateBuilder = new InventoryUpdateBuilder();
        inventoryUpdateBuilder.CurrencyChange(senetToken, amount * gweiPerSenet);

        await _beamContext.Inventory.Update(inventoryUpdateBuilder);
        return;
    }
}
