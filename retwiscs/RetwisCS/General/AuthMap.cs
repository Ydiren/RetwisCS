using StackExchange.Redis;

namespace RetwisCS.General;

public interface IAuthMap
{
    Task AddUserSecretMap(string secret, long userId);
    Task<(bool authExists, string auth)> GetUserIdBySecret(string secret);
}

public class AuthMap : IAuthMap
{
    private readonly IDatabase _redis;

    public AuthMap(IDatabase redis)
    {
        _redis = redis;
    }

    public async Task AddUserSecretMap(string secret, long userId)
    {
        await _redis.HashSetAsync("auths", [
            new(secret, userId)
        ]);
    }

    public async Task<(bool authExists, string auth)> GetUserIdBySecret(string secret)
    {
        var userId = await _redis.HashGetAsync("auths", secret);
        return userId.IsNull
            ? (false, string.Empty)
            : (true, userId.ToString());
    }
}
