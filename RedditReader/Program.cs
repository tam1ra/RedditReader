using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using RedditReader.Services;

const int LIMIT = 5;

Dictionary<string, int> UpVotes = new();
PriorityQueue<string, int> TopVoted = new(Comparer<int>.Create((x, y) => y - x));
Dictionary<string, int> UserPosts = new();
PriorityQueue<string, int> TopPosted = new(Comparer<int>.Create((x, y) => y - x));


string appId = (args.Length > 0 ? args[0] : null);
string refreshToken = (args.Length > 1 ? args[1] : null);
string accessToken = (args.Length > 2 ? args[2] : null);

if (string.IsNullOrWhiteSpace(appId))
{
    Console.Write("App ID: ");
    appId = Console.ReadLine();
}

if (string.IsNullOrWhiteSpace(refreshToken))
{
    Console.Write("Refresh Token: ");
    refreshToken = Console.ReadLine();
}

// Initialize the API library instance.  
RedditClient reddit = new RedditClient(appId: appId, refreshToken: refreshToken, accessToken: accessToken);

// Get info on the Reddit user authenticated by the OAuth credentials.  
User me = reddit.Account.Me;

Console.WriteLine("Username: " + me.Name);
Console.WriteLine("Cake Day: " + me.Created.ToString("D"));

// Get info about a subreddit.  
Subreddit sub = reddit.Subreddit("askreddit").About();

Console.WriteLine("Subreddit Name: " + sub.Name);
Console.WriteLine("Subreddit Fullname: " + sub.Fullname);
Console.WriteLine("Subreddit Title: " + sub.Title);
Console.WriteLine("Subreddit Description: " + sub.Description);

// Get new posts from this subreddit.  
List<Post> newPosts = sub.Posts.New;
Console.WriteLine("Retrieved " + newPosts.Count.ToString() + " new posts.");
IService service = new MemoryService();

service.NewPostsUpdated(null, new PostsUpdateEventArgs() { Added = newPosts });

// Monitor new posts on this subreddit for a minute.  
Console.WriteLine("Monitoring " + sub.Name + " for new posts....");

sub.Posts.NewUpdated += service.NewPostsUpdated;
sub.Posts.MonitorNew(); // Toggle on.