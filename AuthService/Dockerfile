# Stage 0: Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier les csproj pour restore
COPY ["AuthService/AuthService.csproj", "AuthService/"]
COPY ["DineroBank.Shared/DineroBank.Shared.csproj", "DineroBank.Shared/"]

WORKDIR /src/AuthService
RUN dotnet restore "AuthService.csproj"

# Copier le code source nécessaire
COPY ["AuthService/", "AuthService/"]
COPY ["DineroBank.Shared/", "DineroBank.Shared/"]

RUN dotnet build "AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 2: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 3: Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.dll"]
