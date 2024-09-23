using Alyx.Discord.Bot;
using Alyx.Discord.Core;
using NetStone.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var version = typeof(Program).Assembly.GetName().Version!;
builder.Services.AddSingleton(version);
builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddBotServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Logger.LogInformation("Alyx.Discord, Version {v}", version.ToVersionString());

app.Run();