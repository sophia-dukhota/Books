FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
COPY . /src
RUN dotnet publish /src/Books.API/Books.API.csproj -o /app/publish --no-self-contained

FROM mcr.microsoft.com/dotnet/aspnet:9.0
COPY --from=build /app/publish /app
EXPOSE 80
WORKDIR /app
ENTRYPOINT ["dotnet", "/app/Books.API.dll"]