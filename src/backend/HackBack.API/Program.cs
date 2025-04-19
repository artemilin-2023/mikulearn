using HackBack.API.ServiceRegistration;
using HackBack.Application.ServiceRegistration;
using HackBack.Infrastructure.ServiceRegistration;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

await services
    .AddApi(configuration)
    .AddApplication(configuration)
    .AddInfrastructureAsync(configuration);

var app = builder.Build();

// мне лень делать по нормальному
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<DbContext>();
await db.Database.MigrateAsync();

app.UseResultSharpLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHubs();

app.Run();