# Use .NET SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy only the .csproj file(s) for restore
COPY ./AIGame_backend/*.csproj ./

# Restore dependencies
RUN dotnet restore

# Copy the rest of the code
COPY ./AIGame_backend/. ./

# Build the application
RUN dotnet publish -c Release -o /out

# Use the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "AIGame_backend.dll"]
