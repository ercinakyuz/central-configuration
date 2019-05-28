FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
COPY ["CentralConfiguration.ApiClient/CentralConfiguration.ApiClient.csproj", "CentralConfiguration.ApiClient/"]
COPY ["CentralConfiguration.MessageBroker/CentralConfiguration.MessageBroker.csproj", "CentralConfiguration.MessageBroker/"]
COPY ["CentralConfiguration.Model/CentralConfiguration.Model.csproj", "CentralConfiguration.Model/"]
COPY ["CentralConfiguration.Core/CentralConfiguration.Core.csproj", "CentralConfiguration.Core/"]
RUN dotnet restore "CentralConfiguration.ApiClient/CentralConfiguration.ApiClient.csproj"
COPY . .
WORKDIR "/CentralConfiguration.ApiClient"
RUN dotnet build "CentralConfiguration.ApiClient.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CentralConfiguration.ApiClient.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CentralConfiguration.ApiClient.dll"]