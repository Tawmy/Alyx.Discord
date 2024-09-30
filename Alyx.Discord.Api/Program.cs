using Alyx.Discord.Api.Extensions;
using Alyx.Discord.Bot;
using Alyx.Discord.Bot.HealthChecks;
using Alyx.Discord.Core;
using Alyx.Discord.Core.Extensions;
using Alyx.Discord.Core.HealthChecks;
using Alyx.Discord.Db;
using NetStone.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddGenericRequestHandlers();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var version = typeof(Program).Assembly.GetName().Version!;
builder.Services.AddSingleton(version);

builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddDbServices();
builder.Services.AddBotServices(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddDbContextCheck<DatabaseContext>()
    .AddCheck<BotHealthCheck>("bot")
    .AddCheck<NetStoneApiHealthCheck>("netstone");

builder.AddAuthentication();

var app = builder.Build();

await app.Services.MigrateDatabaseAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseHealthChecks();

app.MapControllers();

app.Logger.LogInformation("Alyx.Discord, Version {v}", version.ToVersionString());

app.Run();