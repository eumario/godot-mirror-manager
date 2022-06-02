using GodotMirrorManager.Data;
using GodotMirrorManager.SimpleAPI;
using Hangfire;
using Hangfire.LiteDB;
using HangfireBasicAuthenticationFilter;

namespace GodotMirrorManager.EndpointDefintiions;

public class HangfireEndpointDefintiion : IEndpointDefinition
{
	public void DefineEndpoints(WebApplication app)
	{
		app.UseHangfireDashboard(
			pathMatch: "/hangfire",
			options: new DashboardOptions {
				DashboardTitle = "Godot Mirror Manager",
				Authorization = new[] {
					new HangfireCustomBasicAuthenticationFilter {
						User = "user",
						Pass = "password"
					}
				}
			}
		);

		RecurringJob.AddOrUpdate(
			recurringJobId: "mirrorSiteScrape",
			methodCall: () => app.Services.GetRequiredService<IScraperService>().ScrapeSites(),
			cronExpression: Cron.Hourly
		);
	}

	public void DefineServices(IServiceCollection services)
	{
		services.AddHangfire(x =>
		{
			x.UseLiteDbStorage("gmm-hf.db");
		});
		services.AddHangfireServer();
	}
}
