provider "aws" {
    region = "eu-central-1"  # Change this to your desired region
    profile = "wiaz"
}
locals {
    development_parameters = {
        "Oidc/ClientSecret"         = { type = "SecureString", value = var.oidc_client_secret }
        "Oidc/ClientId"             = { type = "String", value = var.oidc_client_id }
        "Oidc/MetadataAddress"      = { type = "String", value = var.oidc_metadata_address }
        "Postgres/ConnectionString" = { type = "SecureString", value = "connection_string" }
    }
}

resource "aws_ssm_parameter" "development_params" {
    for_each    = local.development_parameters
    name        = "/TypesenseAuthWrapper/Development/${each.key}"
    description = "Development parameter"
    type        = each.value.type
    value       = each.value.value
    
    tier = "Standard"
    
    tags = {
        Environment = "Development"
        Application = "TypesenseAuthWrapper"
        ManagedBy   = "terraform"
    }
}