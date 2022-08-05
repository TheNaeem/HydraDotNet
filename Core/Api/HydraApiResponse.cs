using RestSharp;
using Newtonsoft.Json;
using HydraDotNet.Core.Encoding;
using System.Net;
using System.Collections.Generic;

namespace HydraDotNet.Core.Api;

/// <summary>
/// This classes main purpose is to handle binary responses if necessary.
/// </summary>
public class HydraApiResponse
{
    private RestResponse _response;
    public HttpStatusCode StatusCode { get => _response.StatusCode; }
    public bool IsBinary { get; init; }
    public bool IsJSON { get; init; }
    private object? DecodedContent { get; init; }

    public HydraApiResponse(RestResponse response)
    {
        _response = response;

        if (!_response.IsSuccessful) return;

        IsBinary = response.ContentType == "application/x-ag-binary";
        IsJSON = response.ContentType == "application/json";

        if (IsBinary && _response.StatusCode != HttpStatusCode.InternalServerError && _response.RawBytes is not null)
        {
            using var decoder = new HydraDecoder(_response.RawBytes);
            DecodedContent = decoder.ReadValue();
        }
    }

    /// <summary>
    /// Returns a deserialized response for a generic type. Try to avoid this if you know your response is binary.
    /// </summary>
    /// <returns>An instance of the type passed into the function. Can return default.</returns>
    public T? GetContent<T>() where T : class, new()
    {
        if (!IsBinary && !IsJSON) return default;

        if (IsJSON)
        {
            if (string.IsNullOrEmpty(_response.Content))
                return default;

            return JsonConvert.DeserializeObject<T>(_response.Content);
        }

        var dict = GetContent();

        if (dict is null) return default;

        var ret = new T();
        var obj = (object)ret;

        HydraDecoder.ReadToObject(ref obj, dict);

        return ret;
    }

    /// <summary>
    /// Gets the response content as a string.
    /// </summary>
    /// <returns>Response string. Can be null.</returns>
    public string? GetContentString()
    {
        if (!IsBinary) return _response.Content;
        else return JsonConvert.SerializeObject(DecodedContent);
    }

    /// <summary>
    /// Gets the response as a key string to object dictionary. Best option if expecting a binary response.
    /// </summary>
    /// <returns>Response content deserialized into a dictionary. Can return default.</returns>
    public Dictionary<object, object?>? GetContent()
    {
        if (IsBinary)
        {
            if (DecodedContent is not Dictionary<object, object?> dict)
                return default;

            return dict;
        }

        if (!IsJSON || string.IsNullOrEmpty(_response.Content))
            return default;

        return JsonConvert.DeserializeObject<Dictionary<object, object?>>(_response.Content);
    }

    public override string? ToString() => GetContentString();
}
