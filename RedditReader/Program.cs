﻿using System.Collections;
using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using System.Collections.Generic;

const int LIMIT = 5;

Dictionary<string, int> UpVotes = new();
PriorityQueue<string, int> TopVoted = new(Comparer<int>.Create((x, y) => y - x));
Dictionary<string, int> UserPosts = new();
PriorityQueue<string, int> TopPosted = new(Comparer<int>.Create((x, y) => y - x));

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

    PrintTopUpVotes(newPosts);
    PrintTopPosted(newPosts);

    // Monitor new posts on this subreddit for a minute.  
    Console.WriteLine("Monitoring " + sub.Name + " for new posts....");

    sub.Posts.NewUpdated += C_NewPostsUpdated;
    sub.Posts.MonitorNew(); // Toggle on.
}

void C_NewPostsUpdated(object sender, PostsUpdateEventArgs e)
{
    foreach (Post post in e.Added)
    {
        Console.WriteLine("[" + post.Subreddit + "] New Post by " + post.Author + ": " + post.Title);
    }

    PrintTopUpVotes(e.Added);
    PrintTopPosted(e.Added);
}

void PrintTopUpVotes(List<Post> posts)
{
    Dictionary<string, int> topUpVotes = new();
    foreach (var post in posts)
    {
        if (!UpVotes.ContainsKey(post.Id))
            UpVotes.Add(post.Id, 0);
        UpVotes[post.Id] += post.UpVotes;

        TopVoted.Enqueue(post.Id, UpVotes[post.Id]);
    }

    int count = 0, prev = -1;
    while (TopVoted.TryDequeue(out string id, out int priority))
    {
        if (!topUpVotes.ContainsKey(id))
            topUpVotes.Add(id, priority);
        else
            topUpVotes[id] = Math.Max(topUpVotes[id], priority);

        /*if (prev != priority)
        {
            count++;
            prev = priority;
        }
        if (count == LIMIT) break;*/
        if (topUpVotes.Count == LIMIT) break;
    }

    Console.WriteLine("*****BEGIN TOP UP VOTED POSTS-----");
    foreach (var vote in topUpVotes)
    {
        TopVoted.Enqueue(vote.Key, vote.Value);
        Console.WriteLine($"{vote.Key} : {vote.Value}");
    }

    Console.WriteLine("-----END TOP UP VOTED POSTS*****");
}

void PrintTopPosted(List<Post> posts)
{
    Dictionary<string, int> topList = new();
    foreach (var post in posts)
    {
        if (!UserPosts.ContainsKey(post.Id))
            UserPosts.Add(post.Id, 0);
        UserPosts[post.Id]++;

        TopPosted.Enqueue(post.Id, UserPosts[post.Id]);
    }

    int count = 0, prev = -1;
    while (TopPosted.TryDequeue(out string id, out int priority))
    {
        if (!topList.ContainsKey(id))
            topList.Add(id, priority);
        else
            topList[id] = Math.Max(topList[id], priority);

        /*if (prev != priority)
        {
            count++;
            prev = priority;
        }
        if (count == LIMIT) break;*/
        if (topList.Count == LIMIT) break;
    }

    Console.WriteLine("*****BEGIN TOP POSTED USERS-----");
    foreach (var vote in topList)
    {
        TopPosted.Enqueue(vote.Key, vote.Value);
        Console.WriteLine($"{vote.Key} : {vote.Value}");
    }

    Console.WriteLine("-----END TOP POSTED USERS*****");
}