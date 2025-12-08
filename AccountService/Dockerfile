# Stage 0: Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5001

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier seulement les csproj pour le restore
COPY ["AccountService/AccountService.csproj", "AccountService/"]
COPY ["DineroBank.Shared/DineroBank.Shared.csproj", "DineroBank.Shared/"]

RUN dotnet restore "AccountService/AccountService.csproj"

# Copier tout le code source ensuite
COPY AccountService/ AccountService/
COPY DineroBank.Shared/ DineroBank.Shared/

WORKDIR /src/AccountService
RUN dotnet build "AccountService.csproj" -c $BUILD_CONFIGURATION -o /src/build

# Stage 2: Publish
FROM build AS publish
WORKDIR /src/AccountService
RUN dotnet publish "AccountService.csproj" -c $BUILD_CONFIGURATION -o /src/publish /p:UseAppHost=false

# Stage 3: Final
FROM base AS final
WORKDIR /app
COPY --from=publish /src/publish .
ENTRYPOINT ["dotnet", "AccountService.dll"]
