FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY MyHome.Transformer/*.csproj ./MyHome.Transformer/
COPY MyHome.Transformer/appsettings.Production.json ./MyHome.Transformer/appsettings.Production.json
COPY ha-kafka-net/src/HaKafkaNet/*.csproj ./ha-kafka-net/src/HaKafkaNet/

RUN dotnet restore MyHome.Transformer

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out ./MyHome.Transformer/MyHome.Transformer.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "MyHome.Transformer.dll"]
