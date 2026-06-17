FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /app

COPY src/Gateways/SMessenger.Gateway/*.csproj src/Gateways/SMessenger.Gateway/

RUN dotnet restore src/Gateways/SMessenger.Gateway/SMessenger.Gateway.csproj

COPY . .
WORKDIR /app/src/Gateway/SMessenger.Gateway
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SMessenger.Gateway.dll"]