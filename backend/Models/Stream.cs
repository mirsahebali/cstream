namespace Cstream.Models;

public class Stream
{
    public int ID { get; set; }
    public User streamer { get; set; }
    public DateTime createdAt { get; set; }
    public List<User> audiences()
    {
        return new List<User>() { };
    }

    public Stream(User creator)
    {
        streamer = creator;
        createdAt = DateTime.Now;

        Random rnd = new Random();
        int roomId = rnd.Next(100000, 999999);
        ID = roomId;
    }
}
