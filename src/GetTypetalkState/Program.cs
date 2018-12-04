using System;
using System.IO;
using System.Threading.Tasks;
using GetTypetalkState.OuputLayout;
using GetTypetalkState.Services;
using GetTypetalkState.SubCommand;
using GetTypetalkState.Typetalk;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace GetTypetalkState
{
    [Command("typetalk")]
    [Subcommand(ConfigCommand.CommandName, typeof(ConfigCommand))] // 設定
    [Subcommand(GetSpacesCommand.CommandName, typeof(GetSpacesCommand))] // スペースの取得
    [Subcommand(GetTopicsCommand.CommandName, typeof(GetTopicsCommand))] // トピックの取得
    [Subcommand(LikedSummaryCommand.CommandName, typeof(LikedSummaryCommand))] // いいねを受けたひと
    [Subcommand(LikeSummaryCommand.CommandName, typeof(LikeSummaryCommand))] // いいねをつけたひと
    [Subcommand(GetPostCommand.CommandName, typeof(GetPostCommand))] // ポストの取得
    [Subcommand(LikedPostSummaryCommand.CommandName, typeof(LikedPostSummaryCommand))] // 良いねが多かったポスト
    [Subcommand(TalksSummaryCommand.CommandName, typeof(TalksSummaryCommand))]

    partial class Program
    {
        public static async Task<int> Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (source, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                Log.Logger.Error(ex, "CurrentDomain.UnhandledException");
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Log.Logger.Error(e.Exception, "UnobservedTaskException");
            };

            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(await ConfiureServiceCollection());
            return app.Execute(args);
        }

        /// <summary>
        /// サービスコンテナを初期化します。
        /// </summary>
        /// <returns></returns>
        public static async Task<ServiceProvider> ConfiureServiceCollection()
        {
           var services = new ServiceCollection();
            ConfigureLogger(services);
            await ConfigureServices(services);
            return services.BuildServiceProvider();
        }

        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            app.ShowHelp();
            return 1;
        }

        public static void ConfigureLogger(ServiceCollection services)
        {
            services.AddSingleton(new LoggerFactory()
                .AddConsole()
                .AddSerilog()
            );
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .CreateLogger();
        }
        /// <summary>
        /// サービスをコンテナに登録します。
        /// </summary>
        /// <param name="services"></param>
        private static async Task ConfigureServices(ServiceCollection services)
        {
            string clientSecret;
            string clientId;
            { 
                IConfigService service = await ConfigService.Get();
                clientId = service.ClientId;
                clientSecret = service.ClientSecret;
                services.AddSingleton(provider => service);
            }

            {
                ITypeTalkConnection service = TypeTalkConnection.Create("https://typetalk.com", clientId, clientSecret);
                await service.Login();
                services.AddSingleton(provider => service);
            }

            services.AddSingleton<ILayoutRepository>(new LayoutRepository());
            services.AddTransient<ISpaceService, SpaceService>();
            services.AddTransient<ITopicService, TopicService>();
        }
    }
}
