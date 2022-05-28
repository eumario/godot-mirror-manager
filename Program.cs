using Gmm.SimpleMinimal;
using Gmm.Web;
using Hangfire;
using Hangfire.LiteDB;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointDefinitions(typeof(IEndpointDefinition));

var app = builder.Build();
app.UseEndpointDefinitions();

app.Run();

// Scrapper scrape = new Scrapper();
// Console.WriteLine($"Last Update for https://downloads.tuxfamily.org/godotengine/: {scrape.Settings.LastUpdate["https://downloads.tuxfamily.org/godotengine/"].ToString()}");
// DateTime start = DateTime.Now;
// scrape.ScrapeSites();
// Console.WriteLine($"Elapsed Time: {DateTime.Now - start}");