using BitStore.Bitstamp.Models;
using BitStore.Bitstamp.Services;
using BitStore.Core.Services;
using BitStore.Core.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BitStore.Core.Tests.Services;

/// <summary>
/// Tests covering key logic of <see cref="CoreService"/>.
/// </summary>
public class CoreServiceTests
{
    private readonly Mock<IBitstampService> _bitstampMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
    private readonly Mock<ILogger<CoreService>> _loggerMock;
    private readonly Mock<IDataService> _dataServiceMock;
    private readonly Mock<IServiceScope> _scopeMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly CoreService _sut;

    public CoreServiceTests()
    {
        _bitstampMock = new Mock<IBitstampService>();
        _configMock = new Mock<IConfiguration>();
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _loggerMock = new Mock<ILogger<CoreService>>();
        _dataServiceMock = new Mock<IDataService>();
        _scopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();

        // Configure mocks
        _configMock.Setup(x => x.SecondaryCurrency).Returns("EUR");
        
        // Set up the service provider to return our data service mock
        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IDataService)))
            .Returns(_dataServiceMock.Object);

        // Set up scope factory to return scope with our service provider
        _scopeMock
            .Setup(x => x.ServiceProvider)
            .Returns(_serviceProviderMock.Object);
        _scopeFactoryMock
            .Setup(x => x.CreateScope())
            .Returns(_scopeMock.Object);

        // Configure data service mock to accept any parameters
        _dataServiceMock
            .Setup(x => x.StoreSnapshotForUserAsync(
                It.IsAny<OrderBook>(),
                It.IsAny<Guid>(),
                It.IsAny<DateTimeOffset>()))
            .Returns(Task.CompletedTask);

        _sut = new CoreService(
            _bitstampMock.Object,
            _configMock.Object,
            _scopeFactoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task PollDataAsync_ShouldUpdateLatestOrderBook()
    {
        // Arrange
        var orderBook = TestDataHelper.CreateSampleOrderBook();
        _bitstampMock.Setup(x => x.GetOrderBookAsync("BTC", "EUR"))
            .ReturnsAsync(orderBook);

        // Act
        await _sut.PollDataAsync(CancellationToken.None);
        var result = await _sut.GetLatestOrderBookAsync(Guid.NewGuid().ToString());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderBook.PrimaryCurrency, result.PrimaryCurrency);
        Assert.Equal(orderBook.SecondaryCurrency, result.SecondaryCurrency);
        Assert.Equal(orderBook.Timestamp, result.Timestamp);
        Assert.Equal(orderBook.Bids.Count(), result.Bids.Count());
        Assert.Equal(orderBook.Asks.Count(), result.Asks.Count());
    }

    [Fact]
    public async Task GetLatestOrderBookAsync_WhenNoDataAvailable_ShouldPollFirst()
    {
        // Arrange
        var orderBook = TestDataHelper.CreateSampleOrderBook();
        _bitstampMock.Setup(x => x.GetOrderBookAsync("BTC", "EUR"))
            .ReturnsAsync(orderBook);

        // Act
        var result = await _sut.GetLatestOrderBookAsync(Guid.NewGuid().ToString());

        // Assert
        Assert.NotNull(result);
        _bitstampMock.Verify(x => x.GetOrderBookAsync("BTC", "EUR"), Times.Once);
    }

    [Fact]
    public async Task GetLatestOrderBookAsync_ShouldStoreSnapshot()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var orderBook = TestDataHelper.CreateSampleOrderBook();
        _bitstampMock.Setup(x => x.GetOrderBookAsync("BTC", "EUR"))
            .ReturnsAsync(orderBook);

        // Create ManualResetEventSlim to wait for the background task
        using var backgroundTaskCompleted = new ManualResetEventSlim(false);
        _dataServiceMock
            .Setup(x => x.StoreSnapshotForUserAsync(
                It.IsAny<OrderBook>(),
                It.IsAny<Guid>(),
                It.IsAny<DateTimeOffset>()))
            .Callback(() => backgroundTaskCompleted.Set())
            .Returns(Task.CompletedTask);

        // Act
        await _sut.PollDataAsync(CancellationToken.None);
        await _sut.GetLatestOrderBookAsync(userId.ToString());

        // Wait for background task to complete (with timeout)
        backgroundTaskCompleted.Wait(TimeSpan.FromSeconds(1));

        // Assert
        _dataServiceMock.Verify(x => x.StoreSnapshotForUserAsync(
            It.Is<OrderBook>(ob => 
                ob.PrimaryCurrency == orderBook.PrimaryCurrency &&
                ob.SecondaryCurrency == orderBook.SecondaryCurrency &&
                ob.Timestamp == orderBook.Timestamp &&
                ob.Bids.All(b => b.Price > 1000) // Verify price filter
            ),
            It.Is<Guid>(id => id == userId),
            It.IsAny<DateTimeOffset>()
        ), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetLatestOrderBookAsync_WithInvalidUserId_ShouldThrowException(string userId)
    {
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.GetLatestOrderBookAsync(userId));
    }
}