namespace BitStore.Server.Context;

public interface IUserContext
{
    string Username { get; }
    string UserId { get; }
}