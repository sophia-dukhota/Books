FROM mcr.microsoft.com/dotnet/sdk:9.0 AS blazor_build
COPY . /src
RUN dotnet publish /src/Books.Frontend/Books.Frontend.csproj -o /app/publish

FROM nginx:1.29.4-alpine3.23
COPY --from=blazor_build /app/publish/wwwroot /var/www/html
COPY --from=blazor_build /src/Books.Frontend/nginx.conf /etc/nginx/nginx.conf

EXPOSE 9090