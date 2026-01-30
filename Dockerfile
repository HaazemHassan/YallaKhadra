FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["YallaKhadra.API/YallaKhadra.API.csproj", "YallaKhadra.API/"]
COPY ["YallaKhadra.Core/YallaKhadra.Core.csproj", "YallaKhadra.Core/"]
COPY ["YallaKhadra.Infrastructure/YallaKhadra.Infrastructure.csproj", "YallaKhadra.Infrastructure/"]
COPY ["YallaKhadra.Services/YallaKhadra.Services.csproj", "YallaKhadra.Services/"]
RUN dotnet restore "./YallaKhadra.API/YallaKhadra.API.csproj"
COPY . .
WORKDIR "/src/YallaKhadra.API"
RUN dotnet build "./YallaKhadra.API.csproj" -c $BUILD_CONFIGURATION -o /app/build



FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./YallaKhadra.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false



FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YallaKhadra.API.dll"]