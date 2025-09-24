using BitStore.Bitstamp.Models;
using BitStore.Core.Services;
using BitStore.Core.Tests.Helpers;
using BitStore.Data.Entities;
using BitStore.Data.Repository;
using Moq;
using Xunit;

namespace BitStore.Core.Tests.Services;

/// <summary>
/// Tests covering key logic of <see cref="DataService"/>.
/// </summary>
public class DataServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAuditLogRepository> _auditLogRepositoryMock;
    private readonly DataService _sut;

    public DataServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _auditLogRepositoryMock = new Mock<IAuditLogRepository>();
        _sut = new DataService(_auditLogRepositoryMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldCreateNewUser()
    {
        // Arrange
        var login = "testUser";
        User? createdUser = null;
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .Callback<User>(u => createdUser = u)
            .ReturnsAsync((User u) => u);

        // Act
        var result = await _sut.CreateUserAsync(login);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(login, result.Login);
        Assert.NotEqual(Guid.Empty, result.Id);
        _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserTokenAsync_WhenUserExists_ShouldUpdateToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Login = "testUser" };
        var newToken = "newToken";

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        await _sut.UpdateUserTokenAsync(userId, newToken);

        // Assert
        Assert.Equal(newToken, user.Token);
        _userRepositoryMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateUserTokenAsync_WhenUserNotFound_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _sut.UpdateUserTokenAsync(userId, "newToken"));
    }

    [Fact]
    public async Task StoreSnapshotForUserAsync_WhenUserExists_ShouldCreateAuditLog()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requestedAt = DateTimeOffset.UtcNow;
        var orderBook = TestDataHelper.CreateSampleOrderBook();

        _userRepositoryMock.Setup(x => x.ExistsAsync(userId))
            .ReturnsAsync(true);

        AuditLog? capturedAuditLog = null;
        _auditLogRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<AuditLog>()))
            .Callback<AuditLog>(a => capturedAuditLog = a)
            .ReturnsAsync((AuditLog a) => a);

        // Act
        await _sut.StoreSnapshotForUserAsync(orderBook, userId, requestedAt);

        // Assert
        Assert.NotNull(capturedAuditLog);
        Assert.Equal(userId, capturedAuditLog.UserId);
        Assert.Equal(requestedAt, capturedAuditLog.Timestamp);
        Assert.Contains(orderBook.PrimaryCurrency, capturedAuditLog.Data);
        Assert.Contains(orderBook.SecondaryCurrency, capturedAuditLog.Data);
    }

    [Fact]
    public async Task StoreSnapshotForUserAsync_WhenUserNotFound_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(x => x.ExistsAsync(userId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _sut.StoreSnapshotForUserAsync(TestDataHelper.CreateSampleOrderBook(), userId, DateTimeOffset.UtcNow));
    }
}