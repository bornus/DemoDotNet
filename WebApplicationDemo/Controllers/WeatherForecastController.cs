using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationDemo.Dto;
using WebApplicationDemo.ViewModel;

namespace WebApplicationDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// API sur la route : monServeur/WeatherFocast/{Id}
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        public WeatherForecast GetById([FromRoute] IdWeatherForcastViewModel data)
        {
            var rng = new Random();
            return new WeatherForecast
            {
                Date = DateTime.Now.AddDays(data.Id),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            };
        }

        /// <summary>
        /// API sur la route : monServeur/WeatherFocast/{Id}/TemperatureC
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet("{Id}/TemperatureC")]
        public int GetTemperatureById([FromRoute] IdWeatherForcastViewModel data)
        {
            var rng = new Random();
            var ws = new WeatherForecast
            {
                Date = DateTime.Now.AddDays(data.Id),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            };

            return ws.TemperatureC;
        }

        /// <summary>
        /// API sur la route : monServeur/WeatherFocast/ verbe PUT
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut("{Id}/TemperatureC")]
        public int PutTemperatureCById([FromRoute] IdWeatherForcastViewModel data, [FromBody] WeatherForcastViewModel dataTemp)
        {
            var wf = new WeatherForecast
            {
                Date = dataTemp.Date,
                Summary = dataTemp.Summary,
                TemperatureC = dataTemp.TemperatureC
            };

            //ToDo => injection BBD

            return wf.Id;
        }

        /// <summary>
        /// API sur la route : monServeur/WeatherFocast/ verbe PUT
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut]
        public int Put([FromBody] WeatherForcastViewModel data)
        {
            var wf = new WeatherForecast
            {
                Date = data.Date,
                Summary = data.Summary,
                TemperatureC = data.TemperatureC
            };

            //ToDo => injection BBD

            return wf.Id;
        }
    }
}
