# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Delhivery.Api/Delhivery.Api.csproj", "Delhivery.Api/"]
COPY ["Delhivery.Application/Delhivery.Application.csproj", "Delhivery.Application/"]
COPY ["Delhivery.Domain/Delhivery.Domain.csproj", "Delhivery.Domain/"]
COPY ["Delhivery.Infrastructure/Delhivery.Infrastructure.csproj", "Delhivery.Infrastructure/"]
RUN dotnet restore "./Delhivery.Api/Delhivery.Api.csproj"
COPY . .
WORKDIR "/src/Delhivery.Api"
RUN dotnet build "./Delhivery.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Delhivery.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Delhivery.Api.dll"]
