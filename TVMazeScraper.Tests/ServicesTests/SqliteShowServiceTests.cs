using Moq;
using Moq.EntityFrameworkCore;
using TVMazeScraper.Data;
using TVMazeScraper.Models;

namespace TVMazeScraper.Services.Tests;

public class SqliteShowServiceTests
{
    [Fact]
    public async Task GetLastUpdatedValue_ShouldReturnZero_IfEmpty()
    {
        //Arrange
        var data = new List<Show>().AsQueryable();
        var mockContext = new Mock<ProgramContext>();
        mockContext.Setup(c => c.Shows).ReturnsDbSet(data);
        var service = new SqliteShowService(mockContext.Object);

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
        var service = new SqliteShowService(mockContext.Object);

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
        var service = new SqliteShowService(mockContext.Object);

        //Act
        var value = await service.GetLastUpdatedValue();

        Assert.Equal(updated, value);
    }
}

