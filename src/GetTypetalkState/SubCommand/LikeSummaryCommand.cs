using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetTypetalkState.OuputLayout;
using GetTypetalkState.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace GetTypetalkState.SubCommand
{
    [Command(LikeSummaryCommand.CommandName)]
    public class LikeSummaryCommand : SubCommandBase
    {
        public const string CommandName = "likesummary";
        [Argument(0)]
        public string SpaceKey { get; set; }
        [Option("-t|--topic")]
        public string TopicId { get; set; }
        [Option("-from")]
        public string FromDate { get; set; }
        [Option("-to")]
        public string ToDateTime { get; set; }
        [Option("-l|--layout")]
        public string Layout { get; set; }


        private readonly ILayoutRepository _layoutRepository;
        private readonly ITopicService _topicService;
        private readonly ILogger _logger;

        public LikeSummaryCommand(
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
            if (!DateTime.TryParse(FromDate, out var fromDate))
                throw new InvalidOperationException();
            if (!DateTime.TryParse(ToDateTime, out var toDate))
                throw new InvalidOperationException();

            var response = await _topicService.Search(SpaceKey, TopicId, fromDate, toDate);
            var layout = _layoutRepository.Get(Layout);

            layout.Output(response.SelectMany(p => p.Likes.Select(l => l.Account.Name), (p, a) => a).GroupBy(a => a)
                .OrderByDescending(g => g.Count()).Select(g => new
                {
                    UserId = g.Key,
                    LikeCount = g.Count()
                }));
            return 0;
        }
    }
}
