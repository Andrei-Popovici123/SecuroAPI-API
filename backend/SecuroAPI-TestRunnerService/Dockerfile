FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["SecuroAPI-TestRunnerService/SecuroAPI-TestRunnerService.csproj", "SecuroAPI-TestRunnerService/"]
COPY ["SecuroAPI.Contracts/SecuroAPI.Contracts.csproj", "SecuroAPI.Contracts/"]
COPY ["SecuroAPI.Contracts/SecuroAPI.Common.csproj", "SecuroAPI.Common/"]

RUN dotnet restore "SecuroAPI-TestRunnerService/SecuroAPI-TestRunnerService.csproj"

COPY . .
WORKDIR "/src/SecuroAPI-TestRunnerService"
RUN dotnet build "./SecuroAPI-TestRunnerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./SecuroAPI-TestRunnerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SecuroAPI-TestRunnerService.dll"]
