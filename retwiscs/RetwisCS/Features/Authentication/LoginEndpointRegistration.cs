using RetwisCS.Features.Authentication.Requests;
using RetwisCS.General;
using StackExchange.Redis;

namespace RetwisCS.Features.Authentication;

public class LoginEndpointRegistration : IRegisterEndpoint
{
    private readonly ILoginService _loginService;

    public LoginEndpointRegistration(ILoginService loginService)
    {
        _loginService = loginService;
    }

    public void Register(WebApplication app)
    {
        app.MapGet("/login", async (LoginRequest request, IResponseCookies cookies) =>
            {
                var isAuthenticated = await _loginService.Login(request);
                if (!isAuthenticated)
                {
                    return Results.Unauthorized();
                }

                cookies.Append("auth", request.Password);
            })
            .WithName("Login")
            .WithOpenApi();

    }
}

internal interface ILoginService
{
    Task<bool> Login(LoginRequest request);
}

internal class LoginService : ILoginService
{
    private readonly IDatabase _redis;
    private readonly IUserIdMap _userIdMap;
    private readonly IAuthMap _authMap;

    public LoginService(IDatabase redis, IUserIdMap userIdMap, IAuthMap authMap)
    {
        _userIdMap = userIdMap;
        _authMap = authMap;
        _redis = redis;
    }

    public async Task<bool> Login(LoginRequest request)
    {
        var (userExists, userId) = await _userIdMap.GetUserId(request.Username);
        if (!userExists)
        {
            return false;
        }

        var realPassword = await _redis.HashGetAsync($"user:{userId}", "password");

        return realPassword == request.Password;
    }
}
