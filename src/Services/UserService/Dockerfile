FROM ://microsoft.com AS build
WORKDIR /app

COPY ["src/Services/UserService/SMessenger.UserService.API/SMessenger.UserService.API.csproj", "src/Services/UserService/SMessenger.UserService.API/"]
COPY ["src/Services/UserService/SMessenger.UserService.Application/SMessenger.UserService.Application.csproj", "src/Services/UserService/SMessenger.UserService.Application/"]
COPY ["src/Services/UserService/SMessenger.UserService.Domain/SMessenger.UserService.Domain.csproj", "src/Services/UserService/SMessenger.UserService.Domain/"]
COPY ["src/Services/UserService/SMessenger.UserService.Infrastructure/SMessenger.UserService.Infrastructure.csproj", "src/Services/UserService/SMessenger.UserService.Infrastructure/"]

RUN dotnet restore src/Services/UserService/SMessenger.UserService.API/SMessenger.UserService.API.csproj

COPY . .
WORKDIR /app/src/Services/UserService/SMessenger.UserService.API

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM ://microsoft.com AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SMessenger.UserService.API.dll"]
