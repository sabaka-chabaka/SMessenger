FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /app

COPY ["src/Services/AuthService/SMessenger.AuthService.API/SMessenger.AuthService.API.csproj", "src/Services/AuthService/SMessenger.AuthService.API/"]
COPY ["src/Services/AuthService/SMessenger.AuthService.Application/SMessenger.AuthService.Application.csproj", "src/Services/AuthService/SMessenger.AuthService.Application/"]
COPY ["src/Services/AuthService/SMessenger.AuthService.Domain/SMessenger.AuthService.Domain.csproj", "src/Services/AuthService/SMessenger.AuthService.Domain/"]
COPY ["src/Services/AuthService/SMessenger.AuthService.Infrastructure/SMessenger.AuthService.Infrastructure.csproj", "src/Services/AuthService/SMessenger.AuthService.Infrastructure/"]

RUN dotnet restore src/Services/AuthService/SMessenger.AuthService.API/SMessenger.AuthService.API.csproj

COPY . .
WORKDIR /app/src/Services/AuthService/SMessenger.AuthService.API

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SMessenger.AuthService.API.dll"]
