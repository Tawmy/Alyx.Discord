using System.Security.Cryptography.X509Certificates;
using Alyx.Discord.Db;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using NetStone.Common.Extensions;

namespace Alyx.Discord.Api.Extensions;

internal static class WebApplicationBuilderExtension
{
    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration.GetGuardedConfiguration(EnvironmentVariables.AuthAuthority);
                options.Audience = builder.Configuration.GetGuardedConfiguration(EnvironmentVariables.AuthAudience);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateActor = true,
                    ValidateAudience = true,
                    ValidateTokenReplay = true
                };
            });
    }

    public static void AddDataProtection(this IServiceCollection services, IConfiguration configuration)
    {
        var certificatePath = configuration.GetGuardedConfiguration(EnvironmentVariables.DataProtectionCertificate);
        var certificate = X509Certificate2.CreateFromPemFile($"{certificatePath}.pem", $"{certificatePath}.key");

        X509Certificate2[] decryptionCertificates;
        if (configuration[EnvironmentVariables.DataProtectionCertificateAlt] is { } certificateAltPath)
        {
            // alternative certificate for decryption provided, use both
            var certificateAlt = X509Certificate2.CreateFromPemFile(
                $"{certificateAltPath}.pem", $"{certificateAltPath}.key");
            decryptionCertificates = [certificate, certificateAlt];
        }
        else
        {
            // only one certificate provided
            decryptionCertificates = [certificate];
        }

        services.AddDataProtection()
            .PersistKeysToDbContext<DatabaseContext>()
            .ProtectKeysWithCertificate(certificate)
            .UnprotectKeysWithAnyCertificate(decryptionCertificates.ToArray());
    }
}