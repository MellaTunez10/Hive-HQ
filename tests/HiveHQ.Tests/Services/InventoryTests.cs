using Moq;
using Xunit;
using HiveHQ.Application.Services;
using HiveHQ.Domain.Interfaces;
using HiveHQ.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HiveHQ.Tests.Services;

public class InventoryTests
{
    private readonly Mock<IGenericRepository<InventoryItem>> _inventoryRepoMock;
    private readonly Mock<ILogger<InventoryAlertService>> _loggerMock;
    private readonly InventoryAlertService _service;

    public InventoryTests()
    {
        // We "Mock" the dependencies so we don't need a real database to run tests
        _inventoryRepoMock = new Mock<IGenericRepository<InventoryItem>>();
        _loggerMock = new Mock<ILogger<InventoryAlertService>>();

        _service = new InventoryAlertService(_inventoryRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CheckStockLevels_ShouldLogWarning_WhenStockIsLow()
    {
        // ARRANGE: Setup a fake item that is low on stock
        var lowStockItem = new InventoryItem
        {
            Name = "Hive Desk",
            QuantityInStock = 1,
            ReorderLevel = 5
        };

        _inventoryRepoMock.Setup(repo => repo.GetListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<InventoryItem, bool>>>()))
            .ReturnsAsync(new List<InventoryItem> { lowStockItem });

        // ACT: Run the service logic
        await _service.CheckStockLevels();

        // ASSERT: Verify the logger was called (meaning the alert triggered)
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("low on stock")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
