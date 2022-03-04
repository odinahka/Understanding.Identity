using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Understanding.WebApp.Identity.Authorization;
using Understanding.WebApp.Identity.DTO;

namespace Understanding.Identity.Pages
{
    [Authorize(Policy ="HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;
        [BindProperty]
        public List<WeatherForecastDTO> WeatherForecasts { get; set; }
        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task OnGet()
        {

            WeatherForecasts = await InvokeEndpoint<List<WeatherForecastDTO>>("OurWebAPI", "WeatherForecast");
        }

        private async Task<JwtToken> Authenticate(HttpClient httpClient)
        {
            var response = await httpClient.PostAsJsonAsync("auth", new Credential { UserName = "admin", Password = "password" });
            response.EnsureSuccessStatusCode();
            string strJwt = await response.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("access_token", strJwt);
            return JsonConvert.DeserializeObject<JwtToken>(strJwt);
        }
        private async Task<T> InvokeEndpoint<T>(string clientName, string url)
        {
            // Get token from session
            JwtToken token = null;

            var strTokenObj = HttpContext.Session.GetString("access_token");
            var httpClient = httpClientFactory.CreateClient(clientName);
            if (string.IsNullOrWhiteSpace(strTokenObj)) token = await Authenticate(httpClient);

            else token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj);

            if (token == null || string.IsNullOrWhiteSpace(token.AccessToken) || token.ExpiresAt <= DateTime.UtcNow) await Authenticate(httpClient);

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);

            return await httpClient.GetFromJsonAsync<T>(url);
        }
    }
}
