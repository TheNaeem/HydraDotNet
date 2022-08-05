using RestSharp;

namespace HydraDotNet.Core.Api;

public abstract class HydraEndpoint
{
    public string Path { get; init; }
    public abstract bool UseNetworkToken { get; }
    public abstract string? ApiKey { get; }
    public abstract string BaseUrl { get; }

    public HydraEndpoint(string path)
    {
        if (!path.StartsWith('/'))
            path = '/' + path;

        Path = path;
    }

    public HydraApiRequest CreateRequest(Method method = Method.Get, Parameter? parameter = null, string? urlExtension = null) => new(this, method, parameter, urlExtension);
    public HydraApiRequest CreateRequest(Method method, ParametersCollection parameters, string? urlExtension = null) => new(this, method, parameters, urlExtension);
    public HydraApiRequest CreateRequest(byte[] hydraEncodedBody, Method method = Method.Get, string? urlExtension = null) 
        => CreateRequest(method, new BodyParameter("application/x-ag-binary", hydraEncodedBody, "application/x-ag-binary", DataFormat.Binary), urlExtension);
}
