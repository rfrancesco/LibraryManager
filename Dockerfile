# Dockerfile for deployment
# Listens on port 5010 internally
# Needs a SQL server connection
FROM mcr.microsoft.com/dotnet/aspnet:10.0 as base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=5010
EXPOSE 5010

FROM mcr.microsoft.com/dotnet/sdk:10.0 as build
WORKDIR /src
COPY . .
RUN dotnet restore

# Create test environment
FROM build AS test
RUN dotnet test --no-restore

# Build published target
# FROM test so that tests are required before publish step
FROM test AS publish    
RUN dotnet tool install --global dotnet-ef 
ENV PATH="${PATH}:/root/.dotnet/tools"
WORKDIR /src/LibraryManager
RUN dotnet publish -c Release -o /app/publish

# Create final container for deployment
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibraryManager.dll"]