# HydraDotNet

*This project is not Associated with Warner Bros. or Player First Games.*

HydraDotNet is a library used to communicate with and use the Hydra API. HydraDotNet provides support for decoding and encoding data with the Hydra format.

HydraDotNet is still pretty experimental, so please bring awareness to any issues with the library itself.

Any and all contributions are encouraged.

## Authenticating

HydraDotNet currently only supports authenticating with Epic Games. Use a launcherAppClient2 refresh token to do so. See [this](https://github.com/MixV2/EpicResearch) to learn more.

More ways to do so are in consideration for the future, and any contributions for this are greatly encouraged and apprieciated. 

## Usage

Now that you have your launcher client refresh token, you are in charge of managing it and its lifetime. HydraDotNet provides an `EpicAuthContainer` class with `UpdateToken` and `HasAccessTokenExpired` methods which can do so. 

The code below demonstrates how to log into a Hydra client with a launcher refresh token which will cache the refresh token when updated.
```csharp
using HydraDotNet.Core;
using HydraDotNet.Core.Authentication;

Action<string> cacheRefreshLauncher = (string refresh) =>
{
    File.WriteAllText("refreshLauncher.txt", refresh);
};

var launcherAuth = new EpicAuthContainer(File.ReadAllText("refreshLauncher.txt"), AuthClients.LAUNCHER, onRefreshTokenUpdated: cacheRefreshLauncher);
var externalAuth = new ExternalEpicAuthContainer(launcherAuth, AuthClients.MV);

var client = new HydraClient(externalAuth);

await client.LoginAsync(onLoginFailed: Failed);
```

## Requests

Making an API request and getting a response is straightforward. Create a request using a Hydra endpoint class and use the HydraClient to execute the request. HydraClient will handle the authentication.

```csharp
using HydraDotNet.Core.Api;

var request = Endpoints.Inventory.CreateRequest(); // See the overloads for CreateRequest if you want to use a different request method, add parameters, etc.
var response = await client.DoRequestAsync(request);
```

HydraDotNet already provides some endpoints for use in the Endpoints static class, but it's very straightforward to define your own. You just need to know the host name and path of the endpoint.

```csharp
// https://prod-network-api.wbagora.com
HydraProdEndpoint foo = new("/friends/me/invitations/incoming");

// https://dokken-api.wbagora.com
HydraDokkenEndpoint bar = new("/ssc/invoke/get_item_slugs");

var response = await client.DoRequestAsync(foo.CreateRequest());
var response2 = await client.DoRequestAsync(bar.CreateRequest());
```

`DoRequestAsync` returns an instance of `HydraApiResponse` which has some methods for getting the contents of the response.

The response can be retrieved as a `Dictionary<object, object?>` mapping keys to values like JSON. Recommended for speed and safety. Also recommended to be used with the Dictionary extensions that HydraDotNet provides in `HydraDotNet.Core.Extensions`.
```csharp
var dictResponse = response.GetContent();
```

The response can be received as a JSON string.
```csharp
var jsonResponse = response.GetContentString();
```

The response can be retrieved as a generic type. This is still pretty experimental with binary response, so please bring awareness to any exceptions or issues with this.
```csharp
var typeResponse = response.GetContent<MyType>();
```

## Decoding

`HydraApiResponse` should handle decoding and encoding with responses, but using it directly is very straightforward and fast.

To decode a buffer. 
```csharp
using HydraDotNet.Core.Encoding;

var myBuffer = File.ReadAllBytes("foobar.bin");
using var decoder = new HydraDecoder(myBuffer); // Can also be constructed with a Stream.
var val = decoder.ReadValue();
```
If the buffer is an encoded API response, you will most likely only have to call `ReadValue` once and the entire response will be retrieved in a Dictionary. Otherwise, I encourage reading the source code of the `HydraDecoder` class to get an understanding of the format. 

`ReadValue` will return `null` if you have read to the end of the buffer.

## Encoding

To encode data and objects into the Hydra format, use the `HydraEncoder` class. This will most likely be used for sending encoded data in a request body.
```csharp
await using var encoder = new HydraEncoder();

int foo = 69;
string bar = "hello";

encoder.WriteValue(foo);
encoder.WriteValue(bar);

await File.WriteAllBytesAsync("encoded.bin", await encoder.GetBufferAsync());
```

User defined types can also be encoded, but the code is still being perfected, so please bring awareness to any issues with it.
```csharp
var requestBodyObject = new MyHydraRequestBody()
{
    Foo = "foo",
    Barrier = "bar"
};

await using var encoder = new HydraEncoder();
encoder.WriteValue(requestBodyObject);

var requestBodyBuffer = await encoder.GetBufferAsync();

var request = MyEndpoint.CreateRequest(requestBodyBuffer, RestSharp.Method.Post);
var response = await client.DoRequestAsync(request);
```

## License
[MIT](https://choosealicense.com/licenses/mit/)
