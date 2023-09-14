namespace RedditReader.Models;

public class TopAuthor
{
    public TopAuthor(string author, int postCount)
    {
        Author = author;
        PostCount = postCount;
    }

    public string Author { get; set; }
    public int PostCount { get; set; }
}