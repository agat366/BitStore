using System.Text.Json.Serialization;

namespace BitStore.Bitstamp.Dto;

/// <summary>
/// DTO for deserializing Bitstamp API order book response.
/// </summary>
public class BitstampOrderBook
{
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    // don't really need this, but keeping it for reference
    // [JsonPropertyName("microtimestamp")]
    // public string MicroTimestamp { get; set; } = string.Empty;

    [JsonPropertyName("bids")]
    public IEnumerable<string[]> Bids { get; set; } = [];

    [JsonPropertyName("asks")]
    public IEnumerable<string[]> Asks { get; set; } = [];
}