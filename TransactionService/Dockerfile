# Stage 0: Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5002

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier les csproj pour restore
COPY ["TransactionService/TransactionService.csproj", "TransactionService/"]
COPY ["DineroBank.Shared/DineroBank.Shared.csproj", "DineroBank.Shared/"]

WORKDIR /src/TransactionService
RUN dotnet restore "TransactionService.csproj"

# Copier le code source nécessaire
COPY ["TransactionService/", "TransactionService/"]
COPY ["DineroBank.Shared/", "DineroBank.Shared/"]

RUN dotnet build "TransactionService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 2: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TransactionService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 3: Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransactionService.dll"]
