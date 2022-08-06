using HydraDotNet.Core.Api;
using HydraDotNet.Core.Authentication;

namespace HydraDotNet.Core;

/// <summary>
/// A wrapper for the Hydra client providing player data.
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

    public void Test()
    {
        var request = Endpoints.Preferences.CreateRequest(urlExtension: $"{AccountId}/{AccountId}");
        var response = DoRequest(request);

        System.Console.WriteLine(response.GetContentString());
    }
}
