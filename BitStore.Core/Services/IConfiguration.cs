namespace BitStore.Core.Services;

/// <summary>
/// Provides access to application configuration settings.
/// </summary>
public interface IConfiguration
{
    string SecondaryCurrency { get; }
}