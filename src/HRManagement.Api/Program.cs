using HRManagement.Common.Application.Models;
using HRManagement.Common.Infrastructure;
using HRManagement.Modules.Personnel.Persistence;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

configuration.AddEnvironmentVariables("ASPNETCORE_ENVIRONMENT");

builder.Services.Configure<AppSettings>(configuration);
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "HR Management API", Version = "v1" });
});
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddMemoryCache();

// Add modules
builder.Services.AddModulePersonnelManagement();
builder.Services.AddInfrastructureServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HR Management API v1");
    });
    
    await app.Services.DatabaseInitializer(configuration);
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// For integration tests
public partial class Program
{ }