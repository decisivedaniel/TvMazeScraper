# TvMazeScraper

[![.NET](https://github.com/decisivedaniel/TvMazeScraper/actions/workflows/dotnet.yml/badge.svg)](https://github.com/decisivedaniel/TvMazeScraper/actions/workflows/dotnet.yml)

Started creating the controller based TVMaze API with the built in template given by dotnet. Some consideration was given to creating a minimal API as that would be more suited to respond better to event driven development as simple lambda functions. However, with the needed functionality of scraping the TVMaze database felt the original controller based approach would give me the most options before refactoring at a later time to a more scaliable version.

Current database is driven by Sqlite which will not make for a long term solution as it doesn't scale very well to large datasets. Works for local developement and services can quickly be switched later in development. Refactoring of the solution has been made to enable the separation of testing code from the actual API code (safety reasons) with the process happening through the cli.

Testing of the DbSets was initially ran with scaffolding from [](https://learn.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking); however, a much easier way to mock DbContexts was found from the [Moq.EntityFrameworkCore](https://github.com/MichalJankowskii/Moq.EntityFrameworkCore). This allow much shorter Arrange sections and more focused unit testing.


