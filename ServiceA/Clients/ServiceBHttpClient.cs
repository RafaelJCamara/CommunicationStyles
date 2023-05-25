using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceA.Clients
{
    public class ServiceBHttpClient
    {
        private readonly HttpClient httpClient;

        public ServiceBHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<WeatherForecast>> GetWfAsync()
        {
            var items = await httpClient.GetFromJsonAsync<IReadOnlyCollection<WeatherForecast>>("/WeatherForecast");
            return items;
        }
    }
}