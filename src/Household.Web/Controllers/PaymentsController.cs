using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Household.Web.Models.Services;
using Household.Web.Common.Dtos.Payments;

namespace Household.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _svc;
    public PaymentsController(IPaymentService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<List<PaymentDto>>> List([FromQuery] PaymentQueryDto q, CancellationToken ct)
        => Ok(await _svc.ListAsync(q, ct));

    [Authorize(Roles="Admin")]
    [HttpPost("{id:long}/settle")]
    public async Task<IActionResult> Settle(long id, [FromHeader(Name="If-Match")] string? etag, CancellationToken ct)
    {
        var concurrency = string.IsNullOrEmpty(etag) ? null : Convert.FromBase64String(etag);
        await _svc.SettleAsync(id, User.Identity?.Name ?? "unknown", concurrency, ct);
        return NoContent();
    }

    [Authorize(Roles="Admin")]
    [HttpPost("{id:long}/cancel")]
    public async Task<IActionResult> Cancel(long id, [FromBody] CancelRequestDto req, [FromHeader(Name="If-Match")] string? etag, CancellationToken ct)
    {
        var concurrency = string.IsNullOrEmpty(etag) ? null : Convert.FromBase64String(etag);
        await _svc.CancelAsync(id, req.Reason ?? "", User.Identity?.Name ?? "unknown", concurrency, ct);
        return NoContent();
    }
}