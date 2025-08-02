FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AvaloniaSimpleChat.Server/AvaloniaSimpleChat.Server.csproj", "AvaloniaSimpleChat.Server/"]
RUN dotnet restore "AvaloniaSimpleChat.Server/AvaloniaSimpleChat.Server.csproj"
COPY . .
WORKDIR "/src/AvaloniaSimpleChat.Server"
RUN dotnet build "./AvaloniaSimpleChat.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AvaloniaSimpleChat.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AvaloniaSimpleChat.Server.dll"]
