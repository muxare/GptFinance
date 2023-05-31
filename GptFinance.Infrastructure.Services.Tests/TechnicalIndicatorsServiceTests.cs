using GptFinance.Application.Interfaces;
using GptFinance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GptFinance.Infrastructure.Services.Tests;

public class TechnicalIndicatorsServiceTests
{
    private readonly TechnicalIndicatorsService _service;
    private readonly Mock<AppDbContext> _mockContext;
    private readonly Mock<IEodDataRepository> _mockEodDataRepository;

    public TechnicalIndicatorsServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase") // Use InMemory database for testing
            .Options;
        _mockContext = new Mock<AppDbContext>(options);
        _mockEodDataRepository = new Mock<IEodDataRepository>();

        _service = new TechnicalIndicatorsService(_mockContext.Object, _mockEodDataRepository.Object);
    }

    [Fact]
    public void ImputeMissingData_Should_ThrowException_If_ContextIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new TechnicalIndicatorsService(null, _mockEodDataRepository.Object));
    }

    [Fact]
    public void ImputeMissingData_Should_ThrowException_If_EodDataRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new TechnicalIndicatorsService(_mockContext.Object, null));
    }

    [Fact]
    public void ImputeMissingData_Should_Correctly_Impute_Missing_Data_With_Mean()
    {
        var priceData = new Dictionary<DateTime, decimal?>
        {
            { new DateTime(2023, 5, 20), 100 },
            { new DateTime(2023, 5, 21), null },
            { new DateTime(2023, 5, 22), 200 }
        };

        var expected = new Dictionary<DateTime, decimal>
        {
            { new DateTime(2023, 5, 20), 100 },
            { new DateTime(2023, 5, 21), 150 }, // Mean of 100 and 200
            { new DateTime(2023, 5, 22), 200 }
        };

        var result = _service.ImputeMissingData(priceData, TechnicalIndicatorsService.ImputationMethod.Mean);

        Assert.Equal(expected, result);
    }

    // Add more unit tests for other methods and scenarios
    // Test case 1: Price data with only one point

    // Test case 2: Price data with multiple points and period = 1
    [Fact]
    public void TestCalculateEMA_Period1()
    {
        var priceData = new Dictionary<DateTime, decimal>
        {
            { new DateTime(2022, 1, 1), 10.0M },
            { new DateTime(2022, 1, 2), 11.0M },
            { new DateTime(2022, 1, 3), 12.0M },
            { new DateTime(2022, 1, 4), 13.0M },
            { new DateTime(2022, 1, 5), 14.0M }
        };
        var ema = _service.CalculateEMA(priceData, 1);
        Assert.Equal(ema[new DateTime(2022, 1, 1)], 10.0M); // EMA is same as data for the first point
        Assert.Equal(ema[new DateTime(2022, 1, 2)], 11.0M);
        Assert.Equal(ema[new DateTime(2022, 1, 3)], 12.0M);
        Assert.Equal(ema[new DateTime(2022, 1, 4)], 13.0M);
        Assert.Equal(ema[new DateTime(2022, 1, 5)], 14.0M);
    }

    // Test case 4: Price data not sorted in ascending order
    [Fact]
    public void TestCalculateEMA_UnsortedData()
    {
        var priceData = new Dictionary<DateTime, decimal>
        {
            { new DateTime(2022, 1, 1), 10.0M },
            { new DateTime(2022, 1, 5), 14.0M },
            { new DateTime(2022, 1, 4), 13.0M },
            { new DateTime(2022, 1, 3), 12.0M },
            { new DateTime(2022, 1, 2), 11.0M }
        };
        Assert.Throws<ArgumentException>(() =>
        {
            _service.CalculateEMA(priceData, 10);
        });
    }
}