# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.azure.cn/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.azure.cn/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HomeLibrary/HomeLibrary.Api/HomeLibrary.Api.csproj", "HomeLibrary.Api/"]
COPY ["HomeLibrary/HomeLibrary.Core/HomeLibrary.Core.csproj", "HomeLibrary.Core/"]
COPY ["HomeLibrary/HomeLibrary.Application/HomeLibrary.Application.csproj", "HomeLibrary.Application/"]

RUN dotnet restore "./HomeLibrary.Api/HomeLibrary.Api.csproj"
COPY HomeLibrary/. .
WORKDIR "/src/HomeLibrary.Api"
RUN dotnet build "./HomeLibrary.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HomeLibrary.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM node:24-alpine AS febuild
WORKDIR /src/fe
COPY ["frontend/home-library/package.json", "./"]
COPY ["frontend/home-library/package-lock.json", "./"]
RUN wget -q -O- --timeout=10 https://registry.npmjs.org/ >/dev/null && echo "registry OK" || echo "registry FAILED"
RUN npm install
COPY frontend/home-library/. .
RUN npm run build

RUN echo "FE  done"

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=febuild /src/fe/dist/home-library ./ClientApp/
ENTRYPOINT ["dotnet", "HomeLibrary.Api.dll"]
