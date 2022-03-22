variable "project" {
  type        = string
  description = "Project name"
}

variable "environment" {
  type        = string
  description = "Environment (dev / stage / prod)"
}

variable "location" {
  type        = string
  description = "Azure region to deploy module to"
}

variable "functionapp_source" {
  type    = string
   
  default="C:/Users/LENOVO/Desktop/AzureFunctionUsingBlobStorage/AzureFunctionUsingBlobStorage/bin/Debug/net6.0/publish"
  #C:\Users\LENOVO\Desktop\AzureFunctions\AzureFunctions\bin\Debug\net6.0\publish
}
 

variable "functionapp" {
  type    = string
  default="C:/Users/LENOVO/Desktop/AzureFunctions/PublishedAzureFunctionUsingBlobStorage/PublishedAzureFunctionUsingBlobStorage.zip"
#C:\Users\LENOVO\Desktop\AzureFunctions\PublishedAzureFunctions
}

variable "storage_blob" {
  type    = string
  default = "my-first-function-deployment-zip"
}



#resource "azurerm_storage_container" "storage_container" {
#    name = "function-releases"
#    storage_account_name = "${azurerm_storage_account.storage_account.name}"
#    container_access_type = "private"
#}

#resource "azurerm_storage_blob" "appcode" {
#    name = "functionapp.zip"
#    storage_account_name = "${azurerm_storage_account.storage_account.name}"
#    storage_container_name = "${azurerm_storage_container.storage_container.name}"
#    type = "block"
#    source = "${var.functionapp}"
#}
#data "azurerm_storage_account_sas" "sas" {
#    connection_string = "${azurerm_storage_account.storage_account.primary_connection_string}"
#    https_only = true
#    start = "2022-01-01"
#    expiry = "2023-12-31"
#    resource_types {
#        object = true
#        container = false
#        service = false
#    }
#    services {
#        blob = true
#        queue = false
#        table = false
#        file = false
#    }
#    permissions {
#        read = true
#        write = false
#        delete = false
#        list = false
#        add = false
#        create = false
#        update = false
#        process = false
#    }
#}


