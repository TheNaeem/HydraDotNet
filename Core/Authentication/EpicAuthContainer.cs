using System;
using System.Diagnostics;

namespace HydraDotNet.Core.Authentication;

public class EpicAuthContainer
{
    private string? _accessToken;
    public string AccessToken
    {
        get
        {
            if (Sw.Elapsed >= Expiration)
            {
                UpdateToken();
            }

            return _accessToken ?? string.Empty;
        }
    }

    private string RefreshToken { get; set; }
    private Stopwatch Sw { get; set; }
    private TimeSpan Expiration { get; set; }
    private string AuthClient { get; set; }

    /// <summary>
    /// Creates an auth which automatically updates the access token when necessary.
    /// </summary>
    /// <param name="refreshToken">Refresh token for the specified auth client.</param>
    /// <param name="authClient">Base64 encoded client id and secret.</param>
    public EpicAuthContainer(string refreshToken, string authClient)
    {
        RefreshToken = refreshToken;
        AuthClient = authClient;
        Sw = new();

        UpdateToken();
    }

    private void UpdateToken()
    {
        var refreshResponse = Epic.GetAccessFromRefresh(RefreshToken, AuthClient);

        if (refreshResponse is null ||
            string.IsNullOrEmpty(refreshResponse.access_token) ||
            string.IsNullOrEmpty(refreshResponse.refresh_token) ||
            !refreshResponse.expires_in.HasValue)
        {
            return;
        }

        Sw = Stopwatch.StartNew();
        Expiration = TimeSpan.FromSeconds(refreshResponse.expires_in.Value);
        RefreshToken = refreshResponse.refresh_token;
        _accessToken = refreshResponse.access_token;
    }
}
