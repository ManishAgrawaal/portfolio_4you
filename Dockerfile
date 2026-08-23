FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DevPortfolio.API/DevPortfolio.API.csproj DevPortfolio.API/
RUN dotnet restore DevPortfolio.API/DevPortfolio.API.csproj

COPY . .
RUN dotnet publish DevPortfolio.API/DevPortfolio.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "DevPortfolio.API.dll"]
*.db
*.db-shm
*.db-wal