# Use the official .NET 8 SDK as a build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory
WORKDIR /src

# Copy the project files to the container
COPY . .

# Restore dependencies
RUN dotnet restore

# Publish the application
RUN dotnet publish -c Release -o ./Publish

# Use the official .NET 8 runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

# Copy the publish directory from the build stage
COPY --from=build /src/Publish /app

# Set the working directory
WORKDIR /app

# Set the entry point for the application
ENTRYPOINT ["dotnet", "/app/Inkloom.Api.dll"]