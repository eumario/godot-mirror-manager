using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GodotMirrorManager.Data;
using GodotMirrorManager.SimpleAPI;

namespace GodotMirrorManager.EndpointDefintiions;

public class LatestVersionEndpointDefinition : IEndpointDefinition
{
	public void DefineEndpoints(WebApplication app)
	{
		// Nothing to be done here, as there are no Endpoints to LatestVersion Service.
	}

	public void DefineServices(IServiceCollection services)
	{
		services.AddSingleton<ILatestVersionScraper, LatestVersionScraper>();
	}
}
