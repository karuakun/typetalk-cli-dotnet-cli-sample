using System.Threading.Tasks;
using GetTypetalkState.Typetalk.Request;

namespace GetTypetalkState.Typetalk
{
    public interface ITypeTalkConnection
    {
        Task<TResponse> GetAsync<TRequest, TResponse>(TRequest request)
            where TRequest : TypetalkApiRequest
            where TResponse : new();
        Task Login();
    }
}