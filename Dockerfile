FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["backend/ECommerce.API/ECommerce.API.csproj", "ECommerce.API/"]
COPY ["backend/ECommerce.Application/ECommerce.Application.csproj", "ECommerce.Application/"]
COPY ["backend/ECommerce.Domain/ECommerce.Domain.csproj", "ECommerce.Domain/"]
COPY ["backend/ECommerce.Infrastructure/ECommerce.Infrastructure.csproj", "ECommerce.Infrastructure/"]
RUN dotnet restore "ECommerce.API/ECommerce.API.csproj"
COPY backend/ .
WORKDIR "/src/ECommerce.API"
RUN dotnet build "ECommerce.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ECommerce.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ECommerce.API.dll"]
