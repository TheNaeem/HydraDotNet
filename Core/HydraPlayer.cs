using HydraDotNet.Core.Api;
using HydraDotNet.Core.Authentication;
using HydraDotNet.Core.Encoding;
using HydraDotNet.Core.Models;
using RestSharp;
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
    /// Retrieves incoming invites.
    /// </summary>
    public PlayerInvites? GetIncomingInvites()
    {
        var request = Endpoints.IncomingInvites.CreateRequest();
        return DoRequest(request).GetContent<PlayerInvites>();
    }

    /// <summary>
    /// Asynchronously retrieves incoming invites.
    /// </summary>
    public async Task<PlayerInvites?> GetIncomingInvitesAsync()
    {
        var request = Endpoints.IncomingInvites.CreateRequest();
        return (await DoRequestAsync(request)).GetContent<PlayerInvites>();
    }

    /// <summary>
    /// Asynchronously retrieves equipped items.
    /// </summary>
    public async Task<HydraApiResponse> GetEquippedItemsAsync()
    {
        var request = Endpoints.Preferences.CreateRequest(urlExtension: $"{AccountId}/{AccountId}");
        return await DoRequestAsync(request);
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <returns></returns>
    public async Task<HydraApiResponse> GetBatchAsync()
    {
        var body = new HydraBatchRequestBody();
        body.requests = new(1);
        body.requests.Add(new()
        {
            headers = new()
            {
                url = $"/profiles/{AccountId}?fields=server_data&fields=data",
                verb = "GET"
            }
        });

        await using var encoder = new HydraEncoder();
        encoder.WriteValue(body);

        var request = Endpoints.Batch.CreateRequest(await encoder.GetBufferAsync(), Method.Put);

        return await DoRequestAsync(request);
    }

    public async Task<ItemSlugsArray?> GetSlugsAsync()
    {
        var req = Endpoints.GetItemSlugs.CreateRequest();
        return (await DoRequestAsync(req)).GetContent<ItemSlugsArray>();
    }

    /// <summary>
    /// Retrieves equipped items.
    /// </summary>
    public HydraApiResponse GetEquippedItems()
    {
        var request = Endpoints.Preferences.CreateRequest(urlExtension: $"{AccountId}/{AccountId}");
        return DoRequest(request);
    }
}
