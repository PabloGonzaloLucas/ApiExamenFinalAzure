using ApiExamenFinalAzure.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace ApiExamenFinalAzure.Services
{
    public class ServiceStorageBlobs
    {
        private readonly BlobServiceClient client;
        public ServiceStorageBlobs(BlobServiceClient client)
        {
            this.client = client;
        }

        //METODO PARA RECUPERAR TODOS LOS CONTAINERS
        public async Task<List<string>> GetContainersAsync()
        {
            List<string> containers = new List<string>();
            await foreach (BlobContainerItem container in this.client.GetBlobContainersAsync())
            {
                containers.Add(container.Name);
            }
            return containers;
        }
        //METODO PARA CREAR UN CONTAINER
        public async Task CreateContainerAsync(string containerName)
        {
            await this.client.CreateBlobContainerAsync(containerName.ToLower(),
                PublicAccessType.None);
        }
        //ELIMINAR UN CONTAINER
        public async Task DeleteContainerAsync(string containerName)
        {
            await this.client.DeleteBlobContainerAsync(containerName);
        }
        //LISTADO DE FICHEROS DENTRO DE UN CONTAINER
        public async Task<List<BlobModel>> GetBlobsAsync(string containerName)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            List<BlobModel> models = new List<BlobModel>();
            await foreach (BlobItem item in containerClient.GetBlobsAsync())
            {
                BlobModel blob = new BlobModel();
                blob.Nombre = item.Name;
                blob.Container = containerName;
                blob.Url = this.GetBlobUrl(containerName, item.Name);
                models.Add(blob);
            }
            return models;
        }

        //ELIMINAR UN BLOB
        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient blobContainerClient =
                this.client.GetBlobContainerClient(containerName);
            await blobContainerClient.DeleteBlobAsync(blobName);

        }
        //SUBIR UN BLOB A UN CONTAINER
        public async Task UploadBlobAsync(string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);

        }
        public async Task<Stream> GetBlobStreamAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            // Descargamos la información y obtenemos el stream del fichero
            BlobDownloadInfo response = await blobClient.DownloadAsync();
            return response.Content;
        }

        public string GetBlobUrl(string containerName, string blobName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            if (blobClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = containerName,
                    BlobName = blobName,
                    Resource = "b",
                    StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);
                return blobClient.GenerateSasUri(sasBuilder).AbsoluteUri;
            }

            return blobClient.Uri.AbsoluteUri;
        }

        public string GetContainerUrl(string containerName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);

            return containerClient.Uri.AbsoluteUri;
        }
    }
}
