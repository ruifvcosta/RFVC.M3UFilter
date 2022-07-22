using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RFVC.M3UFilter.API.Controllers
{
    [Route("api/m3u")]
    [ApiController]
    public class M3uPersonalFilterController : ControllerBase
    {
        readonly IWebHostEnvironment _environment;
        readonly IServiceProvider _services;
        readonly ILogger<M3uPersonalFilterController> _logger;
        private readonly IConfiguration _config;

        private string? PersonalPath { get; set; }

        public M3uPersonalFilterController(IWebHostEnvironment environment, IServiceProvider services, ILogger<M3uPersonalFilterController> logger, IConfiguration config)
        {
            _environment = environment;
            _services = services;
            _logger = logger;
            _config = config;

        }
     

        /// <summary>
        /// Returns the Personal list with pre created filters
        /// </summary>
        /// <param name="filter">sport | basic | extra -> Names for the filters </param>
        /// <param name="local"> True tu use a cached list, False to download</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Get(string filter, bool local = true)
        {

            PersonalPath = _config["AppSettings:PersonalUrl"];
          


            if (string.IsNullOrEmpty(PersonalPath))
                return NoContent();


            if (local)
            {
                _logger.LogInformation($"Get Local {filter}");
                string path;
                switch (filter.ToLower())
                {
                    case "sport":
                        path = System.IO.Path.Combine(_environment.ContentRootPath, "sport.m3u");
                        if (System.IO.File.Exists(path))
                        {
                            _logger.LogInformation($"Get  {path}");
                            return PhysicalFile(path, "audio/x-mpegurl");
                        }

                        break;

                    case "basic":
                        path = System.IO.Path.Combine(_environment.ContentRootPath, "basic.m3u");
                        if (System.IO.File.Exists(path))
                        {
                            _logger.LogInformation($"Get  {path}");
                            return PhysicalFile(path, "audio/x-mpegurl");
                        }
                        break;
                    case "extra":
                        path = System.IO.Path.Combine(_environment.ContentRootPath, "extra.m3u");
                        if (System.IO.File.Exists(path))
                        {
                            _logger.LogInformation($"Get  {path}");
                            return PhysicalFile(path, "audio/x-mpegurl");
                        }
                        break;
                    default:
                        path = System.IO.Path.Combine(_environment.ContentRootPath, "all.m3u");
                        if (System.IO.File.Exists(path))
                        {
                            _logger.LogInformation($"Get  {path}");
                            return PhysicalFile(path, "audio/x-mpegurl");
                        }
                        break;
                }
            }

            if (_services != null)
            {
                var m3ulogger = _services.GetService<ILogger<M3uFilterController>>();
                if (m3ulogger != null)
                {
                    var m3u = new M3uFilterController(m3ulogger);
                    var para = new M3uParameter()
                    {
                        Url = PersonalPath
                    };

                    switch (filter)
                    {
                        case "sport":
                            para.Groups = "PT Desporto|spor*";
                            break;
                        case "basic":
                            para.Groups = "PT Desporto|PT Cinema|spor*|(V|PT)*";
                            break;
                        case "extra":
                            para.Groups = "PT Desporto|PT Cinema|spor*|(V|PT)*|adult*";
                            break;

                    }
                    return await m3u.Get(para);
                }              
            }
            return NoContent();
        } 
    }
}
