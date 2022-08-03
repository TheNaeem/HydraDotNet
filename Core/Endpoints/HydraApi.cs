namespace HydraDotNet.Core.Endpoints;

public static class HydraApi
{
    public static readonly HydraEndpoint GetCountryCode = new("/ssc/invoke/get_country_code");
}
