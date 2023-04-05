# HarvestHelper.EquipmentInventory

EquipmentInventory service used in HarvestHelper

## How to create and publish my package
```powershell
$version="1.0.1"
$owner="HarvestHelper" 
$gh_pat="[PAT HERE]"

dotnet pack src\HarvestHelper.EquipmentInventory.Contracts\ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/HarvestHelper.EquipmentInventory -o ..\packages

dotnet nuget push ..\packages\HarvestHelper.EquipmentInventory.Contracts.$version.nupkg --api-key $gh_pat --source "github" 
```