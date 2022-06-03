using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GodotMirrorManager.Data;
using GodotMirrorManager.SimpleAPI;

namespace GodotMirrorManager.EndpointDefintiions;

public class FSScrapeEndpointDefinition : IEndpointDefinition
{
	public void DefineEndpoints(WebApplication app)
	{
		// Nothing to be done here, as there are no Endpoints to ScraperService.
	}

	public void DefineServices(IServiceCollection services)
	{
		services.AddSingleton<IFSScraperService, FSScraperService>();
	}
}
