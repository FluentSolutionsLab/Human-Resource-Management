using HRManagement.Common.Application.Models;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Persistence;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = builder.Environment.ApplicationName, Version = "v1" });
});
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Configuration.AddEnvironmentVariables("ASPNETCORE_ENVIRONMENT");
builder.Services.AddMemoryCache();

// Add modules
builder.Services.AddModulePersonnelManagement();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{builder.Environment.ApplicationName} v1");
    });
    
    /*
     * Uncomment when you want to generate new data in Dev environment
     */
    // await app.Services.ModulePersonnelManagementDatabaseInitializer();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// For integration tests
public partial class Program
{ }