using System;
using System.Diagnostics;

namespace HydraDotNet.Core.Authentication;

public class EpicAuthContainer
{
    protected Action<string>? _onRefreshTokenUpdated;
    protected string? _scope;
    protected string? _accessToken;
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

    protected string RefreshToken { get; set; }
    private Stopwatch Sw { get; set; }
    private TimeSpan Expiration { get; set; }
    private string AuthClient { get; set; }

    protected EpicAuthContainer(string authClient)
    {
        AuthClient = authClient;
        RefreshToken = string.Empty;
        _accessToken = string.Empty;
        Sw = new();
    }

    /// <summary>
    /// Creates an auth which automatically updates the access token when necessary.
    /// </summary>
    /// <param name="refreshToken">Refresh token for the specified auth client.</param>
    /// <param name="authClient">Base64 encoded client id and secret.</param>
    /// <param name="onRefreshTokenUpdated">Optional: Action that will be executed with the updated refresh token passed. Recommended for caching the refresh token.</param>
    public EpicAuthContainer(string refreshToken, string authClient, Action<string>? onRefreshTokenUpdated = null)
    {
        RefreshToken = refreshToken;
        AuthClient = authClient;
        _onRefreshTokenUpdated = onRefreshTokenUpdated;
        Sw = new();

        UpdateToken();
    }

    protected void UpdateToken()
    {
        var refreshResponse = Epic.GetAccessFromRefresh(RefreshToken, AuthClient, _scope);

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

        if (_onRefreshTokenUpdated is not null)
            _onRefreshTokenUpdated(RefreshToken);
    }
}
