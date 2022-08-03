using RestSharp;
using System.Threading.Tasks;

namespace HydraDotNet.Core.Endpoints;

public class HydraEndpoint
{
    public RestRequest Request { get; init; }

    public HydraEndpoint(string path)
    {
        Request = new(path);
    }

    public RestResponse GetResponse(RestClient client)
    {
        return client.Execute(Request);
    }

    public async Task<RestResponse> GetResponseAsync(RestClient client)
    {
        return await client.ExecuteAsync(Request);
    }
}
