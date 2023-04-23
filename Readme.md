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
$version="1.0.4"
$env:GH_OWNER="HarvestHelper"
$env:GH_PAT="[PAT]"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t harvesthelper.equipmentinventory:$version .
```

## Run the docker image
### local
```powershell
$version="1.0.4"
docker run -it --rm -p 5004:5004 --name equipmentinventory -e MongoDbSettings__Host=mongo -e RabbitMQSettings__Host=rabbitmq --network=harvesthelperinfra_default harvesthelper.equipmentinventory:$version
```
### cloud
```powershell
$version="1.0.4"
$cosmosDbConnString="[Connection string]"
$serviceBusConnString="[Connection string]"
docker run -it --rm -p 5004:5004 --name equipmentinventory -e MongoDbSettings__ConnectionString=$cosmosDbConnString -e ServiceBusSettings__ConnectionString=$serviceBusConnString -e ServiceSettings__MessageBroker="SERVICEBUS" harvesthelper.equipmentinventory:$version
```

## Publishing the docker image
```powershell
$version="1.0.4"
$appname="harvesthelper"

az acr login --name $appname

docker tag harvesthelper.equipmentinventory:$version "$appname.azurecr.io/harvesthelper.equipmentinventory:$version"

docker push "$appname.azurecr.io/harvesthelper.equipmentinventory:$version"
```

## Create the kubernetes namespace
```powershell
$namespace="equipmentinventory"
kubectl create namespace $namespace
```

## Create the kubernetes pods
```powershell
$namespace="equipmentinventory"

kubectl apply -f .\kubernetes\equipmentInventory.yaml -n $namespace
```

## Create the azure managed identity and granting access to keyvault secrets
```powershell
$appname="harvesthelper"
$namespace="equipmentinventory"

az identity create --resource-group $appname --name $namespace

$IDETITY_CLIENT_ID=az identity show -g $appname -n $namespace --query clientId -otsv

az keyvault set-policy -n $appname --secret-permissions get list --spn $IDETITY_CLIENT_ID
```

## Establish the federated identity credential 
```powershell
$appname="harvesthelper"

$AKS_OIDC_ISSUER=az aks show -n $appname -g $appname --query "oidcIssuerProfile.issuerUrl" -otsv

az identity federated-credential create --name $namespace --identity-name $namespace --resource-group $appname --issuer $AKS_OIDC_ISSUER --subject "system:serviceaccount:${namespace}:${namespace}-serviceaccount"
```