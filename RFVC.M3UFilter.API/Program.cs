using RFVC.M3UFilter.API;
using Microsoft.Extensions.Hosting.WindowsServices;

//var builder = WebApplication.CreateBuilder(args);

//Point to the output path in order to work no the deployed host
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
});


// Add services to the container.
builder.Services.AddHostedService<MyHostedService>();
builder.Services.AddLogging();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseWindowsService();


// Allow any ip to a specific port
// Disabled https to use on local network without certificate
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // to listen for incoming http connection on port 5001
    //options.ListenAnyIP(5001, configure => configure.UseHttps()); // to listen for incoming https connection on port 5001
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Disabled https redirection to use on local network without certificate
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

