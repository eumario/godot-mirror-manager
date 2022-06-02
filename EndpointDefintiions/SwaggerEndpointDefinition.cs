using GodotMirrorManager.SimpleAPI;

namespace GodotMirrorManager.EndpointDefintiions;

public class SwaggerEndpointDefinition : IEndpointDefinition
{
	public void DefineEndpoints(WebApplication app)
	{
		if (app.Environment.IsDevelopment()) {
			app.UseSwagger();
			app.UseSwaggerUI();
		}
	}

	public void DefineServices(IServiceCollection services)
	{
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();
	}
}
