FROM ://microsoft.com AS build
WORKDIR /app

COPY ["src/Services/ChatService/SMessenger.ChatService.API/SMessenger.ChatService.API.csproj", "src/Services/ChatService/SMessenger.ChatService.API/"]
COPY ["src/Services/ChatService/SMessenger.ChatService.Application/SMessenger.ChatService.Application.csproj", "src/Services/ChatService/SMessenger.ChatService.Application/"]
COPY ["src/Services/ChatService/SMessenger.ChatService.Domain/SMessenger.ChatService.Domain.csproj", "src/Services/ChatService/SMessenger.ChatService.Domain/"]
COPY ["src/Services/ChatService/SMessenger.ChatService.Infrastructure/SMessenger.ChatService.Infrastructure.csproj", "src/Services/ChatService/SMessenger.ChatService.Infrastructure/"]

RUN dotnet restore src/Services/ChatService/SMessenger.ChatService.API/SMessenger.ChatService.API.csproj

COPY . .
WORKDIR /app/src/Services/ChatService/SMessenger.ChatService.API

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM ://microsoft.com AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SMessenger.ChatService.API.dll"]