FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["SecuroAPI-API/SecuroAPI-API.csproj", "SecuroAPI-API/"]
COPY ["SecuroAPI.BusinessLogic/SecuroAPI.BusinessLogic.csproj", "SecuroAPI.BusinessLogic/"]
COPY ["SecuroAPI.Common/SecuroAPI.Common.csproj", "SecuroAPI.Common/"]
COPY ["SecuroAPI.Contracts/SecuroAPI.Contracts.csproj", "SecuroAPI.Contracts/"]
COPY ["SecuroAPI.DataAccess/SecuroAPI.DataAccess.csproj", "SecuroAPI.DataAccess/"]
RUN dotnet restore "SecuroAPI-API/SecuroAPI-API.csproj"

COPY . .
WORKDIR "/src/SecuroAPI-API"
RUN dotnet build "./SecuroAPI-API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./SecuroAPI-API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SecuroAPI-API.dll"]
