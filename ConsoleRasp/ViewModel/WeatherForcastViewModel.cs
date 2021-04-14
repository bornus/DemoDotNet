using System;

namespace ConsoleRasp.ViewModel
{
    /// <summary>
    /// La classe de manipulation d'un weatherForcast
    /// </summary>
    public class WeatherForcastViewModel
    {

        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }
    }
}
