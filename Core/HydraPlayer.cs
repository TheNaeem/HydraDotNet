using HydraDotNet.Core.Api;
using HydraDotNet.Core.Authentication;
using System.Threading.Tasks;

namespace HydraDotNet.Core;

/// <summary>
/// A wrapper for the Hydra client providing player data for *CENSORED*
/// </summary>
public class HydraPlayer : HydraClient
{
    /// <summary>
    /// Hydra player constructor.
    /// </summary>
    /// <param name="epicAuth">Container with external Epic games authentication information.</param>
    /// <param name="config">Optional: Configuration for Hydra client.</param>
    public HydraPlayer(ExternalEpicAuthContainer epicAuth, HydraClientConfiguration? config = null) 
        : base(epicAuth, config)
    {
    }

    /// <summary>
    /// Asynchronously retrieves equipped items.
    /// </summary>
    /// <returns></returns>
    public async Task<HydraApiResponse> GetEquippedItemsAsync()
    {
        var request = Endpoints.Preferences.CreateRequest(urlExtension: $"{AccountId}/{AccountId}");
        return await DoRequestAsync(request);
    }

    /// <summary>
    /// Retrieves equipped items.
    /// </summary>
    /// <returns></returns>
    public HydraApiResponse GetEquippedItems()
    {
        var request = Endpoints.Preferences.CreateRequest(urlExtension: $"{AccountId}/{AccountId}");
        return DoRequest(request);
    }
}
