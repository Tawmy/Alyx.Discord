namespace Alyx.Discord.Api;

public static class EnvironmentVariables
{
    /// <summary>
    ///     OIDC Authority for REST endpoint authorization.
    /// </summary>
    public const string AuthAuthority = "AUTH_AUTHORITY";

    /// <summary>
    ///     OIDC access token audience for REST endpoint authorization.
    /// </summary>
    public const string AuthAudience = "AUTH_AUDIENCE";

    /// <summary>
    ///     PEM formatted X.509 Certificate to encrypt and decrypt data encryption keys with.
    /// </summary>
    /// <remarks>
    ///     Use path + file name WITHOUT extension. .pem and .key extensions for both files will be added automatically.
    ///     See compose.yml for reference.
    /// </remarks>
    public const string DataProtectionCertificate = "DATA_PROTECTION_CERTIFICATE";

    /// <summary>
    ///     Optional secondary PEM formatted X.509 certificate to decrypt data encryption keys with.
    /// </summary>
    /// <remarks>
    ///     When generating a new certificate and replacing <see cref="DataProtectionCertificate" />,
    ///     set this to be the previous certificate.
    ///     New keys will be generated using the new certificate while
    ///     this older certificate will still be available for keys that are still in use by older cookies etc.
    ///     Use path + file name WITHOUT extension. .pem and .key extensions for both files will be added automatically.
    /// </remarks>
    public const string DataProtectionCertificateAlt = "DATA_PROTECTION_CERTIFICATE_ALT";
}