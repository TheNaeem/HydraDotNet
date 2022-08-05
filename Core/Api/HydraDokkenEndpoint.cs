namespace HydraDotNet.Core.Api;

public class HydraDokkenEndpoint : HydraEndpoint
{
    public override string BaseUrl => "https://dokken-api.wbagora.com";
    public override string ApiKey => "51586fdcbd214feb84b0e475b130fce0";

    public HydraDokkenEndpoint(string path) : base(path)
    {
    }
}
