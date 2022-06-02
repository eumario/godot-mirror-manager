using GodotMirrorManager.Data;
using GodotMirrorManager.Models;
using GodotMirrorManager.SimpleAPI;

namespace GodotMirrorManager.EndpointDefintiions;

public class MirrorEndpointDefinition : IEndpointDefinition
{
	public void DefineEndpoints(WebApplication app)
	{
		app.MapGet("/mirrors", GetAll);
		app.MapGet("/mirrors/{id}", Get);
		app.MapPost("/mirrors", Create);
		app.MapPut("/mirrors/{id}", Update);
		app.MapDelete("/mirrors/{id}", Delete);
	}

	internal List<MirrorSite> GetAll(IMirrorSiteService mirrorService) => mirrorService.Get();
	
	internal IResult Get(IMirrorSiteService mirrorService, int id)
	{
		var mirror = mirrorService.Get(id);
		return mirror is not null ? Results.Ok(mirror) : Results.NotFound();
	}

	internal IResult Create(IMirrorSiteService mirrorService, MirrorSite site)
	{
		mirrorService.Create(site);
		return Results.Created("/mirror/{site.Id}", site);
	}

	internal IResult Update(IMirrorSiteService mirrorService, int id, MirrorSite updatedSite) {
		var site = mirrorService.Get(id);
		if (site is null)
			return Results.NotFound();
		
		mirrorService.Update(updatedSite);
		return Results.NoContent();
	}

	internal IResult Delete(IMirrorSiteService mirrorService, int id) {
		var site = mirrorService.Get(id);
		if (site is null)
			return Results.NotFound();
		
		mirrorService.Remove(id);
		return Results.NoContent();
	}

	public void DefineServices(IServiceCollection services)
	{
		services.AddSingleton<IMirrorSiteService, MirrorSiteService>();
	}
}
