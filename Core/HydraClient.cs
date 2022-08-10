﻿using HydraDotNet.Core.Api;
using HydraDotNet.Core.Authentication;
using HydraDotNet.Core.Encoding;
using HydraDotNet.Core.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using HydraDotNet.Core.Extensions;

namespace HydraDotNet.Core;

public class HydraClient
{
    public string? Username { get; set; }
    public string? AccountId { get; set; }

    private RestClient Client { get; set; }
    private EpicAuthContainer EpicAuth { get; set; }

    /// <summary>
    /// Hydra client constructor.
    /// </summary>
    /// <param name="epicAuth">Container with external Epic games authentication information.</param>
    public HydraClient(ExternalEpicAuthContainer epicAuth)
    {
        EpicAuth = epicAuth;

        Client = new RestClient()
        {
            Authenticator = new HydraAuthenticator(),
            AcceptedContentTypes = new[] { "application/x-ag-binary" }
        };
    }

    /// <summary>
    /// Creates access to authorized endpoints.
    /// </summary>
    /// <param name="onLoginSuccessful">Optional: Executes after logging in successfully.</param>
    /// <param name="onLoginFailed">Optional: Executes if logging in fails. If this is not passed in then an exception will be thrown on failure.</param>
    /// <returns></returns>
    public async Task LoginAsync(Action<WarnerAccount>? onLoginSuccessful = null, Action<HydraApiResponse?, HttpStatusCode?, Exception>? onLoginFailed = null)
    {
        var response = await GetAccountAccessInfoAsync();
        var accountAccess = response.GetContent();

        var auth = new HydraAuthContainer(this);

        if (accountAccess is null ||
            response.StatusCode != HttpStatusCode.OK ||
            string.IsNullOrEmpty(auth.NetworkToken) ||
            string.IsNullOrEmpty(auth.Token)) // do this to test that everything is right
        {
            var ex = new TaskCanceledException($"Access token request unsuccessful with response status code {response.StatusCode}");

            if (onLoginFailed is null)
                throw ex;

            onLoginFailed(response, response.StatusCode, ex);

            return;
        }

        if (Client.Authenticator is HydraAuthenticator authenticator)
            authenticator.SetAuthContainer(auth);

        var account = await GetAccountAsync();

        if (account is null)
        {
            var ex = new TaskCanceledException($"Could not retrieve account information.");

            if (onLoginFailed is null)
                throw ex;

            onLoginFailed(null, null, ex);

            return;
        }

        Username = account.username;

        if (accountAccess.TryGetNested("account", out var accountData) &&
            accountData.TryGetValueAs("id", out string? id))
        {
            AccountId = id;
        }


        if (onLoginSuccessful is not null)
            onLoginSuccessful(account);
    }

    /// <summary>
    /// Gets the response for the /access endpoint which contains account and player data.
    /// </summary>
    /// <returns></returns>
    public HydraApiResponse GetAccountAccessInfo()
    {
        var body = new HydraAccessRequestBody();
        body.auth.epic = EpicAuth.AccessToken;

        using var encoder = new HydraEncoder();
        encoder.WriteValue(body);

        var request = Endpoints.Access.CreateRequest(encoder.GetBuffer(), Method.Post);
        return DoRequest(request);
    }

    /// <summary>
    /// Asynchronously retrieves sensitive account information.
    /// </summary>
    /// <returns>Account data. Can be null.</returns>
    public async Task<WarnerAccount?> GetAccountAsync()
    {
        var req = Endpoints.MyAccount.CreateRequest();

        var ret = await DoRequestAsync(req);

        return ret.GetContent<WarnerAccount>();
    }

    /// <summary>
    /// Asynchronously retrieves account information with a user id.
    /// </summary>
    /// <param name="id">Id of the account you are trying to retrieve.</param>
    /// <returns></returns>
    public async Task<HydraApiResponse> GetAccountByIdAsync(string id)
    {
        var body = new HydraAccountLookupRequestBody()
        {
            _model_update = true,
            operations = new string[][] { new[] { "set", "data.LastLoginPlatform", "EPlatform::PC_Epic" } }
        };

        await using var encoder = new HydraEncoder();
        encoder.WriteValue(body);

        var request = Endpoints.AccountLookup.CreateRequest(await encoder.GetBufferAsync(), Method.Put, id);

        return await DoRequestAsync(request);
    }

    /// <summary>
    /// Asynchronously gets the response for the /access endpoint which contains account and player data.
    /// </summary>
    /// <returns></returns>
    public async ValueTask<HydraApiResponse> GetAccountAccessInfoAsync()
    {
        var body = new HydraAccessRequestBody();
        body.auth.epic = EpicAuth.AccessToken;

        await using var encoder = new HydraEncoder();
        encoder.WriteValue(body);

        var request = Endpoints.Access.CreateRequest(await encoder.GetBufferAsync(), Method.Post);
        return await DoRequestAsync(request);
    }

    /// <summary>
    /// Executes a request to a Hydra endpoint.
    /// </summary>
    /// <param name="req">Endpoint to execute the request upon.</param>
    /// <returns>Response from the endpoint.</returns>
    public HydraApiResponse DoRequest(HydraApiRequest req)
    {
        var restResponse = req.GetResponse(Client);
        return new(restResponse);
    }

    /// <summary>
    /// Asynchronously executes a request to a Hydra endpoint.
    /// </summary>
    /// <param name="req">Request to execute.</param>
    /// <returns>Response from the endpoint.</returns>
    public async ValueTask<HydraApiResponse> DoRequestAsync(HydraApiRequest req)
    {
        var restResponse = await req.GetResponseAsync(Client);
        return new(restResponse);
    }

    /// <summary>
    /// Executes a request to a Hydra endpoint.
    /// </summary>
    /// <param name="req">Request to execute.</param>
    /// <param name="response">Response from the endpoint.</param>
    /// <returns>If the response returned an OK status code.</returns>
    public bool TryDoRequest(HydraApiRequest req, out HydraApiResponse response)
    {
        response = DoRequest(req);
        return response.StatusCode == HttpStatusCode.OK;
    }
}
