using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RFVC.M3UFilter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class M3uFilterController : ControllerBase
    {
        readonly ILogger<M3uFilterController> _logger;

        public M3uFilterController(ILogger<M3uFilterController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns a filtered M3uList with the groups that were choosen
        /// Needs a URL of the list ( usualy points to a API with user and password )
        /// Needs a list of words separated with |  to filter the groups
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] M3uParameter options)
        {

            try
            {
                Stream? stream = null;

                string composedUrl = Helpers.GenerateComposedUrl(options.Url, options.Password, options.Type, options.Output);


                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = System.TimeSpan.FromMinutes(10);
                    _logger.LogInformation($"Get:{composedUrl}");
                    using (var response = await httpClient.GetAsync(composedUrl))
                    {

                        string apiResponse = await response.Content.ReadAsStringAsync();

                        if (string.IsNullOrEmpty(apiResponse))
                        {
                            _logger.LogInformation($"Content not found:{composedUrl}");
                            return NotFound();
                        }
                        if (options.Groups == null)
                        {
                            stream = Helpers.GenerateStreamFromString(apiResponse);
                            _logger.LogInformation($"No Filters defined");
                        }
                        else
                        {
                            List<string> groups = options.Groups.ToLower().Split("|").ToList();
                            string parsedFile = RFVC.IPTV.M3u.M3uHelper.FilterM3uFileByGroup(apiResponse, groups);
                            if (string.IsNullOrEmpty(parsedFile) || parsedFile == "#EXTM3U\r\n")
                                stream = Helpers.GenerateStreamFromString(apiResponse);
                            else
                                stream = Helpers.GenerateStreamFromString(parsedFile);
                        }

                    }
                }

                if (stream == null)
                    return NotFound();

                return File(stream, "audio/x-mpegurl", "trimmedlist.m3u");
            }
            catch (System.Exception ex)
            {
                _logger.LogInformation(ex.ToString());
                return NotFound();
            }
        }
    }
}
