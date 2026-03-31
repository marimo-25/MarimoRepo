using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Household.Web.Models.Services;
using Household.Web.Common.Dtos.Payments;

namespace Household.Web.Controllers;

[Authorize]
[Route("Payments")]
public class PaymentsMvcController : Controller
{
    private readonly IPaymentService _svc;
    public PaymentsMvcController(IPaymentService svc) => _svc = svc;

    [HttpGet(""), HttpGet("Index")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var list = await _svc.ListAsync(new PaymentQueryDto(), ct);
        return View("~/Views/Payments/Index.cshtml", list);
    }

    [HttpGet("Create")]
    public IActionResult Create()
        => View("~/Views/Payments/Create.cshtml",
               new CreatePaymentDto { PaymentDate = DateOnly.FromDateTime(DateTime.Today) });

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromForm] CreatePaymentDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.StoreName) || dto.Amount <= 0 || string.IsNullOrWhiteSpace(dto.Payer))
        {
            ViewBag.Error = "全項目を正しく入力してください。";
            return View("~/Views/Payments/Create.cshtml", dto);
        }
        await _svc.CreateAsync(dto, User.Identity?.Name ?? "unknown", ct);
        return Redirect("/Payments");
    }

    [HttpPost("{id:long}/Settle")]
    public async Task<IActionResult> Settle(long id, CancellationToken ct)
    {
        await _svc.SettleAsync(id, User.Identity?.Name ?? "unknown", null, ct);
        return Redirect("/Payments");
    }

    [HttpPost("{id:long}/Cancel")]
    public async Task<IActionResult> Cancel(long id, [FromForm] string? reason, CancellationToken ct)
    {
        await _svc.CancelAsync(id, reason ?? "", User.Identity?.Name ?? "unknown", null, ct);
        return Redirect("/Payments");
    }
}
