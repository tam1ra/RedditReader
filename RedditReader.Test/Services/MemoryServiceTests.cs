using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using RedditReader.Services;

namespace RedditReader.Test.Services;

public class MemoryServiceTests
{
    private MemoryService _memoryService;

    [SetUp]
    public void Setup()
    {
        _memoryService = new MemoryService();
    }

    [Test]
    public void GetTopVotedPosts_ProvidePostWith2And1Votes_Get2VotedFirstThen1Voted()
    {
        List<Post> tests = new List<Post>();
        var post1 = new Post(null)
        {
            Id = "test1", Author = "Author1", Created = DateTime.Now, Subreddit = "AskTest",
            Title = "CheckingVotes", UpVotes = 2,
        };
        var post2 = new Post(null)
        {
            Id = "test2", Author = "Author2", Created = DateTime.Now, Subreddit = "AskTest",
            Title = "CheckingVotes - 2", UpVotes = 1,
        };
        tests.Add(post1);
        tests.Add(post2);

        _memoryService.NewPostsUpdated(null, new PostsUpdateEventArgs() { Added = tests });
        var topVotedPosts = _memoryService.GetTopVotedPosts();

        Assert.That(topVotedPosts[0].Id, Is.EqualTo(post1.Id));
        Assert.That(topVotedPosts[0].UpVotes, Is.EqualTo(post1.UpVotes));
    }

    [Test]
    public void GetTopAuthors_ProvideAuthorWith2PostAndAuthorWith1Post_Get2FirstAnd1Next()
    {
        List<Post> tests = new List<Post>();
        var post1 = new Post(null)
        {
            Id = "test1", Author = "Author1", Created = DateTime.Now, Subreddit = "AskTest",
            Title = "CheckingAuthor", UpVotes = 2,
        };
        var post2 = new Post(null)
        {
            Id = "test2", Author = "Author2", Created = DateTime.Now, Subreddit = "AskTest",
            Title = "CheckingAuthor - 2", UpVotes = 1,
        };
        var post3 = new Post(null)
        {
            Id = "test3", Author = "Author2", Created = DateTime.Now, Subreddit = "AskTest",
            Title = "CheckingAuthor - 3", UpVotes = 1,
        };
        tests.Add(post1);
        tests.Add(post2);
        tests.Add(post3);

        _memoryService.NewPostsUpdated(null, new PostsUpdateEventArgs() { Added = tests });
        var topVotedPosts = _memoryService.GetTopAuthors();

        var author = post2.Author;
        Assert.That(topVotedPosts[0].Author, Is.EqualTo(author));
        Assert.That(topVotedPosts[0].PostCount, Is.EqualTo(tests.Count(p => p.Author.Equals(author))));
    }
    
    [Test]
    public void GetTopVotedPosts_ProvideNoPosts_GetNothing()
    {
        _memoryService.NewPostsUpdated(null, new PostsUpdateEventArgs() { Added = null });
        var topVotedPosts = _memoryService.GetTopVotedPosts();

        int nothing = 0;
        Assert.That(topVotedPosts.Count, Is.EqualTo(nothing));
    }
    
    [Test]
    public void GetTopAuthors_ProvideNoPosts_GetNothing()
    {
        _memoryService.NewPostsUpdated(null, new PostsUpdateEventArgs() { Added = null });
        var topVotedPosts = _memoryService.GetTopAuthors();

        int nothing = 0;
        Assert.That(topVotedPosts.Count, Is.EqualTo(nothing));
    }
}