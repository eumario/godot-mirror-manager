using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GodotMirrorManager.SimpleAPI;

public interface IEndpointDefinition
{
    void DefineServices(IServiceCollection services);

	void DefineEndpoints(WebApplication app);
}
