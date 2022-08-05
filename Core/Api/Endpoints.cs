namespace HydraDotNet.Core.Api;

/// <summary>
/// Global Hydra endpoints and util to use.
/// </summary>
public static class Endpoints
{
    /// <summary>
    /// Gets a string of the country the client is in.
    /// </summary>
    public static readonly HydraDokkenEndpoint GetCountryCode = new("/ssc/invoke/get_country_code");

    /// <summary>
    /// Retrieves account and player information as well as a token to get the Hydra access token.
    /// </summary>
    public static readonly HydraDokkenEndpoint Access = new("/access");

    /// <summary>
    /// Retrieves the Hydra access token.
    /// </summary>
    public static readonly HydraProdEndpoint AuthToken = new("/sessions/auth/token");

    /// <summary>
    /// Retrives Hydra account information.
    /// </summary>
    public static readonly HydraProdEndpoint MyAccount = new("/accounts/me");

    /// <summary>
    /// Retrives Hydra account information.
    /// </summary>
    public static readonly HydraDokkenEndpoint AccountLookup = new("/accounts/");
}
