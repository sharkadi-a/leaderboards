FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["/", "/"]
RUN ls ./
RUN dotnet restore "AndreyGames.Leaderboards.Service/AndreyGames.Leaderboards.Service.csproj"
COPY . .
WORKDIR "/src/AndreyGames.Leaderboards.Service"
RUN ls ./
RUN dotnet build "AndreyGames.Leaderboards.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AndreyGames.Leaderboards.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AndreyGames.Leaderboards.Service.dll"]
