FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["OGameServicesRunner/OGameServicesRunner.csproj", "OGameServicesRunner/"]
RUN dotnet restore "OGameServicesRunner/OGameServicesRunner.csproj"
COPY . .
WORKDIR "/src/OGameServicesRunner"
RUN dotnet build "OGameServicesRunner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OGameServicesRunner.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OGameServicesRunner.dll"]
