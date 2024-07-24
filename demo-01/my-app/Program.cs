using OpenFeature.Contrib.Providers.Flagd;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var flagdProvider = new FlagdProvider(new Uri("http://flagd:8013"));

// Set the flagdProvider as the provider for the OpenFeature SDK
await OpenFeature.Api.Instance.SetProviderAsync(flagdProvider);

var client = OpenFeature.Api.Instance.GetClient("my-app");

app.MapGet("/", async () => {

    // Get the flag
    var flag = await client.GetBooleanValue("feature-a", false, null);

    if (flag)
        return "Hello the whole new world!";
    else 
        return "Hello World!";
});

app.Run();
