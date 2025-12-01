# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["backend/ECommerce.API/ECommerce.API.csproj", "ECommerce.API/"]
COPY ["backend/ECommerce.Application/ECommerce.Application.csproj", "ECommerce.Application/"]
COPY ["backend/ECommerce.Domain/ECommerce.Domain.csproj", "ECommerce.Domain/"]
COPY ["backend/ECommerce.Infrastructure/ECommerce.Infrastructure.csproj", "ECommerce.Infrastructure/"]

RUN dotnet restore "ECommerce.API/ECommerce.API.csproj"

# Copy everything else and build
COPY backend/. .
WORKDIR "/src/ECommerce.API"
RUN dotnet build "ECommerce.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ECommerce.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80

ENTRYPOINT ["dotnet", "ECommerce.API.dll"]
