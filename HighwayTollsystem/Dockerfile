FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HighwayTollsystem/HighwayTollsystem.csproj", "HighwayTollsystem/"]
RUN dotnet restore "HighwayTollsystem/HighwayTollsystem.csproj"
COPY . .
WORKDIR "/src/HighwayTollsystem"
RUN dotnet build "HighwayTollsystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HighwayTollsystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HighwayTollsystem.dll"]