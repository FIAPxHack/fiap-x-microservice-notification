# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solution e projeto
COPY NotificationService.sln ./
COPY src/NotificationService.csproj src/

# Restaurar dependências
RUN dotnet restore

# Copiar código fonte
COPY . .

# Build da aplicação
WORKDIR /src/src
RUN dotnet build NotificationService.csproj -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish NotificationService.csproj -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 5004

# Copiar artefatos publicados
COPY --from=publish /app/publish .

# Definir entrypoint
ENTRYPOINT ["dotnet", "NotificationService.dll"]
