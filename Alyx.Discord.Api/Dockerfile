FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Alyx.Discord.Api/Alyx.Discord.Api.csproj", "Alyx.Discord.Api/"]
COPY ["Alyx.Discord.Bot/Alyx.Discord.Bot.csproj", "Alyx.Discord.Bot/"]
RUN dotnet restore "Alyx.Discord.Api/Alyx.Discord.Api.csproj"
COPY . .
WORKDIR "/src/Alyx.Discord.Api"
RUN dotnet build "Alyx.Discord.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Alyx.Discord.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Alyx.Discord.Api.dll"]
