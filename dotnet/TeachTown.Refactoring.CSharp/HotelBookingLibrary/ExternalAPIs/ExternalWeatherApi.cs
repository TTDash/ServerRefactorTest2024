namespace HotelReservationLibrary.ExternalAPIs
{
    // ASSUME THIS AN EXTERNAL API THAT IS IMMUTABLE

    public sealed class WeatherForecast
    {
        public DateOnly Date { get; set; }
        public int TemperatureC { get; set; }
        public string? Summary { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }

    public sealed class ExternalWeatherApi : IWeatherApi
    {
        private WeatherForecast? _forecast;

        public WeatherForecast GetForecast(DateOnly startDate, DateOnly endDate)
        {
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            _forecast = Enumerable.Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date = startDate.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                })
            .First();

            return _forecast;
        }
    }

    public interface IWeatherApi
    {
        WeatherForecast GetForecast(DateOnly startDate, DateOnly endDate);
    }
}
