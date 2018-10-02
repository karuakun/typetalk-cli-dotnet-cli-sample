using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GetTypetalkState.Typetalk.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace GetTypetalkState.Typetalk
{
    public class TypeTalkConnection : ITypeTalkConnection
    {
        private HttpClient _httpClient;
        private string _clientId;
        private string _clientSecret;
        private JsonSerializerSettings _jsonSerializedSettings;

        public static TypeTalkConnection Create(string baseAddress, string clientId, string clientSecret)
        {
            var hander = new HttpClientHandler
            {
                UseCookies = false
            };
            var connection =  new TypeTalkConnection
            {
                _clientId = clientId,
                _clientSecret = clientSecret,
                _httpClient = new HttpClient(hander)
                {
                    BaseAddress = new Uri(baseAddress),
                },
                _jsonSerializedSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            };
            return connection;
        }

        public async Task Login()
        {
            var response = await _httpClient.PostAsync("oauth2/access_token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                            { "client_id", _clientId },
                            { "client_secret", _clientSecret },
                            { "grant_type", "client_credentials" },
                            { "scope", "topic.read,my" }
                }));
            var authResponse = await response.Content.ReadAsStringAsync();
            var auth = JObject.Parse(authResponse);
            var accessToken = (string)auth["access_token"];
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        }

        public async Task<TResponse> GetAsync<TRequest, TResponse>(TRequest request)
            where TRequest: TypetalkApiRequest
            where TResponse: new()
        {
            var form = new Dictionary<string, string>();
            foreach (var p in typeof(TRequest).GetProperties())
            {
                var attr = p.GetCustomAttributes(typeof(TypetalkRequestParameterAttribute), false).FirstOrDefault() as TypetalkRequestParameterAttribute;
                var value = p.GetValue(request);
                if (attr == null)
                    continue;
                
                form.Add(attr.Name, value == null ? string.Empty: value.ToString());
            }

            var query = string.Empty;
            if (form.Any())
                query = "?" + string.Join("&", form.Select(f => $"{f.Key}={WebUtility.UrlEncode(f.Value)}"));

            var response = await _httpClient.GetAsync($"{request.ApiName}{query}");
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.StatusCode.ToString());

            var text = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(text, _jsonSerializedSettings);
        }
    }
}
