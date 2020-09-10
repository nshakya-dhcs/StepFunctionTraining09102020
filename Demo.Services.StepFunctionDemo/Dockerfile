FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY src/Demo.Services.ECS/Demo.Services.ECS.csproj ./Demo.Services.ECS/

COPY nuget.config .
COPY proxycert.pem .
ENV SSL_CERT_FILE=/src/proxycert.pem

RUN dotnet restore "Demo.Services.ECS/Demo.Services.ECS.csproj"

COPY src/Demo.Services.ECS/. ./Demo.Services.ECS/

WORKDIR /src
RUN dotnet build "Demo.Services.ECS/Demo.Services.ECS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Demo.Services.ECS/Demo.Services.ECS.csproj" -c Release -o /app/publish
FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demo.Services.ECS.dll"]