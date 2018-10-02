using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GetTypetalkState.Services
{
    public interface IConfigService
    {
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string[] Spaces { get; set; }

        Task Set();
    }
    class ConfigService: IConfigService
    {
        private const string SettingFileName = "config.json";
        public string TypetalkApiUrl { get; set; } = "https://typetalk.com/";
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string[] Spaces { get; set; }
        public static async Task<IConfigService> Get()
        {
            if (!File.Exists(SettingFileName))
            {
                return new ConfigService();
            }
            var content = await File.ReadAllTextAsync(SettingFileName);
            return JsonConvert.DeserializeObject<ConfigService>(content);
        }

        public async Task Set()
        {
            var json = JsonConvert.SerializeObject(this);
            await File.WriteAllTextAsync(SettingFileName, json);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
