/*
 * Code written by Elfocrash (https://github.com/Elfocrash) Nick Chapsas
 * Minimal API in clean secitonalized form, instead of all in single file.
 */


namespace Gmm.SimpleMinimal;

public interface IEndpointDefinition
{
	void DefineServices(IServiceCollection services);

	void DefineEndpoints(WebApplication app);
}