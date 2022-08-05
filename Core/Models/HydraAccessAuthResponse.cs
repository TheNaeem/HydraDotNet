namespace HydraDotNet.Core.Models;

public class HydraAccessAuthResponse
{
    public string? token { get; set; }
    public WBNetworkToken? wb_network { get; set; }
}

public class WBNetworkToken
{
    public string? network_token { get; set; }
}
