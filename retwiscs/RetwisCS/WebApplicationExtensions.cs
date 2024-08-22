using System.Reflection;
using RetwisCS.Features.Authentication;
using RetwisCS.General;

namespace RetwisCS;

public static class WebApplicationExtensions
{
    public static WebApplication RegisterEndpoints(this WebApplication app)
    {
        var register = app.Services.GetRequiredService<EndpointRegister>();
        register.Register(app);

        return app;
    }

    internal class EndpointRegister
    {
        private readonly IEnumerable<IRegisterEndpoint> _registrations;

        public EndpointRegister(IEnumerable<IRegisterEndpoint> registrations)
        {
            _registrations = registrations;
        }

        public void Register(WebApplication app)
        {
            foreach (var endpoint in _registrations)
            {
                endpoint.Register(app);
            }
        }
    }
}
