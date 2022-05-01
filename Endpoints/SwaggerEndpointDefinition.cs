using Gmm.SimpleMinimal;

namespace Gmm.Endpoints;

public class SwaggerEndpointDefinition : IEndpointDefinition
{
	public void DefineEndpoints(WebApplication app)
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	public void DefineServices(IServiceCollection services)
	{
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();
	}
}