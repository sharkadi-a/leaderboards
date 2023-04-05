# Leaderboards

Leaderboards API for my personal projects (games).

The primary usage are Unity games. You can use it for your own projects. 
To do that you should use API project and build you own library. The base
class is LeaderboardsClientBase, there are some methods to implement manually. 
Also do not forget to configure crypto keys.

To run locally (or to run tests) copy appsettings.json to appsettings.Development.json
and edit the connection string to accessible PostgreSQL database.

PR's are welcome.

Licensed under MIT.
