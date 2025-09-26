FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root
WORKDIR /app
EXPOSE 80

RUN apt update && \
    apt install -y curl htop nano

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Presentation/FileApi/FileApi.csproj", "Presentation/FileApi/"]
COPY ["Core/Appilcation/Appilcation.csproj", "Core/Appilcation/"]
COPY ["Core/Domain/Domain.csproj", "Core/Domain/"]
COPY ["Infrastructure/FaceArt/FaceArt.csproj", "Infrastructure/FaceArt/"]
RUN dotnet restore "Presentation/FileApi/FileApi.csproj"
COPY . .
WORKDIR "/src/Presentation/FileApi"
RUN dotnet build "FileApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FileApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileApi.dll"]
