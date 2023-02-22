using Carter;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Persistence;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddPersistenceServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Configuration.AddEnvironmentVariables("ASPNETCORE_ENVIRONMENT");
builder.Services.AddCarter();
builder.Services.AddSwaggerGen(c => c.TagActionsBy(d => new List<string> {d.ActionDescriptor.DisplayName!}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.DefaultModelsExpandDepth(-1));
    using (var serviceScope = app.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;
        var personnelDbContext = services.GetRequiredService<PersonnelDbContext>();
        DatabaseInitializer.Initialize(personnelDbContext);
    }
}

app.MapCarter();

app.Run();