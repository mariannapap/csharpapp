# Use .NET SDK 8 for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution file from the root folder
COPY ../CSharpApp.sln ./

# Copy project files from the solution folder
COPY ../CSharpApp.Api/*.csproj CSharpApp.Api/
COPY ../CSharpApp.Application/*.csproj CSharpApp.Application/
COPY ../CSharpApp.Core/*.csproj CSharpApp.Core/
COPY ../CSharpApp.Infrastructure/*.csproj CSharpApp.Infrastructure/

# Restore dependencies
RUN dotnet restore CSharpApp.Api/CSharpApp.Api.csproj

# Copy all project files and build
COPY ../ ./
WORKDIR /app/CSharpApp.Api
RUN dotnet publish -c Release -o /out

# Use ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "CSharpApp.Api.dll"]
