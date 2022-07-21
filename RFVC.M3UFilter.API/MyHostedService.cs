using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RFVC.M3UFilter.API
{
    /// <summary>
    /// A Service that downloads and filters the personal list
    /// to save time on BIG LISTS
    /// Updates every 6 hours
    /// </summary>
    public class MyHostedService : IHostedService
    {
        readonly IWebHostEnvironment _environment;
        readonly ILogger _logger;
        private readonly IConfiguration _config;

        private string? PersonalPath { get; set; }
   
        public MyHostedService(IWebHostEnvironment environment, ILogger<MyHostedService> logger, IConfiguration config)
        {
            _environment = environment;
            _logger = logger;
            _config = config;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    string content = string.Empty;

                    PersonalPath = _config["AppSettings:PersonalUrl"];
                    if (string.IsNullOrEmpty(PersonalPath))
                        throw new ArgumentNullException("PersonalUrl");

                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(PersonalPath);
                        content = await response.Content.ReadAsStringAsync();
                    }


                    if (!string.IsNullOrEmpty(content))
                    {
                        // write to file to ContentRoot
                        var path = Path.Combine(_environment.ContentRootPath, "all.m3u");
                        File.WriteAllText(path, content, Encoding.UTF8);
                        _logger.LogInformation($" Created file {path}");

                        string parsedFile = RFVC.IPTV.M3u.M3uHelper.FilterM3uFileByGroup(content, new List<string>() { "PT Desporto",  "spor*"});
                        if (!string.IsNullOrEmpty(parsedFile))
                        {
                            path = Path.Combine(_environment.ContentRootPath, "sport.m3u");
                            File.WriteAllText(path, parsedFile, Encoding.UTF8);
                            _logger.LogInformation($" Created file {path}");
                        }

                        parsedFile = RFVC.IPTV.M3u.M3uHelper.FilterM3uFileByGroup(content, new List<string>() { "PT Desporto", "PT Cinema", "spor*", "(V|PT)*" });
                        if (!string.IsNullOrEmpty(parsedFile))
                        {
                            path = Path.Combine(_environment.ContentRootPath, "basic.m3u");
                            File.WriteAllText(path, parsedFile, Encoding.UTF8);
                            _logger.LogInformation($" Created file {path}");
                        }

                        parsedFile = RFVC.IPTV.M3u.M3uHelper.FilterM3uFileByGroup(content, new List<string>() { "PT Desporto", "PT Cinema", "spor*", "(V|PT)*","adult*"});
                        if (!string.IsNullOrEmpty(parsedFile))
                        {
                            path = Path.Combine(_environment.ContentRootPath, "extra.m3u");
                            File.WriteAllText(path, parsedFile, Encoding.UTF8);
                            _logger.LogInformation($" Created file {path}");
                        }
                        await Task.Delay(new TimeSpan(6, 0, 0)); //6 hours delay
                    }
                    else
                        await Task.Delay(new TimeSpan(0, 30, 0)); //30  min delay



                }
            }, cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

