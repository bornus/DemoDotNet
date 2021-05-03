using ConsoleRasp.Configuration;
using ConsoleRasp.ViewModel;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Device.Gpio;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ConsoleRasp
{
    public class App
    {
        private static bool LedOn { get; set; }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly AppConfiguration config;
        private readonly ILogger<App> logger;

        public App(IOptions<AppConfiguration> config, ILogger<App> logger)
        {
            this.config = config.Value;
            this.logger = logger;
        }

        public void Run()
        {
            logger.LogDebug("Hello World!");
            logger.LogDebug("Blinking LED. Press Ctrl+C to end.");
            CancellationTokenSource canTok = new CancellationTokenSource();
            var blinkTask = Task.Run(() => BlinkLed(), canTok.Token);
            var readButtonTask = Task.Run(() => ReadBtn());

            readButtonTask.Wait();

            canTok.Cancel();
            blinkTask.Wait();
        }

        private void BlinkLed()
        {
            using var controller = new GpioController();
            controller.OpenPin(config.LedPin, PinMode.Output);
            while (true)
            {
                logger.LogDebug($"Led = {!LedOn}");
                controller.Write(config.LedPin, ((LedOn) ? PinValue.High : PinValue.Low));
                Thread.Sleep(500);
                LedOn = !LedOn;
            }
        }

        private void ReadBtn()
        {
            using var controller = new GpioController();
            controller.OpenPin(config.BtnPin, PinMode.Input);
            bool noHigh = true;
            bool callSuccess = false;
            while (noHigh || !callSuccess)
            {
                PinValue btnVal = controller.Read(config.BtnPin);
                logger.LogDebug($"Btn = {btnVal}");
                noHigh = btnVal == PinValue.Low;
                if (!noHigh)
                {
                    callSuccess = CallWebApi();
                }
                Thread.Sleep(200);
            }
        }

        private bool CallWebApi()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(config.UrlServer);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var rng = new Random();
            var wf = new WeatherForcastViewModel
            {
                Date = DateTime.Now.AddDays(1),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            };

            string weatherForcastJson = JsonConvert.SerializeObject(wf);

            logger.LogDebug($"Call Api Forcast {client.BaseAddress} with data : {weatherForcastJson}");
            HttpResponseMessage message = client.PostAsync("WeatherForcast", new StringContent(weatherForcastJson, Encoding.UTF8, "application/json")).Result;

            return message.IsSuccessStatusCode;
        }
    }
}
