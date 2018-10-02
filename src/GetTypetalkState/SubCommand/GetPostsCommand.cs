using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GetTypetalkState.OuputLayout;
using GetTypetalkState.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace GetTypetalkState.SubCommand
{
    [Command(GetPostsCommand.CommandName)]
    public class GetPostsCommand : SubCommandBase
    {
        public const string CommandName = "getposts";
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

        public GetPostsCommand(
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
            layout.Output(response.Select(r => new
            {
                r.TopicId,
                PostId = r.Id,
                Url = $"https://typetalk.com/topics/{r.TopicId}/posts/{r.Id}",
                r.CreatedAt,
                AccoutName = r.Account.Name,
                Message = r.Message,
                LikesCount = r.Likes.Length
            }).OrderBy(p => p.CreatedAt));
            return 0;
        }
    }
}
