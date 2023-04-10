# Leaderboards API and Service

This is a repository for the Leaderboards API for my personal projects (games). 
The source code is provided AS IS, so use it for your own good (or bad, hehe).

The primary usage are Unity games. You can use it for your own projects. 
To do that, you should use API project and build you own client library. 
The base class is LeaderboardsClientBase, there are some methods you should  
implement manually (because HttpClient is not portable is Unity). 

In the configuration file, you have to configure two options:
- `Auth` section for the authorization
- `ConnectionStrings.Default` should contain a valid PostgreSQL connection
string

To run locally, copy `appsettings.json` to `appsettings.Development.json`.

To run tests, crate environment variable named `TEST_DB_CONNECTION_STRING` with
appropriate connection string value (the `Auth` section is configured for tests
automatically for you).

PR's are welcome.

Licensed under MIT.
