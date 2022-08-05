namespace HydraDotNet.Core.Models;

public class HydraAuthTokenRequestBody
{
    public string? code { get; set; }
    public string? grant_type { get; set; }
}

