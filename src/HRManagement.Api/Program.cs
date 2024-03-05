using HRManagement.BuildingBlocks.Caching;
using HRManagement.BuildingBlocks.Models;
using HRManagement.Modules.Staff;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var env = builder.Environment;
builder.Services.AddOptions<AppSettings>().Bind(configuration).ValidateDataAnnotations();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {Title = "HR Management API", Version = "v1"});
    c.EnableAnnotations();
});
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddMemoryCache();

// Add modules
builder.Services.AddStaffManagementModule();
builder.Services.AddInfrastructureServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
        options.DocExpansion(DocExpansion.None);
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HR Management API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// For integration tests
public partial class Program
{
}