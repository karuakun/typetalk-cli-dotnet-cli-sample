using System;

namespace GetTypetalkState.Typetalk.Response
{
    public class Post
    {
        public string Id { get; set; }
        public string TopicId { get; set; }
        public Account Account { get; set; }
        public string Message { get; set; }
        public Like[] Likes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
