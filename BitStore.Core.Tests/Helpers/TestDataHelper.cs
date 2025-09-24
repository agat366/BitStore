using BitStore.Bitstamp.Models;

namespace BitStore.Core.Tests.Helpers;

/// <summary>
/// Provides helper methods for creating test data.
/// </summary>
public static class TestDataHelper
{
    private static readonly Random _random = new();

    /// <summary>
    /// Creates a sample order book with realistic random data.
    /// </summary>
    /// <param name="primaryCurrency">Primary currency symbol (default: BTC)</param>
    /// <param name="secondaryCurrency">Secondary currency symbol (default: EUR)</param>
    /// <param name="bidCount">Number of bid entries to generate (default: 5)</param>
    /// <param name="askCount">Number of ask entries to generate (default: 5)</param>
    /// <returns>An OrderBook with randomized but realistic data</returns>
    public static OrderBook CreateSampleOrderBook(
        string primaryCurrency = "BTC",
        string secondaryCurrency = "EUR")
    {
        const decimal basePrice = 80000m;
        const decimal priceVariation = 10000m;
        const decimal maxAmount = 55m; // Maximum BTC amount per order

        // Generate bids (slightly below base price)
        int bidCount = 10 + _random.Next(100);
        var bids = Enumerable.Range(0, bidCount)
            .Select(_ => new OrderBookEntry 
            { 
                Price = basePrice - (decimal)(_random.NextDouble() * (double)priceVariation),
                Amount = (decimal)(_random.NextDouble() * (double)maxAmount)    
            })
            .OrderByDescending(b => b.Price) // Sort bids by price descending
            .ToArray();

        // Generate asks (slightly above base price)
        int askCount = 10 + _random.Next(100);
        var asks = Enumerable.Range(0, askCount)
            .Select(_ => new OrderBookEntry 
            { 
                Price = basePrice + (decimal)(_random.NextDouble() * (double)priceVariation),
                Amount = (decimal)(_random.NextDouble() * (double)maxAmount)
            })
            .OrderBy(a => a.Price) // Sort asks by price ascending
            .ToArray();
    
        return new OrderBook
        {
            PrimaryCurrency = primaryCurrency,
            SecondaryCurrency = secondaryCurrency,
            Timestamp = DateTimeOffset.UtcNow,
            Bids = bids,
            Asks = asks
        };
    }
}