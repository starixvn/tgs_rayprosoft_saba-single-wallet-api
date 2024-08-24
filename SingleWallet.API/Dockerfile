#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.


FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["SabaSingleWallet.API/SabaSingleWallet.API.csproj", "SabaSingleWallet.API/"]
COPY ["SabaSingleWallet.Repositories/SabaSingleWallet.Repositories.csproj", "SabaSingleWallet.Repositories/"]
RUN dotnet restore "./SabaSingleWallet.API/./SabaSingleWallet.API.csproj"
COPY . .
WORKDIR "/src/SabaSingleWallet.API"
RUN dotnet build "./SabaSingleWallet.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SabaSingleWallet.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM hub.lumigame.com/base/dotnet/core/aspnet:8.0-alpine-24h1.1 AS base
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.Apis.SabaSingleWallet.API.dll"]