using GodotMirrorManager.Data;
using GodotMirrorManager.SimpleAPI;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Configure Settings
builder.Services.Configure<GmmDatabaseSettings>(builder.Configuration.GetSection("GmmDatabaseSettings"));
// Add Endpoints
builder.Services.AddEndpointDefinitions(typeof(IEndpointDefinition));

// Build our Application
var app = builder.Build();
app.MapGet("/", () => "Godot Mirror Manager!");
// Use Endpoints
app.UseEndpointDefinitions();

// Let's get to running!
app.Run();
