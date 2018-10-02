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
    [Subcommand(ConfigCommand.CommandName, typeof(ConfigCommand))]
    [Subcommand(GetSpacesCommand.CommandName, typeof(GetSpacesCommand))]
    [Subcommand(GetTopicsCommand.CommandName, typeof(GetTopicsCommand))]
    [Subcommand(LikedSummaryCommand.CommandName, typeof(LikedSummaryCommand))]
    [Subcommand(GetPostsCommand.CommandName, typeof(GetPostsCommand))]
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


        //static async Task Main(string[] args)
        //{
        //    var clientId = "JVc7wcPOhZBYbeNDVfMaaIChG3yaEubQ";
        //    var clientSecret = "xVeBwr8WW8eX8DoFSbJR4v6lsnmehOhkU2Zau6tboUMiTovRP3qksJg5eSayrANp";
        //    var spaceKey = "LGgwv2CGGn";
        //    var topickId = "40377";
        //    var fromSpan = new DateTime(2018, 8, 1);
        //    var toSpan = new DateTime(2018, 9, 1);


        //    var httpClient = new HttpClient();
        //    var jsonSerializedSettings = new JsonSerializerSettings
        //    {
        //        ContractResolver = new CamelCasePropertyNamesContractResolver()
        //    };

        //    { 
        //        var response = await httpClient.PostAsync("https://typetalk.com/oauth2/access_token",
        //            new FormUrlEncodedContent(new Dictionary<string, string>
        //            {
        //                { "client_id", clientId },
        //                { "client_secret", clientSecret },
        //                { "grant_type", "client_credentials" },
        //                { "scope", "topic.read,my" }
        //            }));
        //        var authResponse = await response.Content.ReadAsStringAsync();
        //        var auth = JObject.Parse(authResponse);
        //        var accessToken = (string)auth["access_token"];
        //        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        //    }

        //    var newestPost = await FindNewestPost(spaceKey, topickId, fromSpan, toSpan, httpClient, jsonSerializedSettings);

        //    // 検索の始点から Forward して、ToSpanまで取得する
        //    var foundsPost = new List<Post>();
        //    {
        //        var fromPostId = newestPost.Id;
        //        while (true)
        //        {
        //            var response = await httpClient.GetAsync($"https://typetalk.com/api/v1/topics/{topickId}?count=200&direction=backward&from={fromPostId}");
        //            var postsResponse = await response.Content.ReadAsStringAsync();
        //            var posts = JsonConvert.DeserializeObject<PostResponse>(postsResponse);
        //            var rangePosts = posts.Posts.Where(p => fromSpan <= p.CreatedAt && p.CreatedAt < toSpan).ToList();
        //            if (!rangePosts.Any())
        //                break;
        //            foundsPost.AddRange(rangePosts);
        //            fromPostId = rangePosts.Min(p => p.Id);
        //        }
        //    }

        //    Console.WriteLine($"{fromSpan}～{toSpan}の集計");
        //    foreach (var g in foundsPost.GroupBy(p => p.Account.Name).OrderByDescending(g => g.Count()))
        //    {
        //        Console.WriteLine($"{g.Key} post:{g.Count()}, liked:{g.Sum(p => p.Likes.Length)}");
        //    }

        //    Console.ReadLine();
        //}

        //private static async Task<Post> FindNewestPost(string spaceKey, string topickId, DateTime fromSpan, DateTime toSpan, HttpClient httpClient, JsonSerializerSettings jsonSerializedSettings)
        //{
        //    var request = new FormUrlEncodedContent(new Dictionary<string, string>
        //    {
        //        {"q", string.Empty},
        //        {"spaceKey", spaceKey},
        //        {"topicIds", topickId},
        //        {"from", fromSpan.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")},
        //        {"to", toSpan.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") }
        //    });
        //    var response = await httpClient.GetAsync($"https://typetalk.com/api/v2/search/posts?{await request.ReadAsStringAsync()}");
        //    if (!response.IsSuccessStatusCode)
        //        return null;
        //    var searchReponse = await response.Content.ReadAsStringAsync();
        //    var foundPosts = JsonConvert.DeserializeObject<PostResponse>(searchReponse, jsonSerializedSettings);
        //    if (!foundPosts.Posts.Any())
        //        return null;
        //    return foundPosts.Posts.FirstOrDefault(p => p.Id == foundPosts.Posts.Max(_ => _.Id));
        //}
    }
}
