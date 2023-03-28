FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS tests
WORKDIR /src
COPY ["/src", "/src"]
RUN echo $USERNAME
RUN dotnet test --verbosity normal
RUN dotnet restore "AndreyGames.Leaderboards.Service/AndreyGames.Leaderboards.Service.csproj"

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
COPY . .
WORKDIR "/src/AndreyGames.Leaderboards.Service"
RUN dotnet build "AndreyGames.Leaderboards.Service.csproj" -c Release -o /app/build

FROM build AS publish
ARG NAMESPACE
ARG NUGET_KEY
RUN dotnet publish "AndreyGames.Leaderboards.Service.csproj" -c Release -o /app/publish
RUN dotnet pack "../AndreyGames.Leaderboards.Tests/AndreyGames.Leaderboards.Tests.csproj" --configuration Release
RUN dotnet nuget push "../AndreyGames.Leaderboards.API/bin/Release/*.nupkg" --source https://nuget.pkg.github.com/${NAMESPACE}/index.json --api-key $NUGET_KEY --skip-duplicate

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AndreyGames.Leaderboards.Service.dll"]
