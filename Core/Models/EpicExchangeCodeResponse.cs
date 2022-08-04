namespace HydraDotNet.Core.Models;

public class EpicExchangeCodeResponse
{
    public int expiresInSeconds { get; set; }
    public string? code { get; set; }
    public string? creatingClientId { get; set; }
}

