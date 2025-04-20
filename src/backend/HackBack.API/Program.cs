using HackBack.API.ServiceRegistration;
using HackBack.Application.ServiceRegistration;
using HackBack.Infrastructure.ServiceRegistration;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
configuration.AddEnvironmentVariables();


await services
    .AddApi(configuration)
    .AddApplication(configuration)
    .AddInfrastructureAsync(configuration);

var app = builder.Build();


// ��� ���� ������ �� �����������
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

app.UseCors(policy => 
    policy.AllowAnyHeader()
        .AllowCredentials()
        .WithOrigins(configuration.GetSection("ALLOWED_CORS_HOSTS").Get<string>()!.Split(';')!) // ������ ���� ����� ��
);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHubs();

app.Run();