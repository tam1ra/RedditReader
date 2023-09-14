using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using RedditReader.Models;

namespace RedditReader.Services;

public class MemoryService : IService
{
    private const int LIMIT = 5;

    private Dictionary<string, int> UpVotes = new();
    private PriorityQueue<string, int> TopVoted = new(Comparer<int>.Create((x, y) => y - x));
    private Dictionary<string, int> UserPosts = new();
    private PriorityQueue<string, int> TopPosted = new(Comparer<int>.Create((x, y) => y - x));

    public MemoryService()
    {
        
    }

    public void NewPostsUpdated(object sender, PostsUpdateEventArgs e)
    {
        foreach (Post post in e.Added)
        {
        }

        CalcTopVotedPosts(e.Added);
        CalcTopAuthors(e.Added);
    }

    private void CalcTopVotedPosts(IList<Post> posts)
    {
        
    }

    private void CalcTopAuthors(IList<Post> posts)
    {
        
    }

    public IList<LightPost> GetTopVotedPosts(IList<Post> posts)
    {
        return new List<LightPost>();
    }

    public IList<TopAuthor> GetTopAuthors(IList<Post> posts)
    {
        return new List<TopAuthor>();
    }
}