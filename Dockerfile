# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /usr
EXPOSE 5000
ENV ASPNETCORE_URLS=http://*:5000

# Copy csproj and restore as distinct layers
COPY *.* ./GenFinder/
ADD GenFinder ./GenFinder/GenFinder/
ADD GenFinderTests ./GenFinder/GenFinderTests/

WORKDIR /usr/GenFinder
RUN dotnet build

WORKDIR /usr/GenFinder/GenFinder/bin/Debug/net5.0

#Entrypoint
ENTRYPOINT ["dotnet","GenFinder.dll"]
