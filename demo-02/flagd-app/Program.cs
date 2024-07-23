using System.IO;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string FILEPATH = "../flagd/flagd.json";

app.MapGet("/", () => {
    return File.ReadAllText(FILEPATH);
});

app.Run();
