using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using RedditReader.Models;

namespace RedditReader.Services;

public class MemoryService : IService
{
    private const int LIMIT = 5;

    private PriorityQueue<string, int> TopVotedPosts = new(Comparer<int>.Create((x, y) => y - x));
    private PriorityQueue<string, TopAuthor> TopPostedAuthor = new(Comparer<TopAuthor>.Create((x, y) => y.PostCount - x.PostCount))
        ;
    private Dictionary<string, LightPost> AllPosts = new();
    private Dictionary<string, int> MostVotesPost = new();
    private Dictionary<string, TopAuthor> MostPostedAuthor = new();

    public MemoryService()
    {
    }

    public void NewPostsUpdated(object sender, PostsUpdateEventArgs e)
    {
        foreach (Post post in e.Added)
        {
            if (!AllPosts.ContainsKey(post.Id))
                AllPosts.Add(post.Id, new LightPost());

            LightPost lightPost = new()
            {
                Id = post.Id,
                Author = post.Author,
                Created = post.Created,
                IsUpVoted = post.IsUpvoted,
                Subreddit = post.Subreddit,
                Title = post.Title,
                UpVotes = post.UpVotes
            };
            AllPosts[post.Id] = lightPost;
        }

        CalcTopVotedPosts(e.Added);
        CalcTopAuthors(e.Added);
    }

    private void CalcTopVotedPosts(IList<Post> posts)
    {
        foreach (var post in posts)
        {
            if (!MostVotesPost.ContainsKey(post.Id))
                MostVotesPost.Add(post.Id, 0);
            MostVotesPost[post.Id] = post.UpVotes;

            TopVotedPosts.Enqueue(post.Id, MostVotesPost[post.Id]);
        }
        
        //TODO:
        //Remove it and use it in UI side
        GetTopVotedPosts();
    }

    public IList<LightPost> GetTopVotedPosts()
    {
        Dictionary<string, int> topVotes = new();
        int count = 0, prev = -1;
        while (TopVotedPosts.TryDequeue(out string id, out int priority))
        {
            if (!topVotes.ContainsKey(id))
                topVotes.Add(id, priority);
            else
                topVotes[id] = Math.Max(topVotes[id], priority);

            /*if (prev != priority)
            {
                count++;
                prev = priority;
            }
            if (count == LIMIT) break;*/
            if (topVotes.Count == LIMIT) break;
        }

        List<LightPost> topVotedPosts = new();
        foreach (var post in topVotes)
        {
            TopVotedPosts.Enqueue(post.Key, post.Value);
            topVotedPosts.Add(AllPosts[post.Key]);
        }
        
        //TODO:
        //just for test purposes. Need to remove after UI.
        PrintTopVotedPosts(topVotedPosts);
        
        return topVotedPosts;
    }

    private void PrintTopVotedPosts(IList<LightPost> posts)
    {
        Console.WriteLine("*****BEGIN TOP UP VOTED POSTS-----");
        foreach (var post in posts)
        {
            Console.WriteLine($"{post.Title} : {post.UpVotes}");
        }

        Console.WriteLine("-----END TOP UP VOTED POSTS*****");
    }

    private void CalcTopAuthors(IList<Post> posts)
    {
        foreach (var post in posts)
        {
            if (!MostPostedAuthor.ContainsKey(post.Id))
                MostPostedAuthor.Add(post.Id, new TopAuthor(post.Author, 0) );
            MostPostedAuthor[post.Id].PostCount++;

            TopPostedAuthor.Enqueue(post.Id, MostPostedAuthor[post.Id]);
        }

        //TODO:
        //Remove it and use it in UI side
        GetTopAuthors();
    }
    
    public IList<TopAuthor> GetTopAuthors()
    {
        Dictionary<string, int> topList = new();
        int count = 0, prev = -1;
        while (TopPostedAuthor.TryDequeue(out string id, out TopAuthor topAuthor))
        {
            if (!topList.ContainsKey(id))
                topList.Add(id, topAuthor.PostCount);
            else
                topList[id] = Math.Max(topList[id], topAuthor.PostCount);

            /*if (prev != priority)
            {
                count++;
                prev = priority;
            }
            if (count == LIMIT) break;*/
            if (topList.Count == LIMIT) break;
        }

        List<TopAuthor> topAuthors = new();
        foreach (var post in topList)
        {
            var author = MostPostedAuthor[post.Key];
            TopPostedAuthor.Enqueue(post.Key, author);
            topAuthors.Add(author);
        }

        //TODO:
        //just for test purposes. Need to remove after UI.
        PrintTopAuthors(topAuthors);
        
        return topAuthors;
    }

    private void PrintTopAuthors(IList<TopAuthor> topAuthors)
    {
        Console.WriteLine("*****BEGIN TOP POSTED USERS-----");
        foreach (var top in topAuthors)
        {
            Console.WriteLine($"{top.Author} : {top.PostCount}");
        }

        Console.WriteLine("-----END TOP POSTED USERS*****");
    }
}