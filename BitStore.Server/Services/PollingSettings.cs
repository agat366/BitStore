namespace BitStore.Server.Services;

/// <summary>
/// Configuration settings for order book polling service.
/// </summary>
public class PollingSettings
{
    public const string SectionName = "PollingSettings";
    
    public string SecondaryCurrency { get; set; } = "EUR";
    public int UpdateIntervalSeconds { get; set; } = 2;
}