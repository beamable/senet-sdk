using Beamable;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using System.Threading.Tasks;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;
    public double senet = 0;
    private BeamContext _beamContext;
    private const long gweiPerSenet = 1000000000;
    private string senetToken = "currency.Senet";

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        _beamContext = BeamContext.Default;
        await _beamContext.OnReady;
        
        await FetchBalance();
        SubscribeToInventory();
    }

    private void SubscribeToInventory()
    {
        _beamContext.Api.InventoryService.Subscribe("currency", inventoryView =>
        {
            if (inventoryView.currencies.ContainsKey(senetToken))
            {
                var senetAmount = inventoryView.currencies[senetToken];
                senet = senetAmount;
            }
        });
    }

    public async Promise AddOrRemoveSenet(long amount = default)
    {
        var inventoryUpdateBuilder = new InventoryUpdateBuilder();
        inventoryUpdateBuilder.CurrencyChange(senetToken, amount);

        await _beamContext.Inventory.Update(inventoryUpdateBuilder);

        await FetchBalance();
    }
    
    private async Task FetchBalance()
    {
        var inventoryView = await _beamContext.Api.InventoryService.GetCurrent();
        if (inventoryView.currencies.ContainsKey(senetToken))
        {
            senet = inventoryView.currencies[senetToken];
        }
    }
}
