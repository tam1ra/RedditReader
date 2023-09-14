namespace RedditReader.Models;

public class LightPost
{
    public string Id { get; set; }
    public string Author { get; set; }
    public DateTime Created { get; set; }
    public bool IsVoted { get; set; }
    public string Subreddit { get; set; }
    public string Title { get; set; }
    public int UpVotes { get; set; }
}