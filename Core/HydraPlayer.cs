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
    /// <param name="apiKey">Optional: Overrides the api key.</param>
    public HydraPlayer(ExternalEpicAuthContainer epicAuth, HydraClientConfiguration? config = null) 
        : base(epicAuth, config)
    {
    }
}
