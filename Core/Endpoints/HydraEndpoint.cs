using RestSharp;
using System.Threading.Tasks;

namespace HydraDotNet.Core.Endpoints;

public class HydraEndpoint
{
    public RestRequest Request { get; init; }

    public HydraEndpoint(string path, byte[] encodedBody, Method method = Method.Get)
    {
        Request = new(path, method);
        Request.RequestFormat = DataFormat.Binary;
        Request.AddParameter("application/x-ag-binary", encodedBody, ParameterType.RequestBody);
    }

    public HydraEndpoint(string path, Method method = Method.Get, Parameter? param = null)
    {
        Request = new(path, method);

        if (param is not null)
            Request.AddParameter(param);
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
