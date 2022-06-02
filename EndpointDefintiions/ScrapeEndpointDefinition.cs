using GodotMirrorManager.Data;
using GodotMirrorManager.SimpleAPI;

namespace GodotMirrorManager.EndpointDefintiions;

public class ScrapeEndpointDefinition : IEndpointDefinition
{
	public void DefineEndpoints(WebApplication app)
	{
		// Nothing to be done here, as there are no Endpoints to ScraperService.
	}

	public void DefineServices(IServiceCollection services)
	{
		services.AddSingleton<IScraperService, ScraperService>();
	}
}
