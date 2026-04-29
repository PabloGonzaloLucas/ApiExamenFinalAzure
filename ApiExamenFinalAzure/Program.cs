using ApiExamenFinalAzure.Data;
using ApiExamenFinalAzure.Helpers;
using ApiExamenFinalAzure.Models;
using ApiExamenFinalAzure.Repositories;
using ApiExamenFinalAzure.Services;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient
    (builder.Configuration.GetSection("KeyVault"));
});

SecretClient secretClient =
    builder.Services.BuildServiceProvider()
    .GetService<SecretClient>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<HelperUsuarioToken>();
HelperCryptography.Initialize(builder.Configuration, secretClient);

HelperActionOAuthService helper =
    new HelperActionOAuthService(builder.Configuration, secretClient);

builder.Services.AddSingleton<HelperActionOAuthService>(helper);

builder.Services.AddAuthentication(helper.GetAuthenticationSchema())
    .AddJwtBearer(helper.GetJWTBearerOptions());

SecretClient secretClient2 =
    builder.Services.BuildServiceProvider().GetService<SecretClient>();
KeyVaultSecret secretoStorage = await secretClient2.GetSecretAsync("secretstorageexamenazurepgl");
KeyVaultSecret secretoBlobsUrl = await secretClient2.GetSecretAsync("secretblobfreeurlpgl");
KeyVaultSecret secretoSqlConnectionString = await secretClient2.GetSecretAsync("secretsqlstringexamenpgl");
//KeyVaultSecret secretoCypherKey = await secretClient.GetSecretAsync("secretcypherkeyexamenpgl");
KeyVaultSecret secretoCypherKey = await secretClient2.GetSecretAsync("secretkeycypherprueba");
KeyVaultSecret secretoSecretKey = await secretClient2.GetSecretAsync("secretkeyprueba");
KeyVaultSecret secretoIssuer = await secretClient2.GetSecretAsync("secretissuerexamenpgl");
KeyVaultSecret secretoAudience = await secretClient2.GetSecretAsync("secretaudienceexamenpgl");

var keyVaultSecrets = new KeyVaultAccesorModel
{
    StorageAccountConnectionString = secretoStorage.Value,
    BlobsBaseUrl = secretoBlobsUrl.Value,
    SqlConnectionString = secretoSqlConnectionString.Value,
    SecretKey = secretoSecretKey.Value,
    CypherKey = secretoCypherKey.Value,
    Issuer = secretoIssuer.Value,
    Audience = secretoAudience.Value
};
builder.Services.AddSingleton(keyVaultSecrets);


//HelperCryptography.Initialize(builder.Configuration, keyVaultSecrets);




BlobServiceClient blobServiceClient = new BlobServiceClient(secretoStorage.Value);
builder.Services.AddTransient<BlobServiceClient>
    (x => blobServiceClient);
builder.Services.AddTransient<ServiceStorageBlobs>();

builder.Services.AddDbContext<CubosContext>((serviceProvider, options) =>
{
    options.UseSqlServer(keyVaultSecrets.SqlConnectionString);
});

builder.Services.AddTransient<RepositoryCubos>();
builder.Services.AddTransient<RepositoryUsuarios>();


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
    app.MapOpenApi();

app.MapScalarApiReference();
app.MapGet("/", context =>
{
    context.Response.Redirect("/scalar");
    return Task.CompletedTask;
});
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
