using System.Net;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using RedditReader.Models;

namespace RedditReader.Services;

public interface IService
{
    void NewPostsUpdated(object sender, PostsUpdateEventArgs e);
    IList<LightPost> GetTopVotedPosts();
    IList<TopAuthor> GetTopAuthors();
}