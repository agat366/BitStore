namespace BitStore.Core.Services;

public class Configuration : IConfiguration
{
    public Configuration(string secondaryCurrency)
    {
        SecondaryCurrency = secondaryCurrency;
    }

    public string SecondaryCurrency { get; }
}