﻿using System;

namespace HydraDotNet.Core.Authentication;

public class ExternalEpicAuthContainer : EpicAuthContainer
{
    /// <summary>
    /// Creates an external auth container which can be used to get Hydra credentials. Will not continuously update the passed in launcher auth container, the caller is responsible for that.
    /// </summary>
    /// <param name="launcherAuthContainer">Auth container of a launcher client.</param>
    /// <param name="authClient">Auth client that will be generated.</param>
    /// <param name="scope">The scope of the authorization.</param>
    /// <exception cref="NullReferenceException">Throws if the exchange code can't be retrived.</exception>
    public ExternalEpicAuthContainer(EpicAuthContainer launcherAuthContainer, string authClient, string scope = "basic_profile friends_list presence openid")
        : base(authClient)
    {
        _scope = scope;

        var exchangeCode = EpicAuthUtil.GetExchangeCode(launcherAuthContainer.AccessToken);

        if (string.IsNullOrEmpty(exchangeCode))
            throw new NullReferenceException("Could not retrieve exchange code with launcher access token.");

        var externalAuth = EpicAuthUtil.GetExternalAuth(exchangeCode, authClient, _scope);

        if (externalAuth is null || string.IsNullOrEmpty(externalAuth.refresh_token))
        {
            return;
        }

        RefreshToken = externalAuth.refresh_token;

        UpdateToken(); // calling this here is hacky but it will handle the expiration for us
    }
}
