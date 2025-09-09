using Azure.Storage.Blobs;

namespace Household.Web.Common.Helpers
{
    public sealed class BlobJsonWriter
    {
        private readonly BlobServiceClient _svc;
        public BlobJsonWriter(BlobServiceClient svc) => _svc = svc;

        public async Task WriteAsync(string path, object payload, CancellationToken ct = default)
        {
            var container = _svc.GetBlobContainerClient(path.Split('/')[0]);
            await container.CreateIfNotExistsAsync(cancellationToken: ct);
            var name = string.Join('/', path.Split('/')[1..]);
            var blob = container.GetBlobClient(name);
            using var ms = new MemoryStream(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(payload));
            await blob.UploadAsync(ms, overwrite: true, cancellationToken: ct);
        }
    }
}