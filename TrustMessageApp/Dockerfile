FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Server/Server.csproj", "Server/"]
COPY ["BaseLibrary/BaseLibrary.csproj", "BaseLibrary/"]
RUN dotnet restore "Server/Server.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "Server/Server.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Server/Server.csproj" -c Release -o /app/publish

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

# Ensure ASPNETCORE_ENVIRONMENT is set to Production
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Server.dll"] 