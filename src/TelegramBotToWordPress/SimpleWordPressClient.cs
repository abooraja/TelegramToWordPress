using System;
using System.Threading.Tasks;
using WordPressSharp;
using WordPressSharp.Models;

namespace TelegramBotToWordPress
{
    class SimpleWordPressClient : ISimpleWordPressClient
    {
        private readonly string _author;
        private readonly string _parentId;
        private WordPressSiteConfig _wpConfig;
        WordPressClient _client;
        public SimpleWordPressClient(string baseUrl, string username, string password, int blogId, string author, string parentId)
        {
            _author = author;
            _parentId = parentId;
            _wpConfig = new WordPressSiteConfig()
            {
                BaseUrl = baseUrl,
                Username = username,
                Password = password,
                BlogId = blogId
            };
            _client = new WordPressSharp.WordPressClient(_wpConfig);
        }
        public async Task<int> SendForumPost(string content)
        {
            var p = new Post()
            {
                PostType = "reply",
                Status = "publish",
                //post_format = "standard",
                PublishDateTimeGmt = DateTime.UtcNow.AddMinutes(-5),
                ParentId = _parentId,
                Name = content,
                Content = content,
                Terms = new Term[0],
                Author = _author,
                Title = "",
            };

            var id = await _client.NewPostAsync(p);
            return Convert.ToInt32(id);
        }
    }
}
