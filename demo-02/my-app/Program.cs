using OpenFeature.Contrib.Providers.Flagd;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var flagdProvider = new FlagdProvider(new Uri("http://flagd:8013"));

// Set the flagdProvider as the provider for the OpenFeature SDK
await OpenFeature.Api.Instance.SetProviderAsync(flagdProvider);

var client = OpenFeature.Api.Instance.GetClient("my-app");

var val = await client.GetBooleanValue("feature-a", false, null);

if (val)
    app.MapGet("/", () => "Hello the whole new world!");
else
    app.MapGet("/", () => "Hello World!");

app.Run();
