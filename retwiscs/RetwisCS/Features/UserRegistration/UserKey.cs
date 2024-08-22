namespace RetwisCS.Features.UserRegistration;

internal readonly record struct UserKey(long Id)
{
    public override string ToString()
    {
        return "user:" + Id;
    }
}
