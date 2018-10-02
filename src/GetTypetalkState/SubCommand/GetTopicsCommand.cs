using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GetTypetalkState.OuputLayout;
using GetTypetalkState.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace GetTypetalkState.SubCommand
{
    [Command(GetTopicsCommand.CommandName)]
    public class GetTopicsCommand: SubCommandBase
    {
        public const string CommandName = "gettopic";

        [Argument(0)]
        public string SpaceKey { get; set; }

        [Option("-l|--layout")]
        public string Layout { get; set; }


        private readonly ILayoutRepository _layoutRepository;
        private readonly ITopicService _topicService;
        private readonly ILogger _logger;

        public GetTopicsCommand(
            ILayoutRepository layoutRepository,
            ITopicService topicService,
            ILoggerFactory loggerFactory)
        {
            _layoutRepository = layoutRepository;
            _topicService = topicService;
            _logger = loggerFactory.CreateLogger<GetTopicsCommand>();
        }
        public override async Task<int> OnExecute(CommandLineApplication app)
        {
            var response = await _topicService.GetTopics(SpaceKey);
            var layout = _layoutRepository.Get(Layout);
            layout.Output(response.Topics.Select(t => new
            {
                t.Topic.Id,
                t.Topic.Name,
                t.Topic.Description
            }));
            return 0;
        }
    }
}
