namespace BitStore.Core.Services;

/// <inheritdoc cref="IConfiguration" />
public class Configuration(string secondaryCurrency) : IConfiguration
{
    public string SecondaryCurrency { get; } = secondaryCurrency;
}