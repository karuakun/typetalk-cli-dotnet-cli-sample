using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetTypetalkState.Typetalk.Request;
using GetTypetalkState.Typetalk;
using GetTypetalkState.Typetalk.Response;
using Newtonsoft.Json;

namespace GetTypetalkState.Services
{
    public interface ITopicService
    {
        Task<TopicService.GetTopicsResopnse> GetTopics(string spaceKey);
        Task<IEnumerable<Post>> Search(string spaceKey, string topicId, DateTime fromDate, DateTime toDate);
    }

    public class TopicService : ITopicService
    {
        private readonly ITypeTalkConnection _typetalkConnection;

        public TopicService(ITypeTalkConnection typetalkConnection)
        {
            _typetalkConnection = typetalkConnection;
        }

        public async Task<GetTopicsResopnse> GetTopics(string spaceKey)
        {
            return await _typetalkConnection.GetAsync<GetTopicsRequest, GetTopicsResopnse>(new GetTopicsRequest
                {SpaceKey = spaceKey});
        }

        public async Task<IEnumerable<Post>> Search(string spaceKey, string topicId, DateTime fromDate, DateTime toDate)
        {
            var foundPosts = await _typetalkConnection.GetAsync<SearchRequest, PostResponse>(new SearchRequest
            {
                Query = string.Empty,
                SpaceKey = spaceKey,
                TopickId = topicId,
                FromDate = fromDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                ToDate = toDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            });
            var newestPost = foundPosts.Posts.FirstOrDefault(p => p.Id == foundPosts.Posts.Max(_ => _.Id));
            if (newestPost == null)
                return null;

            // 検索の始点から Forward して、toDateまで取得する
            var foundsPost = new List<Post>();
            {
                var fromPostId = newestPost.Id;
                while (true)
                {
                    var posts = await _typetalkConnection.GetAsync<GetTopicRequest, PostResponse>(new GetTopicRequest
                    {
                        ApiName = $"api/v1/topics/{topicId}",
                        FromPostId = fromPostId
                    });


                    var rangePosts = posts.Posts.Where(p => fromDate <= p.CreatedAt && p.CreatedAt < toDate).ToList();
                    if (!rangePosts.Any())
                        break;
                    foundsPost.AddRange(rangePosts);
                    fromPostId = rangePosts.Min(p => p.Id);
                }
            }
            return foundsPost;
        }

        #region Dtos

        public class GetTopicRequest : TypetalkApiRequest
        {
            public override string ApiName { get; set; } = "api/v1/topics/{0}";

            [TypetalkRequestParameter(Name = "count")]
            public string Count { get; set; } = "200";

            [TypetalkRequestParameter(Name = "direction")]
            public string Direction { get; set; } = "backward";

            [TypetalkRequestParameter(Name = "from")]
            public string FromPostId { get; set; }
        }


        public class SearchRequest : TypetalkApiRequest
        {
            public override string ApiName { get; set; } = "api/v2/search/posts";
            [TypetalkRequestParameter(Name = "q")] public string Query { get; set; }

            [TypetalkRequestParameter(Name = "spaceKey")]
            public string SpaceKey { get; set; }

            [TypetalkRequestParameter(Name = "topicIds")]
            public string TopickId { get; set; }

            [TypetalkRequestParameter(Name = "from")]
            public string FromDate { get; set; }

            [TypetalkRequestParameter(Name = "to")]
            public string ToDate { get; set; }

        }

        public class GetTopicsRequest : TypetalkApiRequest
        {
            public override string ApiName { get; set; } = "api/v2/topics";

            [TypetalkRequestParameter(Name = "spaceKey")]
            public string SpaceKey { get; set; }
        }

        public class GetTopicsResopnse
        {
            public TopicResponse[] Topics { get; set; }
        }

        public class TopicResponse
        {
            public Topic Topic { get; set; }
        }

        public class PostResponse
        {
            public Post[] Posts { get; set; }
        }

        #endregion
    }

}
