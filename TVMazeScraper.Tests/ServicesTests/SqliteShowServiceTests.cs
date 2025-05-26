using TVMazeScraper.Services;
using TVMazeScraper.Data;
using Microsoft.EntityFrameworkCore;

namespace TVMazeScraper.Services.Tests;

public class SqliteShowServiceTests
{
    [Fact]
    public void GetLastUpdatedValue_ShouldReturnNull_IfEmpty()
    {
        //Arrange
        var context = new ProgramContext(new DbContextOptionsBuilder<ProgramContext>().Options);
        var service = new SqliteShowService(context);

        //Act
        var data = service.GetLastUpdatedValue();

        //Assert
        Assert.Null(data);
    }

    [Fact]
    public void GetLastUpdatedValue_ShouldReturnHighest_IfValueExists()
    {
        
    }
}

