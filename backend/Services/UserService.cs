using Cstream.Models;

namespace Cstream.Services;

public static class UserService
{
    static List<User> Users { get; }

    static UserService()
    {
        Users = new List<User> { };
    }

    public static List<User> GetAll => Users;

    public static User? Get(string username) => Users.FirstOrDefault(u => u.Username == username);

    public static User? CreateUser(string username)
    {
        User? foundUser = Users.FirstOrDefault(u => u.Username == username);
        if (foundUser != null)
        {
            Console.WriteLine("user " + username + " already exists");
            return null;
        }

        User user = new User(username);

        Users.Add(user);

        Console.WriteLine("Created user " + username);
        return user;
    }

    public static void RemoveUser(string username)
    {
        User? foundUser = Users.Find(u => u.Username == username);

        if (foundUser == null)
        {
            Console.WriteLine("User " + username + " not found ");
            return;
        }

        Users.Remove(foundUser);
        Console.WriteLine("User " + username + " removed ");
    }

    public static void ApproveUser(User user, bool approve)
    {
        User? foundUser = Users.FirstOrDefault(u => u.Username == user.Username);
        if (foundUser != null)
        {
            user.Approved = approve;
        }
        else
        {
            Console.WriteLine("user " + user.Username + " not found");
        }
    }
}
