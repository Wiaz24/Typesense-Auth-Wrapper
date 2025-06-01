FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["TypesenseAuthWrapper.Bootstrapper/TypesenseAuthWrapper.Bootstrapper.csproj", "TypesenseAuthWrapper.Bootstrapper/"]
COPY ["TypesenseAuthWrapper.Shared/TypesenseAuthWrapper.Shared.csproj", "TypesenseAuthWrapper.Shared/"]
RUN dotnet restore "TypesenseAuthWrapper.Bootstrapper/TypesenseAuthWrapper.Bootstrapper.csproj"
COPY . .
WORKDIR "/src/TypesenseAuthWrapper.Bootstrapper"
RUN dotnet build "TypesenseAuthWrapper.Bootstrapper.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TypesenseAuthWrapper.Bootstrapper.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TypesenseAuthWrapper.Bootstrapper.dll"]
