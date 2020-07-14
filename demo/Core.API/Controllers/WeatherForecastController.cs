using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Core.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                //用户取消了请求
                cancellationToken.ThrowIfCancellationRequested();
            }
            return new List<string>();
        }
    }
}
