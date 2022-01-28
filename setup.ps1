param (
    $resourceBaseName="orleansonazure$( Get-Random -Maximum 1000)",
    $location='northcentralus'
)

Write-Host 'Compiling app code' -ForegroundColor Cyan

dotnet build

Write-Host 'Building Silo' -ForegroundColor Cyan
dotnet publish OrleansOnAppService.Silo\OrleansOnAppService.Silo.csproj

Write-Host 'Building Dashboard' -ForegroundColor Cyan
dotnet publish OrleansOnAppService.Dashboard\OrleansOnAppService.Dashboard.csproj

Write-Host 'Building Client' -ForegroundColor Cyan
dotnet publish OrleansOnAppService.Client\OrleansOnAppService.Client.csproj

Write-Host 'Creating resource group' -ForegroundColor Cyan
az group create -l $location -n $resourceBaseName

Write-Host 'Creating Orleans Cluster and deploying code to it ' -ForegroundColor Cyan
az deployment group create --resource-group $resourceBaseName --template-file 'deploy/main.bicep'

Write-Host 'Deploying code ' -ForegroundColor Cyan
az webapp deploy -n "$($resourceBaseName)-silo" -g $resourceBaseName --src-path silo.zip
az webapp deploy -n "$($resourceBaseName)-dashboard" -g $resourceBaseName --src-path dashboard.zip
az webapp deploy -n "$($resourceBaseName)-client" -g $resourceBaseName --src-path client.zip

Write-Host 'Orleans Cluster deployed.' -ForegroundColor Cyan
az webapp restart -n "$($resourceBaseName)-silo" -g $resourceBaseName
az webapp restart -n "$($resourceBaseName)-dashboard" -g $resourceBaseName
az webapp restart -n "$($resourceBaseName)-client" -g $resourceBaseName

Write-Host 'Orleans Cluster deployed.' -ForegroundColor Cyan
az webapp browse -n "$($resourceBaseName)-silo" -g $resourceBaseName
az webapp browse -n "$($resourceBaseName)-dashboard" -g $resourceBaseName
az webapp browse -n "$($resourceBaseName)-client" -g $resourceBaseName