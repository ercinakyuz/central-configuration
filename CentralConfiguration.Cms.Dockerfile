FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
COPY ["CentralConfiguration.Cms/CentralConfiguration.Cms.csproj", "CentralConfiguration.Cms/"]
COPY ["CentralConfiguration.Model/CentralConfiguration.Model.csproj", "CentralConfiguration.Model/"]
COPY ["CentralConfiguration.MessageBroker/CentralConfiguration.MessageBroker.csproj", "CentralConfiguration.MessageBroker/"]
RUN dotnet restore "CentralConfiguration.Cms/CentralConfiguration.Cms.csproj"
COPY . .
WORKDIR "/CentralConfiguration.Cms"
RUN dotnet build "CentralConfiguration.Cms.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CentralConfiguration.Cms.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CentralConfiguration.Cms.dll"]