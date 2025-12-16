using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Household.Functions.Functions;

public class TokenFunctions
{
    private const int MaxRequestBodyLength = 1024; // 1KB

    [Function("Ping")]
    public async Task<HttpResponseData> Ping([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ping")] HttpRequestData req)
    {
        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteStringAsync("pong");
        return res;
    }

    [Function("GetToken")]
    public async Task<HttpResponseData> GetToken([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "token")] HttpRequestData req)
    {
        try
        {
            // リクエストボディの長さを検証
            if (req.Body.Length > MaxRequestBodyLength)
            {
                var errorRes = req.CreateResponse(HttpStatusCode.BadRequest);
                await errorRes.WriteAsJsonAsync(new { error = $"Request body is too large. Maximum size: {MaxRequestBodyLength} bytes." });
                return errorRes;
            }

            // リクエストボディを読み込み
            using (var reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                var body = await reader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(body))
                {
                    var errorRes = req.CreateResponse(HttpStatusCode.BadRequest);
                    await errorRes.WriteAsJsonAsync(new { error = "Request body cannot be empty." });
                    return errorRes;
                }

                // ここでトークンを生成またはデータベースから取得
                // 仮の実装として、UUIDを返す
                var token = Guid.NewGuid().ToString();

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new { token });
                return response;
            }
        }
        catch (JsonException)
        {
            var errorRes = req.CreateResponse(HttpStatusCode.BadRequest);
            await errorRes.WriteAsJsonAsync(new { error = "Invalid JSON in request body." });
            return errorRes;
        }
        catch (Exception ex)
        {
            var errorRes = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorRes.WriteAsJsonAsync(new { error = "An unexpected error occurred.", details = ex.Message });
            return errorRes;
        }
    }
}