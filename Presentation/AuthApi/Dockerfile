FROM --platform=linux/amd64 pyroscope/pyroscope-dotnet:0.13.0-glibc AS profile

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

USER root
WORKDIR /app
EXPOSE 80
RUN apt update && \
    apt install -y curl htop nano


COPY --from=profile /Pyroscope.Profiler.Native.so ./Pyroscope.Profiler.Native.so
COPY --from=profile /Pyroscope.Linux.ApiWrapper.x64.so ./Pyroscope.Linux.ApiWrapper.x64.so


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



ENV PYROSCOPE_PROFILING_ENABLED=1
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={BD1A650D-AC5D-4896-B64F-D6FA25D6B26A}
ENV CORECLR_PROFILER_PATH=/app/Pyroscope.Profiler.Native.so
ENV LD_PRELOAD=/app/Pyroscope.Linux.ApiWrapper.x64.so

ENV PYROSCOPE_SERVER_ADDRESS=https://profiles-prod-021.grafana.net
ENV PYROSCOPE_BASIC_AUTH_USER=1348846
ENV PYROSCOPE_BASIC_AUTH_PASSWORD=glc_eyJvIjoiODM1NDQwIiwibiI6InN0YWNrLTEzOTY3NDEtaHQtcmVhZC1wcm9zIiwiayI6IkQ2UjlacDhEZ0JYOXJrNTcwOXYwMVFQZSIsIm0iOnsiciI6InByb2QtbWUtY2VudHJhbC0xIn19

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthApi.dll"]
