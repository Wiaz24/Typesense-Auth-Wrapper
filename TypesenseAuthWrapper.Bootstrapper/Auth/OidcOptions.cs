namespace TypesenseAuthWrapper.Bootstrapper.Auth;

public class OidcOptions
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string MetadataAddress { get; init; }
    public required bool RequireHttpsMetadata { get; init; }
    public required string RoleClaimType { get; init; }
    public bool ValidateIssuer { get; init; } = true;
    public bool ValidateAudience { get; init; } = true;
    public bool ValidateLifetime { get; init; } = true;
    public bool ValidateIssuerSigningKey { get; init; } = true;
}