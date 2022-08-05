using HydraDotNet.Core.Models;
using RestSharp;

namespace HydraDotNet.Core.Authentication;

public static class EpicAuthUtil
{
    private static RestClient ExternalTokenClient = new("https://api.epicgames.dev/epic/oauth/v1/token");
    private static RestClient AccessTokenClient = new("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/token");
    private static RestClient ExchangeCodeClient = new("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/exchange");

    public static EpicExternalAuthResponse? GetExternalAuth(string exchangeCode, string authClient, string scope)
    {
        var request = new RestRequest()
        {
            Method = Method.Post
        };

        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddHeader("Authorization", "Basic " + authClient);
        request.AddParameter("application/x-www-form-urlencoded", $"deployment_id=29da23b21f7f41319c7fa5e86e20dc2b&grant_type=exchange_code&scope={scope.Replace(" ", "%20")}&exchange_code={exchangeCode}", ParameterType.RequestBody);

        return ExternalTokenClient.Execute<EpicExternalAuthResponse>(request).Data;
    }

    public static string? GetExchangeCode(string accessToken)
    {
        var request = new RestRequest()
        {
            Method = Method.Get
        };

        request.AddHeader("Authorization", "Bearer " + accessToken);

        var response = ExchangeCodeClient.Execute<EpicExchangeCodeResponse>(request);

        if (!response.IsSuccessful ||
            response.Data is null ||
            string.IsNullOrEmpty(response.Data.code))
        {
            return null;
        }

        return response.Data.code;
    }

    public static EpicAccessTokenResponse? GetAccessFromRefresh(string refreshToken, string authClient, string? scope = null)
    {
        var request = new RestRequest()
        {
            Method = Method.Post
        };

        var form = "grant_type=refresh_token&refresh_token=" + refreshToken;

        if (!string.IsNullOrEmpty(scope))
            form += $"&scope={scope}";

        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddHeader("Authorization", "basic " + authClient);
        request.AddParameter("application/x-www-form-urlencoded", form, ParameterType.RequestBody);

        return AccessTokenClient.Execute<EpicAccessTokenResponse>(request).Data;
    }
}
