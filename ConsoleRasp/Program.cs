using ConsoleRasp.ViewModel;
using Newtonsoft.Json;
using System;
using System.Device.Gpio;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleRasp
{
    class Program
    {
        private const int LED_PIN = 18;
        private const int BTN_PIN = 19;

        private static bool LedOn { get; set; }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Blinking LED. Press Ctrl+C to end.");
            CancellationTokenSource canTok = new CancellationTokenSource();
            var blinkTask = Task.Run(() => BlinkLed(), canTok.Token);
            var readButtonTask = Task.Run(() => ReadBtn());

            readButtonTask.Wait();

            canTok.Cancel();
            blinkTask.Wait();
        }


        static private void BlinkLed()
        {
            using var controller = new GpioController();
            controller.OpenPin(LED_PIN, PinMode.Output);
            while (true)
            {
                controller.Write(LED_PIN, ((LedOn) ? PinValue.High : PinValue.Low));
                Thread.Sleep(500);
            }
        }

        static private void ReadBtn()
        {
            using var controller = new GpioController();
            controller.OpenPin(BTN_PIN, PinMode.Input);
            bool noHigh = true;
            bool callSuccess = false;
            while (noHigh || !callSuccess)
            {
                PinValue btnVal = controller.Read(BTN_PIN);
                noHigh = btnVal == PinValue.Low;
                if (!noHigh)
                {
                    callSuccess = CallWebApi();
                }
                Thread.Sleep(200);
            }
        }

        static private bool CallWebApi()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:1664/");
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

            HttpResponseMessage message = client.PostAsync("WeatherForcast", new StringContent(weatherForcastJson, Encoding.UTF8, "application/json")).Result;

            return message.IsSuccessStatusCode;
        }
    }
}
