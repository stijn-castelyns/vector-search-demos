param location string = resourceGroup().location
param computer_vision_name string

var unique_computer_vision_name = '${computer_vision_name}${uniqueString(resourceGroup().name)}'

resource aiComputerVision 'Microsoft.CognitiveServices/accounts@2024-10-01' = {
  name: unique_computer_vision_name 
  location: location
  sku: {
    tier: 'Standard'
    name: 'S1'
  }
  kind: 'ComputerVision'
  properties: {
    networkAcls: {
      defaultAction: 'Allow'
      ipRules: []
      virtualNetworkRules: []
    }
  }
}

output endpoint string = aiComputerVision.properties.endpoint
output key string = aiComputerVision.listKeys().key1
