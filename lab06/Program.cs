
using Newtonsoft.Json.Linq;
class Program
{
    const string APIKEY = "837f2b24052035874abca8158212a5c4";
    static async Task Main()
    {
        List<Weather> weathers = new List<Weather>();
        Random random = new Random();
        using (HttpClient client = new HttpClient())
        {
            while (weathers.Count < 10)
            {
                double shirota = random.NextDouble() * 180 - 90; //широта от -90 до 90
                double dolgota = random.NextDouble() * 360 - 180; //долгота от -180 до 180
                string path = $"https://api.openweathermap.org/data/2.5/weather?lat={shirota}&lon={dolgota}&appid={APIKEY}";
                Weather weather;
                using var response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    var dataString = await response.Content.ReadAsStringAsync();
                    var data = JObject.Parse(dataString);
                    weather = new Weather();
                    weather.Country = (string)data["sys"]["country"];
                    weather.Name = (string)data["name"];
                    weather.Temp = Convert.ToDouble(data["main"]["temp"]) - 273.15;
                    weather.Description = (string)data["weather"][0]["description"];
                    if (string.IsNullOrEmpty(weather.Country) || string.IsNullOrEmpty(weather.Name)) { continue; }
                    weathers.Add(weather);
                }
            }
        }

        Weather maxTemp = weathers.Aggregate((max, next) =>
        {
            if (next.Temp > max.Temp)
            {
                return next;
            }
            else
            {
                return max;
            }
        });
        Weather minTemp = weathers.Aggregate((min, next) =>
        {
            if (next.Temp < min.Temp)
            {
                return next;
            }
            else
            {
                return min;
            }
        });
        Console.WriteLine($"\tmin temp: {minTemp.Country}");
        Console.WriteLine($"\tmax temp: {maxTemp.Country}");
        double averageTemp = weathers.Average(another => another.Temp);
        Console.WriteLine($"average temperature:  {averageTemp:F2} °C");
        int countries = weathers.GroupBy(another => another.Country).Count();
        Console.WriteLine($"amount countries in collection:  {countries}");

        Weather country_list = weathers.SkipWhile(point => point.Description != "clear sky" && point.Description != "rain" && point.Description != "few clouds").FirstOrDefault();
        Console.WriteLine(country_list);
    }

    public struct Weather
    {
        public string Country { get; set; }
        public string Name { get; set; }
        public double Temp { get; set; }
        public string Description { get; set; }
        public override string ToString() => $"Country: {Country}, Name: {Name}, Temp: {Temp}, Description: {Description}";
    }
}