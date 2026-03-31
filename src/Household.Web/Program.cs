using Household.Web.Models.Data;
using Household.Web.Models.Data.Repositories;
using Household.Web.Models.Services;
using Household.Web.Models.Mock;
using Household.Web.Common.Helpers;
using Azure.Identity;
using Azure.Storage.Blobs;
using Serilog;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Serilog (console)
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Host.UseSerilog();

// --- 依存切り替え ---
// appsettings.json の "UseMock": true でモック起動、false で実 DB/Blob を使用
var useMock = builder.Configuration.GetValue<bool>("UseMock", defaultValue: true);

if (useMock)
{
    builder.Services.AddSingleton<IPaymentRepository, MockPaymentRepository>();
    builder.Services.AddSingleton<IHistoryWriter, MockHistoryWriter>();
    builder.Services.AddSingleton<IDashboardQuery, MockDashboardQuery>();
    builder.Services.AddScoped<IApiCallLogger, NullApiCallLogger>();
}
else
{
    // SQL Server 接続
    var cs = builder.Configuration.GetConnectionString("Sql")
        ?? "Server=localhost;Database=Household;Trusted_Connection=True;TrustServerCertificate=True";
    var dbOpts = DbConnectionFactory.CreateOptions(cs);
    builder.Services.AddSingleton(new HouseholdDb(dbOpts));
    builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
    builder.Services.AddScoped<IHistoryWriter, HistoryWriter>();
    builder.Services.AddScoped<IDashboardQuery, DashboardQuery>();

    // Azure Blob
    var blobUrl = builder.Configuration["Azure:Storage:AccountUrl"]
        ?? throw new InvalidOperationException("Azure:Storage:AccountUrl is required when UseMock=false.");
    builder.Services.AddSingleton(new BlobServiceClient(new Uri(blobUrl), new DefaultAzureCredential()));
    builder.Services.AddSingleton<BlobJsonWriter>();
    builder.Services.AddScoped<IApiCallLogger, ApiCallLogger>();
}

builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o => { o.LoginPath = "/account/login"; });
builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
