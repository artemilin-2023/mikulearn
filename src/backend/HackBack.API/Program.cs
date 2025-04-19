using HackBack.API.Extensions;
using HackBack.Application.ServiceRegistration;
using HackBack.Infrastructure.ServiceRegistration;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddGraphQL();
services.AddEndpointsApiExplorer();
services.AddCustomSwaggerGen();
services.AddHttpContextAccessor();

services.AddAuthentificationRules(configuration);
services.AddAuthorizationPermissionRequirements();

services
    .AddApplication()
    .AddInfrastructure(configuration);

var app = builder.Build();

// мне лень делать по нормальному
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<DbContext>();
await db.Database.MigrateAsync();

app.UseResultSharpLogging();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGraphQL();

app.Run();