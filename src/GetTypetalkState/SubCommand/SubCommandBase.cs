using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace GetTypetalkState.SubCommand
{
    public abstract class SubCommandBase
    {
        public abstract Task<int> OnExecute(CommandLineApplication app);
    }
}
