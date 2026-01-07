# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Fcg.Games.Service.Api/Fcg.Games.Service.Api.csproj", "src/Fcg.Games.Service.Api/"]
COPY ["src/Fcg.Games.Service.Application/Fcg.Games.Service.Application.csproj", "src/Fcg.Games.Service.Application/"]
COPY ["src/Fcg.Games.Service.Domain/Fcg.Games.Service.Domain.csproj", "src/Fcg.Games.Service.Domain/"]
COPY ["src/Fcg.Games.Service.Infra/Fcg.Games.Service.Infra.csproj", "src/Fcg.Games.Service.Infra/"]
COPY ["src/Fcg.Games.Service.Infra.Elastic/Fcg.Games.Service.Infra.Elastic.csproj", "src/Fcg.Games.Service.Infra.Elastic/"]

# Restaurar dependências
RUN dotnet restore "src/Fcg.Games.Service.Api/Fcg.Games.Service.Api.csproj"

# Copiar tudo
COPY . .

# Build
WORKDIR "/src/src/Fcg.Games.Service.Api"
RUN dotnet build "Fcg.Games.Service.Api.csproj" -c Release -o /app/build

# Estágio de publicação
FROM build AS publish
RUN dotnet publish "Fcg.Games.Service.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fcg.Games.Service.Api.dll"]