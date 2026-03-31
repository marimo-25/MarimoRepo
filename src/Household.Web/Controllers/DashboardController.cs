using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Household.Web.Models.Services;
using Household.Web.Common.Dtos.Dashboard;

namespace Household.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardQuery _query;
    public DashboardController(IDashboardQuery query) => _query = query;

    [HttpGet("weekly")]
    public async Task<ActionResult<WeeklySummaryDto>> Weekly(CancellationToken ct)
        => Ok(await _query.GetWeeklyAsync(ct));
}

[Authorize]
[Route("")]
[Route("Dashboard")]
public class DashboardControllerMvc : Controller
{
    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index() => View("~/Views/Dashboard/Index.cshtml");
}