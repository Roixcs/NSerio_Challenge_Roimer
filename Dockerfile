# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BackEnd/TeamTaskAPI/TeamTaskAPI/TeamTaskAPI.csproj", "TeamTaskAPI/"]
COPY ["BackEnd/TeamTaskAPI/TeamTasks.Application/TeamTasks.Application.csproj", "TeamTasks.Application/"]
COPY ["BackEnd/TeamTaskAPI/TeamTasks.Domain/TeamTasks.Domain.csproj", "TeamTasks.Domain/"]
COPY ["BackEnd/TeamTaskAPI/TeamTasks.Infrastructure/TeamTasks.Infrastructure.csproj", "TeamTasks.Infrastructure/"]
RUN dotnet restore "TeamTaskAPI/TeamTaskAPI.csproj"
COPY ["BackEnd/TeamTaskAPI/", "."]
WORKDIR "/src/TeamTaskAPI"
RUN dotnet build "TeamTaskAPI.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "TeamTaskAPI.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeamTaskAPI.dll"]
