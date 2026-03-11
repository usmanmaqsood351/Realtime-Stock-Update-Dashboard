FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY InventoryDashboard.Api.sln ./
COPY src/InventoryDashboard.Domain/InventoryDashboard.Domain.csproj src/InventoryDashboard.Domain/
COPY src/InventoryDashboard.Application/InventoryDashboard.Application.csproj src/InventoryDashboard.Application/
COPY src/InventoryDashboard.Infrastructure/InventoryDashboard.Infrastructure.csproj src/InventoryDashboard.Infrastructure/
COPY src/InventoryDashboard.WebApi/InventoryDashboard.WebApi.csproj src/InventoryDashboard.WebApi/

RUN dotnet restore

COPY src/ src/
RUN dotnet publish src/InventoryDashboard.WebApi/InventoryDashboard.WebApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InventoryDashboard.WebApi.dll"]
