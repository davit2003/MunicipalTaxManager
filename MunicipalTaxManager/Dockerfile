FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MunicipalTaxManager/MunicipalTaxManager.csproj", "MunicipalTaxManager/"]
RUN dotnet restore "./MunicipalTaxManager/MunicipalTaxManager.csproj"
COPY . .
WORKDIR "/src/MunicipalTaxManager"
RUN dotnet build "./MunicipalTaxManager.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MunicipalTaxManager.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MunicipalTaxManager.dll"]