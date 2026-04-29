namespace ApiExamenFinalAzure.Models
{
    public class KeyVaultAccesorModel
    {
        public string StorageAccountConnectionString { get; set; } = string.Empty;
        public string BlobsBaseUrl { get; set; } = string.Empty;
        public string CypherKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SqlConnectionString { get; set; } = string.Empty;

    }
}
