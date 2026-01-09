using Moq;
using Xunit;
using HiveHQ.Application.Services;
using HiveHQ.Domain.Interfaces;
using HiveHQ.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HiveHQ.Tests.Services;

public class ReportingTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly Mock<IGenericRepository<DailyReport>> _reportRepoMock;
    private readonly Mock<ILogger<ReportingService>> _loggerMock;
    private readonly ReportingService _service;

    public ReportingTests()
    {
        _orderRepoMock = new Mock<IOrderRepository>();
        _reportRepoMock = new Mock<IGenericRepository<DailyReport>>();
        _loggerMock = new Mock<ILogger<ReportingService>>();

        _service = new ReportingService(
            _orderRepoMock.Object,
            _reportRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GenerateEndOfDayReport_ShouldSaveCorrectRevenue()
    {
        // ARRANGE
        var expectedRevenue = 1500.00m;
        var expectedOrderCount = 3;

        // Mock the Order Repo to return our expected numbers
        _orderRepoMock.Setup(r => r.GetTotalRevenueAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(expectedRevenue);

        _orderRepoMock.Setup(r => r.CountAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Order, bool>>>()))
            .ReturnsAsync(expectedOrderCount);

        // ACT
        await _service.GenerateEndOfDayReport();

        // ASSERT
        // Verify that AddAsync was called with a report containing the correct revenue
        _reportRepoMock.Verify(r => r.AddAsync(It.Is<DailyReport>(report =>
                report.TotalRevenue == expectedRevenue &&
                report.TotalOrders == expectedOrderCount)),
            Times.Once);

        // Verify SaveChanges was actually called
        _reportRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
