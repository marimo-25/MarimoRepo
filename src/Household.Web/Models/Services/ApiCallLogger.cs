namespace Household.Web.Models.Services
{
    public sealed class ApiCallLogger : IApiCallLogger
    {
        private readonly Household.Web.Common.Helpers.BlobJsonWriter _writer;
        public ApiCallLogger(Household.Web.Common.Helpers.BlobJsonWriter writer) => _writer = writer;

        public Task LogAsync(object log, CancellationToken ct) => _writer.WriteAsync($"api-logs/{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid()}.json", log, ct);
    }
}