using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using System.Collections.Generic;

if (args.Length < 2)
{
    Console.WriteLine("Usage: Example <Reddit App ID> <Reddit Refresh Token> [Reddit Access Token]");
}
else
{
    string appId = args[0];
    string refreshToken = args[1];
    string accessToken = (args.Length > 2 ? args[2] : null);

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

    // Monitor new posts on this subreddit for a minute.  
    Console.WriteLine("Monitoring " + sub.Name + " for new posts....");

    sub.Posts.NewUpdated += C_NewPostsUpdated;
    sub.Posts.MonitorNew(); // Toggle on.
}

static void C_NewPostsUpdated(object sender, PostsUpdateEventArgs e)
{
    foreach (Post post in e.Added)
    {
        Console.WriteLine("[" + post.Subreddit + "] New Post by " + post.Author + ": " + post.Title);
    }
}