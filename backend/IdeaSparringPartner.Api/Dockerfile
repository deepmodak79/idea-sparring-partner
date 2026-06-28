# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY IdeaSparringPartner.Api.csproj ./
RUN dotnet restore IdeaSparringPartner.Api.csproj

COPY . .
RUN dotnet publish IdeaSparringPartner.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
# Render sets PORT (often 10000). Override in Render env if needed:
# ASPNETCORE_URLS=http://0.0.0.0:10000
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "IdeaSparringPartner.Api.dll"]
