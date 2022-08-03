namespace HydraDotNet.Core;

public class HydraClientConfiguration
{
    /// <summary>
    /// API requests will attempt to get a JSON response instead of a binary response. False by default.
    /// </summary>
    public bool ForceJSONRequest { get; set; } = false;
}
