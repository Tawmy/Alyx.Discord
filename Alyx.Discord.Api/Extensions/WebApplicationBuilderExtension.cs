using Microsoft.AspNetCore.Authentication.JwtBearer;
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
}