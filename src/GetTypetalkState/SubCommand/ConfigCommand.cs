using System;
using System.Linq;
using System.Threading.Tasks;
using GetTypetalkState.Services;
using McMaster.Extensions.CommandLineUtils;

namespace GetTypetalkState.SubCommand
{
    [Command(ConfigCommand.CommandName)]
    public class ConfigCommand: SubCommandBase
    {
        public const string CommandName = "config";

        [Option("--clientId")]
        public string ClientId { get; set; }
        [Option("--clientSecret")]
        public string ClientSecret { get; set; }
        [Option("--spaces")]
        public string Spaces { get; set; }

        private readonly IConfigService _configService;
        public ConfigCommand(IConfigService configService)
        {
            _configService = configService;
        }


        public override async Task<int> OnExecute(CommandLineApplication app)
        {
            if (!string.IsNullOrEmpty(ClientId))
            {
                _configService.ClientId = ClientId;
            }
            if (!string.IsNullOrEmpty(ClientSecret))
            {
                _configService.ClientSecret = ClientSecret;
            }
            if (Spaces != null && Spaces.Any())
            {
                _configService.Spaces = Spaces.Split(",");
            }

            await _configService.Set();
            ShowConfig();
            return 0;
        }

        private void ShowConfig()
        {
            Console.WriteLine(_configService);
        }
      
    }
}
