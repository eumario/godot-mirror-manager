using Gmm.SimpleMinimal;
using Hangfire;
using Hangfire.LiteDB;

namespace Gmm.Endpoints;

public class HangfireEndpointDefinition : IEndpointDefinition
{
	public void DefineEndpoints(WebApplication app)
	{

	}

	public void DefineServices(IServiceCollection services)
	{
		services.AddHangfire(configuration =>
		{
			configuration.UseLiteDbStorage("./gmm_hf.db");
		});
		services.AddHangfireServer();
	}
}