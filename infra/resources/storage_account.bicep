param location string = resourceGroup().location
@minLength(3)
@maxLength(11)
param storage_account_name string
param container_names array

var unique_storage_account_name = '${storage_account_name}${uniqueString(resourceGroup().name)}'

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: unique_storage_account_name
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  resource blobServices 'blobServices@2023-05-01' = {
    name: 'default'
    resource containers 'containers@2023-05-01' = [for container_name in container_names: {
      name: container_name
    }]
  }
}

output storage_account_name string = unique_storage_account_name
output primary_connection_string string = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
output primary_key string = storageAccount.listKeys().keys[0].value
