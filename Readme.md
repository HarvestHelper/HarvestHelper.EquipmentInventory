# HarvestHelper.EquipmentInventory

EquipmentInventory service used in HarvestHelper

## How to create and publish my package
```powershell
$version="1.0.2"
$owner="HarvestHelper" 
$gh_pat="[PAT HERE]"

dotnet pack src\HarvestHelper.EquipmentInventory.Contracts\ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/HarvestHelper.EquipmentInventory -o ..\packages

dotnet nuget push ..\packages\HarvestHelper.EquipmentInventory.Contracts.$version.nupkg --api-key $gh_pat --source "github" 
```

## Build the docker image
```powershell
$version="1.0.3"
$env:GH_OWNER="HarvestHelper"
$env:GH_PAT="[PAT]"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t harvesthelper.equipmentinventory:$version .
```

## Run the docker image
### local
```powershell
$version="1.0.3"
docker run -it --rm -p 5004:5004 --name equipmentinventory -e MongoDbSettings__Host=mongo -e RabbitMQSettings__Host=rabbitmq --network=harvesthelperinfra_default harvesthelper.equipmentinventory:$version
```
### cloud
```powershell
$version="1.0.3"
$cosmosDbConnString="[Connection string]"
$serviceBusConnString="[Connection string]"
docker run -it --rm -p 5004:5004 --name equipmentinventory -e MongoDbSettings__ConnectionString=$cosmosDbConnString -e ServiceBusSettings__ConnectionString=$serviceBusConnString -e ServiceSettings__MessageBroker="SERVICEBUS" harvesthelper.equipmentinventory:$version
```