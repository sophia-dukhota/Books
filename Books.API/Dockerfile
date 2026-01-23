FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
COPY . /src
RUN dotnet publish /src/Videl.Chatty.API/Videl.Chatty.API.csproj -o /app/publish --no-self-contained

FROM mcr.microsoft.com/dotnet/aspnet:9.0
COPY --from=build /app/publish /app
EXPOSE 8080 8081
WORKDIR /app
ENTRYPOINT ["dotnet", "/app/Videl.Chatty.API.dll"]