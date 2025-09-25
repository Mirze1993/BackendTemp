FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root
WORKDIR /app
RUN apt update && \
    apt install -y curl htop nano \
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Presentation/AuthApi/AuthApi.csproj", "Presentation/AuthApi/"]
COPY ["Core/Appilcation/Appilcation.csproj", "Core/Appilcation/"]
COPY ["Core/Domain/Domain.csproj", "Core/Domain/"]
COPY ["Infrastructure/ExternalServices/ExternalServices.csproj", "Infrastructure/ExternalServices/"]
COPY ["Infrastructure/PersistenceMongo/PersistenceMongo.csproj", "Infrastructure/PersistenceMongo/"]
COPY ["Infrastructure/PersistenceOracle/PersistenceOracle.csproj", "Infrastructure/PersistenceOracle/"]
RUN dotnet restore "Presentation/AuthApi/AuthApi.csproj"
COPY . .
WORKDIR "/src/Presentation/AuthApi"
RUN dotnet build "AuthApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AuthApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app


COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthApi.dll"]
