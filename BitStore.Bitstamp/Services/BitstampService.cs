using System.Text.Json;
using BitStore.Bitstamp.Dto;
using BitStore.Bitstamp.Models;

namespace BitStore.Bitstamp.Services
{
    /// <inheritdoc cref="IBitstampService" />
    public class BitstampService : IBitstampService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://www.bitstamp.net/api/v2/";

        public BitstampService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<OrderBook> GetOrderBookAsync(string currencyFrom, string currencyTo)
        {
            var requestUri = $"order_book/{currencyFrom.ToLower()}{currencyTo.ToLower()}/";
            var httpResponse = await _httpClient.GetAsync(requestUri);
            var content = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Bitstamp API error: {httpResponse.StatusCode} - {content}");
            }
            
            try
            {
                var response = JsonSerializer.Deserialize<BitstampOrderBook>(content);
                if (response == null)
                    throw new Exception("Failed to deserialize order book JSON");

                return new OrderBook
                {
                    PrimaryCurrency = currencyFrom.ToUpper(),
                    SecondaryCurrency = currencyTo.ToUpper(),
                    Timestamp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(response.Timestamp)),
                    Bids = ParseOrderBookEntries(response.Bids),
                    Asks = ParseOrderBookEntries(response.Asks)
                };
            }
            catch (JsonException ex)
            {
                throw new Exception($"Bitstamp API returned invalid/unsupported JSON.", ex);
            }
        }

        private IEnumerable<OrderBookEntry> ParseOrderBookEntries(IEnumerable<string[]> entries)
        {
            return entries.Select(entry => new OrderBookEntry
            {
                Price = decimal.Parse(entry[0]),
                Amount = decimal.Parse(entry[1])
            }).ToList();
        }
    }
}
