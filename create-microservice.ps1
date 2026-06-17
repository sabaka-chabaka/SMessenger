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
rm "$ServicePath/SMessenger.$ServiceName.API/$ServiceName.http" -Force
rm "$ServicePath/SMessenger.$ServiceName.Application/Class1.cs" -Force
rm "$ServicePath/SMessenger.$ServiceName.Domain/Class1.cs" -Force
rm "$ServicePath/SMessenger.$ServiceName.Infrastructure/Class1.cs" -Force
Set-Content -Path "$ServicePath/SMessenger.$ServiceName.API/Program.cs" -Value @'
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.Run();
'@


Write-Host "Service $ServiceName successfully created!" -ForegroundColor Green