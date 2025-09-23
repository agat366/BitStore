namespace BitStore.Core.Services;

/// <summary>
/// Implementation of IConfiguration that provides application settings.
/// </summary>
public class Configuration : IConfiguration
{
    public Configuration(string secondaryCurrency)
    {
        SecondaryCurrency = secondaryCurrency;
    }

    public string SecondaryCurrency { get; }
}