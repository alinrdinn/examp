using ExecutiveDashboard.Common;
using ExecutiveDashboard.Common.Extensions;
using ExecutiveDashboard.Common.Middlewares;
using ExecutiveDashboard.Modules;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAppLocalization();

// Add API Controllers with localization support
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddLocalizedValidationEnvelope();

builder.Services.AddDbContext<ExecutiveDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BenchmarkConnection"))
);

builder.Services.AddModules();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAppRequestLocalization();

app.UseCors();

app.UseAuthorization();

app.MapRazorPages();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();