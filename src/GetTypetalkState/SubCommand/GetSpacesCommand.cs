using System.Linq;
using System.Threading.Tasks;
using GetTypetalkState.OuputLayout;
using GetTypetalkState.Services;
using McMaster.Extensions.CommandLineUtils;

namespace GetTypetalkState.SubCommand
{
    [Command(GetSpacesCommand.CommandName)]
    public class GetSpacesCommand : SubCommandBase
    {
        public const string CommandName = "getspace";

        [Option("-l|--layout")]
        public string Layout { get; set; }
       
        private readonly ILayoutRepository _layoutRepository;
        private readonly ISpaceService _spaceService;

        public GetSpacesCommand(
            ILayoutRepository layoutRepository,
            ISpaceService spaceService)
        {
            _layoutRepository = layoutRepository;
            _spaceService = spaceService;
        }

        public override async Task<int> OnExecute(CommandLineApplication app)
        {
            var response = await _spaceService.GetMySpaces();
            var layout = _layoutRepository.Get(Layout);
            layout.Output(response.MySpaces.Select(s => new
            {
                s.Space.Key,
                s.Space.Name
            }));
            return 0;
        }
    }
}
