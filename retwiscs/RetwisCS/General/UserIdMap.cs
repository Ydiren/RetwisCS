using StackExchange.Redis;

namespace RetwisCS.General;

internal interface IUserIdMap
{
    Task AddUser(string username, long id);
    Task<(bool Exists, string Id)> GetUserId(string username);
}

internal class UserIdMap : IUserIdMap
{
    private readonly IDatabase _redis;

    public UserIdMap(IDatabase redis)
    {
        _redis = redis;
    }

    public async Task AddUser(string username, long id)
    {
        await _redis.HashSetAsync("users", [
            new(username, id)
        ]);
    }

    public async Task<(bool Exists, string Id)> GetUserId(string username)
    {
        var id = await _redis.HashGetAsync("users", username);
        return id.IsNull
            ? (false, string.Empty)
            : (true, id.ToString());
    }
}
