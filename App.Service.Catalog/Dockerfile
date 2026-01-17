FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["App.Service.Catalog/App.Service.Catalog.csproj", "App.Service.Catalog/"]
COPY ["App.Modules.Catalog/App.Modules.Catalog.csproj", "App.Modules.Catalog/"]
COPY ["App.Shared.Protos/App.Shared.Protos.csproj", "App.Shared.Protos/"]
COPY ["App.Shared/App.Shared.csproj", "App.Shared/"]
RUN dotnet restore "./App.Service.Catalog/App.Service.Catalog.csproj"
COPY . .
WORKDIR "/src/App.Service.Catalog"
RUN dotnet build "./App.Service.Catalog.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./App.Service.Catalog.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.Service.Catalog.dll"]
