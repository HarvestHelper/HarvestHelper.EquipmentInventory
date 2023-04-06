FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5004

ENV ASPNETCORE_URLS=http://+:5004

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/HarvestHelper.EquipmentInventory.Contracts/HarvestHelper.EquipmentInventory.Contracts.csproj", "src/HarvestHelper.EquipmentInventory.Contracts/"]
COPY ["src/HarvestHelper.EquipmentInventory.Service/HarvestHelper.EquipmentInventory.Service.csproj", "src/HarvestHelper.EquipmentInventory.Service/"]

RUN --mount=type=secret,id=GH_OWNER,dst=/GH_OWNER --mount=type=secret,id=GH_PAT,dst=/GH_PAT \
    dotnet nuget add source --username Duarte --password `cat /GH_PAT` --store-password-in-clear-text --name github "https://nuget.pkg.github.com/`cat /GH_OWNER`/index.json"


RUN dotnet restore "src/HarvestHelper.EquipmentInventory.Service/HarvestHelper.EquipmentInventory.Service.csproj"
COPY ./src ./src
WORKDIR "/src/src/HarvestHelper.EquipmentInventory.Service"
RUN dotnet publish "HarvestHelper.EquipmentInventory.Service.csproj" -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HarvestHelper.EquipmentInventory.Service.dll"]