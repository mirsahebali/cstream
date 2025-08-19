namespace Cstream.Models;

public class User
{
    public int ID { get; set; }
    public string? Username { get; set; }
    public bool Approved { get; set; }

    public User(string username)
    {
        Random rnd = new Random();
        int id = rnd.Next(1, 100);
        ID = id;
        Username = username;
    }
}
