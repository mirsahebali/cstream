using Microsoft.AspNetCore.Identity;

namespace Cstream.Models;

public class User
{
    public int ID { get; set; }
    public string Username { get; set; }
    public string HashedPassword { get; set; }
    public bool Approved { get; set; }
    public ICollection<VideoStream> VideoStreams { get; set; }

    public User(string username, string plain_password)
    {
        var hasher = new PasswordHasher<User>();
        Random rnd = new Random();
        int id = rnd.Next(1, 1000000);
        ID = id;
        Username = username;

        HashedPassword = hasher.HashPassword(this, plain_password);
        Approved = false;

        VideoStreams = new List<VideoStream>() { };
    }
}
