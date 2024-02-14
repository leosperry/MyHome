FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY MyHome/*.csproj ./MyHome/
COPY MyHome/appsettings.Production.json ./MyHome/appsettings.Production.json
COPY ha-kafka-net/src/HaKafkaNet/*.csproj ./ha-kafka-net/src/HaKafkaNet/

RUN dotnet restore MyHome

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out ./MyHome/MyHome.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV TZ="US/Eastern"
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "MyHome.dll"]