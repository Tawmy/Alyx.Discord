using Alyx.Discord.Api.Components;
using Alyx.Discord.Api.Extensions;
using Alyx.Discord.Api.HealthChecks;
using Alyx.Discord.Bot;
using Alyx.Discord.Bot.HealthChecks;
using Alyx.Discord.Core;
using Alyx.Discord.Core.Extensions;
using Alyx.Discord.Core.HealthChecks;
using Alyx.Discord.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddGenericRequestHandlers();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var version = typeof(Program).Assembly.GetName().Version!;
builder.Services.AddSingleton(version);

builder.Services.AddDataProtection(builder.Configuration);

builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddDbServices();
builder.Services.AddBotServices(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddDbContextCheck<DatabaseContext>()
    .AddCheck<DiscordHealthCheck>("bot")
    .AddCheck<CharacterGetHealthCheck>("netstone")
    .AddCheck<DataProtectionCertificateHealthCheck>("cert");

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.AddAuthentication();

var app = builder.Build();

await app.Services.MigrateDatabaseAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", true);
}

app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthorization();
app.UseHealthChecks();

app.MapControllers();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Logger.LogInformation("Alyx.Discord, Version {v}", version.ToString(3));

app.Run();