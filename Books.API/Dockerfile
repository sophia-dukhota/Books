# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy API project file
COPY Books.API/Books.API.csproj Books.API/

# Copy shared project file
COPY Books.Shared/Books.Shared.csproj Books.Shared/

# Restore dependencies
RUN dotnet restore "Books.API/Books.API.csproj"

# Copy the rest of the solution
COPY . .

# Build API
WORKDIR /src/Books.API
RUN dotnet build "Books.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Books.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Books.API.dll"]
