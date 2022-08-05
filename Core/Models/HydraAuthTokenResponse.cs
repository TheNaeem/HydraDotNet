namespace HydraDotNet.Core.Models;

public class HydraAuthTokenResponse
{
    public string? access_token { get; set; }
    public int expires_in { get; set; }
    public bool mfa_required { get; set; }
}

