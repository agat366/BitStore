namespace BitStore.Server.Context;

/// <summary>
/// Provides access to the current user's context and identity information.
/// </summary>
public interface IUserContext
{
    string Username { get; }
    string UserId { get; }
}