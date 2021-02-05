# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY API/*.csproj ./API/
RUN dotnet restore

# copy everything else and build app
COPY API/. ./API/
WORKDIR /source/API
RUN dotnet publish -c release -o /app --no-restore

RUN dotnet tool install --global dotnet-ef --version 5.0
RUN ~/.dotnet/tools/dotnet-ef database update && cp reledger.db /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "API.dll"]

