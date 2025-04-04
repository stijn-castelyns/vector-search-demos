targetScope = 'subscription'

@minLength(3)
@maxLength(11)
param storage_account_name string
param resource_group_name string

param computer_vision_name string
param sql_server_name string
param sql_admin_login string
param sql_admin_password string
param entra_admin_email string
param entra_admin_object_id string

param location string

resource vectorDemoRG 'Microsoft.Resources/resourceGroups@2024-07-01' = {
  name: resource_group_name
  location: location
}

module storageAccount 'resources/storage_account.bicep' = {
  scope: vectorDemoRG
  name: storage_account_name
  params: {
    storage_account_name: storage_account_name
    container_names: ['shoe-pictures', 'uploaded-pictures']
  }
}

module sqlServerAndDatabase 'resources/sql.bicep' = {
  scope: vectorDemoRG
  name: sql_server_name
  params: {
    database_name: 'VectorSearchDemoDb'
    entra_admin_email: entra_admin_email
    entra_admin_object_id: entra_admin_object_id
    sql_admin_login: sql_admin_login
    sql_admin_password: sql_admin_password
    sql_server_name: sql_server_name
  }
}

module computerVision 'resources/computer_vision.bicep' = {
  scope: vectorDemoRG
  name: computer_vision_name
  params: {
    computer_vision_name: computer_vision_name
  }
}

output storage_account_name string = storageAccount.outputs.storage_account_name
output storage_primary_connection_string string = storageAccount.outputs.primary_connection_string
output storage_primary_key string = storageAccount.outputs.primary_key
