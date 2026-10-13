FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source
COPY backend/FactoryPulse.Api/FactoryPulse.Api.csproj backend/FactoryPulse.Api/
RUN dotnet restore backend/FactoryPulse.Api/FactoryPulse.Api.csproj
COPY backend/FactoryPulse.Api/ backend/FactoryPulse.Api/
RUN dotnet publish backend/FactoryPulse.Api/FactoryPulse.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /app/data && chown -R $APP_UID:$APP_UID /app/data
ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__FactoryPulse="Data Source=/app/data/factorypulse.db"
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "FactoryPulse.Api.dll"]
