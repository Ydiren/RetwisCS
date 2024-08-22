using RetwisCS.General;

namespace RetwisCS;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddEndpoints(this WebApplicationBuilder builder)
    {

        // Automatically register all endpoints in the assembly that implement the IRegisterEndpoint interface
        builder.Services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(classes => classes.AssignableTo<IRegisterEndpoint>())
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        builder.Services.AddSingleton<WebApplicationExtensions.EndpointRegister>();

        return builder;
    }
}
