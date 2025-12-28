namespace Alyx.Discord.Core;

// TODO docs
public static class EnvironmentVariables
{
    public const string StatusMessage = "STATUS_MESSAGE";

    public const string NetStoneApiRootUri = "NETSTONE_API_ROOT_URI";
    public const string NetStoneApiAuthority = "NETSTONE_API_AUTHORITY";
    public const string NetStoneApiClientId = "NETSTONE_API_CLIENT_ID";
    public const string NetStoneApiScopes = "NETSTONE_API_SCOPES";

    /// <summary>
    ///     ECDSA P-256 PEM formatted certificate to sign JWTs with.
    /// </summary>
    /// <remarks>
    ///     Use path + file name WITHOUT extension. .pem and .key extensions for both files will be added automatically.
    ///     See compose.yml for reference.
    /// </remarks>
    public const string NetStoneApiClientSignedJwtCert = "NETSTONE_API_CLIENT_SIGNED_JWT_CERTIFICATE";

    public const string NetStoneMaxAgeCharacter = "NETSTONE_MAX_AGE_CHARACTER";
    public const string NetStoneMaxAgeClassJobs = "NETSTONE_MAX_AGE_CLASS_JOBS";
    public const string NetStoneMaxAgeMinions = "NETSTONE_MAX_AGE_MINIONS";
    public const string NetStoneMaxAgeMounts = "NETSTONE_MAX_AGE_MOUNTS";
    public const string NetStoneMaxAgeFreeCompany = "NETSTONE_MAX_FREE_COMPANY";
    public const string ForceRefreshCooldown = "FORCE_REFRESH_COOLDOWN";
}