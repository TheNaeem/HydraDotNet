namespace HydraDotNet.Core.Models;

public class HydraAccessRequestBody
{
    public HydraAccessRequestAuth auth { get; set; } = new();
    public string[] options { get; set; } = new[]
    {
        "configuration",
        "achievements",
        "account",
        "profile",
        "notifications",
        "maintenance",
        "wb_network"
    };
}

public class HydraAccessRequestAuth
{
    public string? epic { get; set; }
    public bool fail_on_missing { get; set; } = false;
}

