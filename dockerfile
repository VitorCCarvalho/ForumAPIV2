# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["src/ForumCRUD.API/ForumCRUD.API.csproj", "ForumCRUD.API/"]
RUN dotnet restore "ForumCRUD.API/ForumCRUD.API.csproj"

# Copy the rest of the source code
COPY ["src/ForumCRUD.API/", "ForumCRUD.API/"]

# Build the application
WORKDIR "/src/ForumCRUD.API"
RUN dotnet build "ForumCRUD.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ForumCRUD.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "ForumCRUD.API.dll"]