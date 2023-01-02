using HRManagement.Common.Domain;
using HRManagement.Modules.Personnel.Application;
using HRManagement.Modules.Personnel.Persistence;
using MediatR;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .Configure<AppSettings>(builder.Configuration)
    .AddScoped(sp => sp.GetRequiredService<IOptionsSnapshot<AppSettings>>().Value);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Configuration.AddEnvironmentVariables("ASPNETCORE_ENVIRONMENT");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.DefaultModelsExpandDepth(-1));
    using (var serviceScope = app.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;
        var personnelDbContext = services.GetRequiredService<PersonnelDbContext>();
        DataSeeder.SeedEmployees(personnelDbContext);
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();