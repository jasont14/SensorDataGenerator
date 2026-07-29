FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/SensorDataGenerator/*.csproj src/SensorDataGenerator/
RUN dotnet restore src/SensorDataGenerator/SensorDataGenerator.csproj
COPY src/ src/
RUN dotnet publish src/SensorDataGenerator/SensorDataGenerator.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:5000
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SensorDataGenerator.dll"]
