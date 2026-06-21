[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$ServiceName
)

$ServicePath = "src/Services/$ServiceName"
$ApiProj = "$ServicePath/SMessenger.$ServiceName.API/SMessenger.$ServiceName.API.csproj"
$AppProj = "$ServicePath/SMessenger.$ServiceName.Application/SMessenger.$ServiceName.Application.csproj"
$DomProj = "$ServicePath/SMessenger.$ServiceName.Domain/SMessenger.$ServiceName.Domain.csproj"
$InfProj = "$ServicePath/SMessenger.$ServiceName.Infrastructure/SMessenger.$ServiceName.Infrastructure.csproj"

Write-Host "Creating projects for service: $ServiceName..." -ForegroundColor Cyan
mkdir $ServicePath -Force | Out-Null

dotnet new webapi -n "SMessenger.$ServiceName.API" -o "$ServicePath/SMessenger.$ServiceName.API"
dotnet new classlib -n "SMessenger.$ServiceName.Application" -o "$ServicePath/SMessenger.$ServiceName.Application"
dotnet new classlib -n "SMessenger.$ServiceName.Domain" -o "$ServicePath/SMessenger.$ServiceName.Domain"
dotnet new classlib -n "SMessenger.$ServiceName.Infrastructure" -o "$ServicePath/SMessenger.$ServiceName.Infrastructure"

Write-Host "Configuring Clean Architecture dependencies..." -ForegroundColor Cyan
dotnet add $AppProj reference $DomProj
dotnet add $InfProj reference $AppProj
dotnet add $ApiProj reference $AppProj
dotnet add $ApiProj reference $InfProj

Write-Host "Adding projects to Solution..." -ForegroundColor Cyan
dotnet sln add $ApiProj $AppProj $DomProj $InfProj

Write-Host "Clear projects..." -ForegroundColor Cyan
rm "$ServicePath/SMessenger.$ServiceName.API/SMessenger.$ServiceName.API.http" -Force
rm "$ServicePath/SMessenger.$ServiceName.Application/Class1.cs" -Force
rm "$ServicePath/SMessenger.$ServiceName.Domain/Class1.cs" -Force
rm "$ServicePath/SMessenger.$ServiceName.Infrastructure/Class1.cs" -Force
Set-Content -Path "$ServicePath/SMessenger.$ServiceName.API/Program.cs" -Value @'
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.Run();
'@

Write-Host "Generating Alpine Dockerfile inside service folder..." -ForegroundColor Cyan
$DockerfilePath = "$ServicePath/Dockerfile" 
$DockerfileContent = @"
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /app

COPY ["src/Services/$ServiceName/SMessenger.$ServiceName.API/SMessenger.$ServiceName.API.csproj", "src/Services/$ServiceName/SMessenger.$ServiceName.API/"]
COPY ["src/Services/$ServiceName/SMessenger.$ServiceName.Application/SMessenger.$ServiceName.Application.csproj", "src/Services/$ServiceName/SMessenger.$ServiceName.Application/"]
COPY ["src/Services/$ServiceName/SMessenger.$ServiceName.Domain/SMessenger.$ServiceName.Domain.csproj", "src/Services/$ServiceName/SMessenger.$ServiceName.Domain/"]
COPY ["src/Services/$ServiceName/SMessenger.$ServiceName.Infrastructure/SMessenger.$ServiceName.Infrastructure.csproj", "src/Services/$ServiceName/SMessenger.$ServiceName.Infrastructure/"]

RUN dotnet restore src/Services/$ServiceName/SMessenger.$ServiceName.API/SMessenger.$ServiceName.API.csproj

COPY . .
WORKDIR /app/src/Services/$ServiceName/SMessenger.$ServiceName.API

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SMessenger.$ServiceName.API.dll"]
"@

Set-Content -Path $DockerfilePath -Value $DockerfileContent


Write-Host "Service $ServiceName successfully created!" -ForegroundColor Green