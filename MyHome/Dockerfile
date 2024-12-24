FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY MyHome/MyHome.csproj ./MyHome/MyHome.csproj
COPY MyHome/appsettings.Production.json ./MyHome/appsettings.json
COPY ha-kafka-net/src/HaKafkaNet/*.csproj ./ha-kafka-net/src/HaKafkaNet/

RUN dotnet restore MyHome/MyHome.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out ./MyHome/MyHome.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
ENV TZ="US/Eastern"
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "MyHome.dll"]