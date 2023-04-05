# Leaderboards

Leaderboards API for my personal projects (games).

The primary usage are Unity games. You can use it for your own projects. 
To do that, you should use API project and build you own client library. 
The base class is LeaderboardsClientBase, there are some methods to 
implement manually. Also do not forget to configure auth info on the
service side (the `Auth` section in appsettings.json).

To run locally, copy `appsettings.json` to `appsettings.Development.json`
and edit the connection string to connect to PostgreSQL database.

To run tests, crate environment variable named `TEST_DB_CONNECTION_STRING` with
appropriate connection string value.

PR's are welcome.

Licensed under MIT.
