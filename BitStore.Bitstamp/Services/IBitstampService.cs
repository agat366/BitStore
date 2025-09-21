using BitStore.Bitstamp.Models;

namespace BitStore.Bitstamp.Services;

public interface IBitstampService
{
    Task<OrderBook> GetOrderBookAsync(string currencyFrom, string currencyTo);
}