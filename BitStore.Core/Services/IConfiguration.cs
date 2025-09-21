namespace BitStore.Core.Services;

public interface IConfiguration
{
    string PrimaryCurrency { get; }
}

public class Configuration : IConfiguration
{
    public Configuration(string primaryCurrency)
    {
        PrimaryCurrency = primaryCurrency;
    }

    public string PrimaryCurrency { get; }
}