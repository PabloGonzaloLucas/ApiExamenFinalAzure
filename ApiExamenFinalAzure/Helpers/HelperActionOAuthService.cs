using ApiExamenFinalAzure.Models;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiExamenFinalAzure.Helpers
{
    public class HelperActionOAuthService
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        private SecretClient secretclient;

        //private static KeyVaultAccesorModel keyVaultSecrets;

        //public HelperActionOAuthService(IConfiguration configuration)
        //{
        //    this.Issuer = configuration.GetValue<string>
        //        ("ApiOAuthToken:Issuer");
        //    this.Audience = configuration.GetValue<string>
        //        ("ApiOAuthToken:Audience");
        //    this.SecretKey = configuration.GetValue<string>
        //        ("ApiOAuthToken:SecretKey");
        //}
        public HelperActionOAuthService(IConfiguration configuration, SecretClient client)
        {
            this.secretclient = client;

            KeyVaultSecret secretIssuer = this.secretclient.GetSecret("Issuer");
            this.Issuer = secretIssuer.Value;

            KeyVaultSecret secretAudience = this.secretclient.GetSecret("Audience");
            this.Audience = secretAudience.Value;

            KeyVaultSecret secretKey = this.secretclient.GetSecret("SecretKey");
            this.SecretKey = secretKey.Value;
        }

        public SymmetricSecurityKey GetKeyToken()
        {
            byte[] data = Encoding.UTF8.GetBytes(this.SecretKey);
            return new SymmetricSecurityKey(data);
        }

        public Action<JwtBearerOptions> GetJWTBearerOptions()
        {
            Action<JwtBearerOptions> options =
                new Action<JwtBearerOptions>(options =>
                {
                    // Helps surface the concrete reason for 401 responses during development/troubleshooting.
                    options.IncludeErrorDetails = true;

                    options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = this.Issuer,
                        ValidAudience = this.Audience,
                        IssuerSigningKey = this.GetKeyToken(),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // Will show up in Application logs (Azure App Service) / console logs.
                            context.Response.Headers["x-jwt-auth-failed"] = "true";
                            context.Response.Headers["x-jwt-auth-error"] = context.Exception.GetType().Name;
                            // Don't put exception messages in headers (can contain sensitive info).
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            // When a 401 is produced, capture the reason in logs.
                            // NOTE: HandleResponse stays false so default 401 behavior remains.
                            if (!string.IsNullOrEmpty(context.Error))
                            {
                                context.Response.Headers["x-jwt-challenge-error"] = context.Error;
                            }

                            if (!string.IsNullOrEmpty(context.ErrorDescription))
                            {
                                // ErrorDescription can be long; keep it minimal.
                                context.Response.Headers["x-jwt-challenge-desc"] = context.ErrorDescription.Length > 120
                                    ? context.ErrorDescription.Substring(0, 120)
                                    : context.ErrorDescription;
                            }

                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            context.Response.Headers["x-jwt-token-validated"] = "true";
                            return Task.CompletedTask;
                        }
                    };
                });
            return options;
        }

        public Action<AuthenticationOptions> GetAuthenticationSchema()
        {
            Action<AuthenticationOptions> options =
                new Action<AuthenticationOptions>(options =>
                {
                    options.DefaultScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                });
            return options;
        }
    }
}
