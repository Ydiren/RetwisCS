using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using RetwisCS.Features.UserRegistration.Requests;
using RetwisCS.Features.UserRegistration.Responses;
using RetwisCS.General;
using StackExchange.Redis;

namespace RetwisCS.Features.UserRegistration;

public interface INewUserRegister
{
    Task<IResult> RegisterUser(UserRegistrationRequest user);
}

internal class NewUserRegister : INewUserRegister
{
    private readonly IDatabase _redis;
    private readonly IUserIdMap _userIdMap;
    private readonly IAuthMap _authMap;

    public NewUserRegister(IDatabase redis, IUserIdMap userIdMap, IAuthMap authMap)
    {
        _redis = redis;
        _userIdMap = userIdMap;
        _authMap = authMap;
    }

    public async Task<IResult> RegisterUser(UserRegistrationRequest user)
    {
        var id = await GetNextUserId();
        var userKey = new UserKey(id);
        await AddUserToCache(user, userKey);

        await GenerateAndStoreUserSecret(userKey);
        await AddUserToUserIdMap(id, user.Username);
        var response = new UserRegistrationResponse((int)id, user.Username, user.Email);

        return Results.Created($"/user/{response.Username}", response);
    }

    private async Task<long> GetNextUserId()
    {
        var id = await _redis.StringIncrementAsync("next_user_id");
        return id;
    }

    private async Task AddUserToCache(UserRegistrationRequest user, UserKey userKey)
    {
        await _redis.HashSetAsync(userKey.ToString(), new HashEntry[]
        {
            new("username", user.Username),
            new("email", user.Email),
            new("password", user.Password),
        });
    }

    private async Task GenerateAndStoreUserSecret(UserKey userKey)
    {
        using var hasher = SHA512.Create();
        var hashBytes = hasher.ComputeHash(Encoding.Default.GetBytes(DateTime.UtcNow.ToString("F")));
        var hash = Convert.ToHexString(hashBytes);
        await _redis.HashSetAsync(userKey.ToString(), [
            new("auth", hash)
        ]);

        await _authMap.AddUserSecretMap(hash, userKey.Id);
    }

    private async Task AddUserToUserIdMap(long id, string username)
    {
        await _userIdMap.AddUser(username, id);
    }
}
