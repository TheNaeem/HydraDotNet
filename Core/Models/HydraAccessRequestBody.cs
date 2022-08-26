namespace HydraDotNet.Core.Models;

public class HydraEpicAccessRequestBody
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

    public class HydraAccessRequestAuth
    {
        public string? epic { get; set; }
        public bool fail_on_missing { get; set; } = false;
    }
}

public class HydraSteamAccessRequestBody
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

    public class HydraAccessRequestAuth
    {
        public string? steam { get; set; }
        public bool fail_on_missing { get; set; } = false;
    }
}

