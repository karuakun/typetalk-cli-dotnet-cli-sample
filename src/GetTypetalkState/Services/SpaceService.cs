using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GetTypetalkState.Typetalk;
using GetTypetalkState.Typetalk.Request;

namespace GetTypetalkState.Services
{
    public interface ISpaceService
    {
        Task<GetMySpaceResponse> GetMySpaces();
    }
    public class SpaceService: ISpaceService
    {
        private readonly ITypeTalkConnection _typetalkConnection;
        public SpaceService(ITypeTalkConnection typetalkConnection)
        {
            _typetalkConnection = typetalkConnection;
        }

        public async Task<GetMySpaceResponse> GetMySpaces()
        {
            return await _typetalkConnection.GetAsync<MySpaceRequest, GetMySpaceResponse>(new MySpaceRequest());
        }
    }
    #region Dtos
    public class MySpaceRequest : TypetalkApiRequest
    {
        public override string ApiName { get; set; } = "api/v1/spaces";
    }

    public class GetMySpaceResponse
    {
        public MySpace[] MySpaces { get; set; }
    }

    public class MySpace
    {
        public Typetalk.Response.Space Space { get; set; }
    }
    #endregion
}
