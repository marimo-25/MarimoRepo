using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Household.Functions.Functions;

public class TokenFunctions
{
    [Function("Ping")]
    public async Task<HttpResponseData> Ping([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ping")] HttpRequestData req)
    {
        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteStringAsync("pong");
        return res;
    }
}