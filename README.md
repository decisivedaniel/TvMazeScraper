# TvMazeScraper

[![.NET](https://github.com/decisivedaniel/TvMazeScraper/actions/workflows/dotnet.yml/badge.svg)](https://github.com/decisivedaniel/TvMazeScraper/actions/workflows/dotnet.yml)

# Run Instructions

To the best of my knowledge on a new clone the following instructions should make it operational
1. `dotnet restore`
2. `cd MVMazeScraper`
3. `dotnet ef database update`
4. `dotnet run`

# Things Learned

Started creating the controller based TVMaze API with the built in template given by dotnet. Some consideration was given to creating a minimal API as that would be more suited to respond better to event driven development as simple lambda functions. However, with the needed functionality of scraping the TVMaze database felt the original controller based approach would give me the most options before refactoring at a later time to a more scaliable version.

Current database is driven by Sqlite which will not make for a long term solution as it doesn't scale very well to large datasets. Works for local developement and services can quickly be switched later in development. Refactoring of the solution has been made to enable the separation of testing code from the actual API code (safety reasons) with the process happening through the cli.

Testing of the DbSets was initially ran with scaffolding from [](https://learn.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking); however, a much easier way to mock DbContexts was found from the [Moq.EntityFrameworkCore](https://github.com/MichalJankowskii/Moq.EntityFrameworkCore). This allow much shorter Arrange sections and more focused unit testing.

Running the scraper service as a HostedService was new for me but me et the requirements I needed for this project for the scraper to start at runtime and include a timer that continues to build the database as time progresses. Some errors can occur with the current setup if the hosted service gets interrupted during creation which can be fixed in future with better sorting of updated show entries.

Parsing of Json can be tricky with these large API services as they offer many nested objects. Current build used an automatic tool to convert an example response (in my case Game of Thrones show) then create the objects for the Json parser to add into. More testing would need to be done to make sure all edge cases are covered by this current. Other methods could be directly using JsonDOM manipulation; however, this creates many hardcoded strings that relates to the json object which is less than preferred.
