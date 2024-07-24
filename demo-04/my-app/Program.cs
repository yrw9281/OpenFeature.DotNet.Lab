using OpenFeature.Contrib.Providers.Flagd;
using OpenFeature.Model;
using Prometheus;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddEndpointsApiExplorer();
builder.Services.UseHttpClientMetrics();

var app = builder.Build();

// Add flagd provider
var flagdProvider = new FlagdProvider(new Uri("http://localhost:8013"));
// Set OpenFeature provider
await OpenFeature.Api.Instance.SetProviderAsync(flagdProvider);
// Create OpenFeature client
var client = OpenFeature.Api.Instance.GetClient("my-app");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Metrics
var histogram = Metrics.CreateHistogram("http_request_duration_ms", "Duration of HTTP requests in ms",
    new HistogramConfiguration
    {
        LabelNames = new[] { "method", "route", "code", "flag" }
    });

app.UseMetricServer();
app.UseHttpMetrics();
app.UseRouting();
app.MapGet("/", async () =>
{
    var enableFeatureFlag = await client.GetBooleanValue("feature-a", false, null);

    if (enableFeatureFlag)
        return "Hello the whole new world!";
    else
        return "Hello World!";
});
app.MapGet("/world", async context =>
{
    // Get header information
    var email = context.Request.Headers["email"].ToString();
    // Get bool with context
    var enableExperimentFeatureFlag = await client.GetBooleanValue("experiment", false, EvaluationContext.Builder().Set("email", email).Build());
    // Metrics setup
    var metricsFlag = enableExperimentFeatureFlag ? "Experiment" : "Normal";

    var stopwatch = Stopwatch.StartNew();
    context.Response.OnStarting(() =>
    {
        stopwatch.Stop();
        histogram.WithLabels(context.Request.Method, context.Request.Path, context.Response.StatusCode.ToString(), metricsFlag)
                    .Observe(stopwatch.Elapsed.TotalMilliseconds);
        return Task.CompletedTask;
    });

    if (enableExperimentFeatureFlag)
    {
        await Task.Delay(new Random().Next(500)); // experiment workflow is slower.
        await context.Response.WriteAsync("Hello the whole new world!");
    }
    else
    {
        await Task.Delay(new Random().Next(200)); // current workflow is faster.
        await context.Response.WriteAsync("Hello World!");
    }
});

app.Run();
