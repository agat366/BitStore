namespace BitStore.Bitstamp.Models
{
    public class OrderBook
    {
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        public IEnumerable<OrderBookEntry> Bids { get; set; } = [];
        public IEnumerable<OrderBookEntry> Asks { get; set; } = [];
        public string PrimaryCurrency { get; set; }
        public string SecondaryCurrency { get; set; }
    }

    public class OrderBookEntry
    {
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }
}
