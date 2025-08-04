FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Presentation/AiAgentApi/AiAgentApi.csproj", "Presentation/AiAgentApi/"]
COPY ["Infrastructure/AiIntegration/AiIntegration.csproj", "Infrastructure/AiIntegration/"]
COPY ["Infrastructure/FaceArt/FaceArt.csproj", "Infrastructure/FaceArt/"]
COPY ["Infrastructure/PersistenceMongo/PersistenceMongo.csproj", "Infrastructure/PersistenceMongo/"]
COPY ["Core/Appilcation/Appilcation.csproj", "Core/Appilcation/"]
COPY ["Core/Domain/Domain.csproj", "Core/Domain/"]
COPY ["Infrastructure/PersistenceOracle/PersistenceOracle.csproj", "Infrastructure/PersistenceOracle/"]
RUN dotnet restore "Presentation/AiAgentApi/AiAgentApi.csproj"
COPY . .
WORKDIR "/src/Presentation/AiAgentApi"
RUN dotnet build "AiAgentApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AiAgentApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AiAgentApi.dll"]
