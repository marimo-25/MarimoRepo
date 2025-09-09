using Microsoft.AspNetCore.Mvc;

namespace Household.Web.Controllers;

public class AccountController : Controller
{
    [HttpGet("/account/login")]
    public IActionResult Login() => View();
}