FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["global.json", "."]
COPY ["DotnetManager/DotnetManager.csproj", "DotnetManager/"]
COPY ["DotnetManager.Core/DotnetManager.Core.csproj", "DotnetManager.Core/"]
COPY ["DotnetManager.Installation/DotnetManager.Installation.csproj", "DotnetManager.Installation/"]
COPY ["DotnetManager.InstalledDotnet/DotnetManager.InstalledDotnet.csproj", "DotnetManager.InstalledDotnet/"]
COPY ["DotnetManager.ReleaseMetadata/DotnetManager.ReleaseMetadata.csproj", "DotnetManager.ReleaseMetadata/"]
COPY ["DotnetManager.Removal/DotnetManager.Removal.csproj", "DotnetManager.Removal/"]
COPY ["DotnetManager.UserEnvironment/DotnetManager.UserEnvironment.csproj", "DotnetManager.UserEnvironment/"]
RUN dotnet restore "DotnetManager/DotnetManager.csproj"
COPY . .
WORKDIR "/src/DotnetManager"
RUN dotnet build "./DotnetManager.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DotnetManager.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotnetManager.dll"]
