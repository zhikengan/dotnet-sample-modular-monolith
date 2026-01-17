FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["App.Web/App.Web.csproj", "App.Web/"]
COPY ["App.Shared/App.Shared.csproj", "App.Shared/"]
COPY ["App.Shared.Protos/App.Shared.Protos.csproj", "App.Shared.Protos/"]
COPY ["App.Modules.User/App.Modules.User.csproj", "App.Modules.User/"]
COPY ["App.Modules.Ordering/App.Modules.Ordering.csproj", "App.Modules.Ordering/"]
COPY ["App.Modules.Notification/App.Modules.Notification.csproj", "App.Modules.Notification/"]
COPY ["App.Portal.Admin/App.Portal.Admin.csproj", "App.Portal.Admin/"]
# Catalog is remote now, but referenced via Protos.
RUN dotnet restore "./App.Web/App.Web.csproj"
COPY . .
WORKDIR "/src/App.Web"
RUN dotnet build "./App.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./App.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.Web.dll"]
