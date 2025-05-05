variable "oidc_client_id" {
    type = string
    description = "The OIDC client ID"
}
variable "oidc_client_secret" {
    type = string
    description = "The OIDC client secret"
    sensitive = true
}
variable "oidc_metadata_address" {
    type = string
    description = "The OIDC metadata address"
}