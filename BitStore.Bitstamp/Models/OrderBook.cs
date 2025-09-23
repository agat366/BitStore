namespace BitStore.Bitstamp.Models
{
    /// <summary>
    /// Represents a local model of order book.
    /// </summary>
    public class OrderBook
    {
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        public IEnumerable<OrderBookEntry> Bids { get; set; } = [];
        public IEnumerable<OrderBookEntry> Asks { get; set; } = [];
        
        public required string PrimaryCurrency { get; set; }
        public required string SecondaryCurrency { get; set; }
    }

    /// <summary>
    /// Represents a local order book entry (bid or ask).
    /// </summary>
    public class OrderBookEntry
    {
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }
}
