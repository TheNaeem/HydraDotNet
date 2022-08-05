using RestSharp;

namespace HydraDotNet.Core.Api;

public abstract class HydraEndpoint
{
    public string Path { get; init; }
    public abstract string? ApiKey { get; }
    public abstract string BaseUrl { get; }

    public HydraEndpoint(string path)
    {
        if (!path.StartsWith('/'))
            path = '/' + path;

        Path = path;
    }

    public HydraApiRequest CreateRequest(Method method = Method.Get, Parameter? parameter = null) => new(this, method, parameter);
    public HydraApiRequest CreateRequest(Method method, ParametersCollection parameters) => new(this, method, parameters);
    public HydraApiRequest CreateRequest(byte[] hydraEncodedBody, Method method = Method.Get) 
        => CreateRequest(method, new BodyParameter("application/x-ag-binary", hydraEncodedBody, "application/x-ag-binary", DataFormat.Binary));
}
