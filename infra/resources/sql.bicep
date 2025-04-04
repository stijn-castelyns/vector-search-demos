param location string = resourceGroup().location
param sql_server_name string
param sql_admin_login string
param sql_admin_password string
param entra_admin_email string
param entra_admin_object_id string
param database_name string

var unique_sql_server_name = '${sql_server_name}${uniqueString(resourceGroup().name)}'

resource sqlServer 'Microsoft.Sql/servers@2024-05-01-preview' = {
  name: unique_sql_server_name
  location: location
  properties: {
    administratorLogin:sql_admin_login
    administratorLoginPassword: sql_admin_password
    publicNetworkAccess: 'Enabled'
    administrators: {
      administratorType: 'ActiveDirectory'
      login: entra_admin_email
      principalType: 'User'
      sid: entra_admin_object_id
      tenantId: tenant().tenantId
    }
  }
  resource firewallRules 'firewallRules@2024-05-01-preview' = {
    name: 'everybodywelcome'
    properties: {
      endIpAddress: '255.255.255.255'
      startIpAddress: '0.0.0.0'
    }
  }
}

resource database 'Microsoft.Sql/servers/databases@2024-05-01-preview' = {
  parent: sqlServer
  name: database_name
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
  }
}

output connection_string string = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${database.name};Persist Security Info=False;User ID=${sql_admin_login};Password=${sql_admin_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
