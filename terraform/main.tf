locals {
    development_parameters = {
        "Oidc/ClientSecret"         = { type = "SecureString", value = var.oidc_client_secret }
        "Oidc/ClientId"             = { type = "String", value = var.oidc_client_id }
        "Oidc/MetadataAddress"      = { type = "String", value = var.oidc_metadata_address }
        "Postgres/ConnectionString" = { type = "SecureString", value = "Host=postgres-service;Port=5432;Database=${var.postgres-default-db};Username=employee_user;Password=employee_${var.postgres-default-password}" }
        "AWS/Cognito/UserPoolId"    = { type = "String", value = "${var.cognito-user-pool-id}" }
        "AllowedOrigins"            = { type = "StringList", value = "${local.allowed_origins}" }
        "MessageBroker/RabbitMQ/UserName"   = { type = "String", value = "${var.rabbitmq-default-username}" }
        "MessageBroker/RabbitMQ/Password"   = { type = "SecureString", value = "${var.rabbitmq-default-password}" }
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