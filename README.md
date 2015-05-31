# Shotgun

A [RestSharp](http://restsharp.org/)-inspired HTTP client designed to give granular control over control flow behaviour.

## Example
```csharp
var request = new ShotgunRequest
{
    Uri = new Uri(url),
    Method = Method.POST,
    Body = new StringRequestBody
    {
        ContentType = "text/plain",
        Content = "Oh hey"
    }
};

var client = new ShotgunClient();
response = client.Execute(request);

Console.WriteLine(response.Content);
```