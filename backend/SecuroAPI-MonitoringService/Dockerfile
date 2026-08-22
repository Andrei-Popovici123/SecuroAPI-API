FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["SecuroAPI-MonitoringService/SecuroAPI-MonitoringService.csproj", "SecuroAPI-MonitoringService/"]
COPY ["SecuroAPI.Contracts/SecuroAPI.Contracts.csproj", "SecuroAPI.Contracts/"]

RUN dotnet restore "SecuroAPI-MonitoringService/SecuroAPI-MonitoringService.csproj"

COPY . .
WORKDIR "/src/SecuroAPI-MonitoringService"
RUN dotnet build "./SecuroAPI-MonitoringService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./SecuroAPI-MonitoringService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SecuroAPI-MonitoringService.dll"]
