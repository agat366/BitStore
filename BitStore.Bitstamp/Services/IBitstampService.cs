using BitStore.Bitstamp.Models;

namespace BitStore.Bitstamp.Services;

/// <summary>
/// Defines operations for interacting with the Bitstamp cryptocurrency exchange API.
/// </summary>
public interface IBitstampService
{
    /// <summary>
    /// Retrieves the order book for a given currency pair.
    /// </summary>
    /// <param name="currencyFrom">The base currency in the pair.</param>
    /// <param name="currencyTo">The quote currency in the pair.</param>
    /// <returns>The order book for the specified currency pair.</returns>
    Task<OrderBook> GetOrderBookAsync(string currencyFrom, string currencyTo);
}