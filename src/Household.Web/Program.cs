using Household.Web.Models.Data;
using Household.Web.Models.Services;
using Household.Web.Common.Helpers;
using Azure.Identity;
using Azure.Storage.Blobs;
using Serilog;
using LinqToDB.Data;

var builder = WebApplication.CreateBuilder(args);

// Serilog (console)
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Host.UseSerilog();

// Config
var cs = builder.Configuration.GetConnectionString("Sql") ?? "Server=.;Database=Household;Trusted_Connection=True;TrustServerCertificate=True";

// linq2db
var opts = Household.Web.Models.Data.DbConnectionFactory.CreateOptions(cs);
builder.Services.AddSingleton(new HouseholdDb(opts));

// Blob
builder.Services.AddSingleton(new BlobServiceClient(new Uri(builder.Configuration["Azure:Storage:AccountUrl"] ?? "http://127.0.0.1"), new DefaultAzureCredential()));
builder.Services.AddSingleton<BlobJsonWriter>();

// Services
builder.Services.AddScoped<IApiCallLogger, ApiCallLogger>();
builder.Services.AddScoped<IDashboardQuery, DashboardQuery>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<Household.Web.Models.Data.Repositories.IPaymentRepository, Household.Web.Models.Data.Repositories.PaymentRepository>();
builder.Services.AddScoped<IHistoryWriter, HistoryWriter>();

builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication().AddCookie();
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();