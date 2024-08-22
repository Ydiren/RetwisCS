using RetwisCS.Features.UserRegistration.Requests;
using RetwisCS.Features.UserRegistration.Responses;
using RetwisCS.General;

namespace RetwisCS.Features.UserRegistration;

public class RegisterNewUserEndpointRegistration : IRegisterEndpoint
{
    public void Register(WebApplication app)
    {
        app.MapPost("/user", async (UserRegistrationRequest user, INewUserRegister userRegistration) =>
                await userRegistration.RegisterUser(user))
            .WithName("RegisterNewUser")
            .Produces<UserRegistrationResponse>(201)
            .WithSummary("Register a new user")
            .WithDescription("Adds a new user to the system and returns the created object with an ID.")
            .WithOpenApi();
    }
}
