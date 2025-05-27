using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using TVMazeScraper.Data;
using TVMazeScraper.Models;

namespace TVMazeScraper.Services.Tests;

public class SqliteShowServiceTests
{
    private readonly ILogger<SqliteShowService> _dummyLogger = new Mock<ILogger<SqliteShowService>>().Object;

    public SqliteShowService SqliteShowServiceFactory(ProgramContext programContext, ILogger<SqliteShowService>? logger = null)
    {
        return new SqliteShowService(programContext, logger == null ? _dummyLogger : logger);
    }

    [Fact]
    public async Task GetLastUpdatedValue_ShouldReturnZero_IfEmpty()
    {
        //Arrange
        var data = new List<Show>().AsQueryable();
        var mockContext = new Mock<ProgramContext>();
        mockContext.Setup(c => c.Shows).ReturnsDbSet(data);
        var service = SqliteShowServiceFactory(mockContext.Object);

        //Act
        var value = await service.GetLastUpdatedValue();

        //Assert
        Assert.Equal(0, value);
    }

    [Fact]
    public async Task GetLastUpdatedValue_ShouldReturnHighest_IfOneShowExists()
    {
        var updated = 200;
        var show = new Show()
        {
            Id = 1,
            LastUpdated = updated
        };
        var data = new List<Show>().Append(show);
        var mockContext = new Mock<ProgramContext>();
        mockContext.Setup(c => c.Shows).ReturnsDbSet(data);
        var service = SqliteShowServiceFactory(mockContext.Object);

        //Act
        var value = await service.GetLastUpdatedValue();

        Assert.Equal(updated, value);
    }

    [Fact]
    public async Task GetLastUpdatedValue_ShouldReturnHighest_IfMultipleShowsExists()
    {
        var updated = 200;
        var show1 = new Show()
        {
            LastUpdated = updated
        };
        updated++;
        var show2 = new Show()
        {
            LastUpdated = updated
        };
        var data = new List<Show> { show1, show2 };
        var mockContext = new Mock<ProgramContext>();
        mockContext.Setup(c => c.Shows).ReturnsDbSet(data);
        var service = SqliteShowServiceFactory(mockContext.Object);

        //Act
        var value = await service.GetLastUpdatedValue();

        Assert.Equal(updated, value);
    }


    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_IfNothingExists()
    {
        //Arrange
        var data = new List<Show>().AsQueryable();
        var mockContext = new Mock<ProgramContext>();
        mockContext.Setup(c => c.Shows).ReturnsDbSet(data);
        var service = SqliteShowServiceFactory(mockContext.Object);

        //Act
        var value = await service.GetAllAsync();

        //Assert
        Assert.Empty(value);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnList_IfShowsExists()
    {
        //Arrange
        var show1 = new Show()
        {
            Id = 1
        };
        var show2 = new Show()
        {
            Id = 2
        };
        var data = new List<Show> { show1, show2 };
        var mockContext = new Mock<ProgramContext>();
        mockContext.Setup(c => c.Shows).ReturnsDbSet(data);
        var service = SqliteShowServiceFactory(mockContext.Object);

        //Act
        var value = await service.GetAllAsync();

        //Assert
        Assert.NotEmpty(value);
        Assert.Equal(2, value.Count);
    }

    [Fact]
    public async Task GetPageAsync_ShouldReturnStartList_IfPassedZeroPage()
    {
        //Arrange
        var show1 = new Show()
        {
            Id = 1
        };
        var show2 = new Show()
        {
            Id = 2
        };
        var show3 = new Show()
        {
            Id = 3
        };
        var data = new List<Show> { show1, show2, show3 };
        var mockContext = new Mock<ProgramContext>();
        mockContext.Setup(c => c.Shows).ReturnsDbSet(data);
        var service = SqliteShowServiceFactory(mockContext.Object);

        //Act
        var value = await service.GetPageAsync(0, 1);

        //Assert
        Assert.Single(value);
        Assert.Equal(show1, value[0]);
    }

    [Fact]
    public async Task GetPageAsync_ShouldReturnMiddleList_IfPassedFirstPage()
    {
        //Arrange
        var show1 = new Show()
        {
            Id = 1
        };
        var show2 = new Show()
        {
            Id = 2
        };
        var show3 = new Show()
        {
            Id = 3
        };
        var data = new List<Show> { show1, show2, show3 };
        var mockContext = new Mock<ProgramContext>();
        mockContext.Setup(c => c.Shows).ReturnsDbSet(data);
        var service = SqliteShowServiceFactory(mockContext.Object);

        //Act
        var value = await service.GetPageAsync(1, 1);

        //Assert
        Assert.Single(value);
        Assert.Equal(show2, value[0]);
    }

    [Fact]
    public async Task GetPageAsync_ShouldReturnLargerList_IfPassedPageSize()
    {
        //Arrange
        var show1 = new Show()
        {
            Id = 1
        };
        var show2 = new Show()
        {
            Id = 2
        };
        var show3 = new Show()
        {
            Id = 3
        };
        var data = new List<Show> { show1, show2, show3 };
        var mockContext = new Mock<ProgramContext>();
        mockContext.Setup(c => c.Shows).ReturnsDbSet(data);
        var service = SqliteShowServiceFactory(mockContext.Object);

        //Act
        var value = await service.GetPageAsync(0, 2);

        //Assert
        Assert.Equal(2, value.Count);
        Assert.Equal(show1, value[0]);
        Assert.Equal(show2, value[1]);
    }
}


