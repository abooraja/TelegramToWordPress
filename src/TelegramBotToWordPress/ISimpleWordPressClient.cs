using System.Threading.Tasks;

namespace TelegramBotToWordPress
{
    interface ISimpleWordPressClient
    {
        Task<int> SendForumPost(string content);
    }
}