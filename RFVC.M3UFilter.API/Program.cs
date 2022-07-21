using RFVC.M3UFilter.API;
using Microsoft.Extensions.Hosting.WindowsServices;

//var builder = WebApplication.CreateBuilder(args);

//Point to the output path in order to work no the deployed host
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
});

// Add services to the container.
builder.Services.AddHostedService<MyHostedService>();
builder.Services.AddLogging();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseWindowsService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
