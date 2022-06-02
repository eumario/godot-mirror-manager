using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GodotMirrorManager.Data;
using GodotMirrorManager.Models;
using GodotMirrorManager.SimpleAPI;

namespace GodotMirrorManager.EndpointDefintiions;

public class ListingEndpointDefinition : IEndpointDefinition
{
	public void DefineEndpoints(WebApplication app)
	{
		app.MapGet("/listings/{mirrorId}", GetAll);
		app.MapGet("/listings/{mirrorId}/{version}", GetByVersion);
		app.MapGet("/listings/{mirrorId}/tags/{tags}", GetByTags);
		app.MapGet("/listings/{mirrorId}/tag/{tag}", GetByTag);
		app.MapGet("/listings/engine/{engineId}", GetByEngineId);
	}

	internal List<EngineUrl> GetAll(IEngineUrlService engineService, int mirrorId) 
	{

		return engineService.GetAll(mirrorId);
	}

	internal IResult GetByVersion(IEngineUrlService engineService, int mirrorId, string version)
	{
		var engine = engineService.GetByVersion(version, mirrorId);
		return engine is not null ? Results.Ok(engine) : Results.NotFound();
	}

	internal IResult GetByTags(IEngineUrlService engineService, int mirrorId, string tags)
	{
		var engine = engineService.GetByTags(tags.Split(",").ToList<string>(), mirrorId);
		return engine is not null ? Results.Ok(engine) : Results.NotFound();
	}

	internal IResult GetByTag(IEngineUrlService engineService, int mirrorId, string tag)
	{
		var engine = engineService.GetByTag(tag, mirrorId);
		return engine is not null ? Results.Ok(engine) : Results.NotFound();
	}

	internal IResult GetByEngineId(IEngineUrlService engineService, int engineId)
	{
		var engine = engineService.Get(engineId);
		return engine is not null ? Results.Ok(engine) : Results.NotFound();
	}

	public void DefineServices(IServiceCollection services)
	{
		services.AddSingleton<IEngineUrlService, EngineUrlService>();
	}
}
