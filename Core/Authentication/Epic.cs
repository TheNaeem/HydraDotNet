using HydraDotNet.Core.Objects;
using RestSharp;

namespace HydraDotNet.Core.Authentication;

public static class Epic
{
    private static RestClient AccessTokenClient = new("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/token");
    private static RestClient ExchangeCodeClient = new("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/exchange");

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

    public static EpicAccessTokenResponse? GetAccessFromRefresh(string refreshToken, string authClient)
    {
        var request = new RestRequest()
        {
            Method = Method.Post
        };

        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddHeader("Authorization", "basic " + authClient);
        request.AddParameter("application/x-www-form-urlencoded", "grant_type=refresh_token&refresh_token=" + refreshToken, ParameterType.RequestBody);

        return AccessTokenClient.Execute<EpicAccessTokenResponse>(request).Data;
    }
}
