namespace HydraDotNet.Core.Api;

public class HydraProdEndpoint : HydraEndpoint
{
    public override string BaseUrl => "https://prod-network-api.wbagora.com";
    public override string ApiKey => "a9019bc4eed048e7bfcb3172756c291e";

    public HydraProdEndpoint(string path) : base(path)
    {
    }
}
