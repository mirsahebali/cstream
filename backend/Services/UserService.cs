using Cstream.Models;
using Cstream.Data;

namespace Cstream.Services;

public class UserService
{
    static List<User> Users { get; set; }

    private static CstreamContext _context;

    public UserService(CstreamContext context) { Users = new List<User> { }; _context = context; }

    public List<User> GetAll => Users;

    public User Get(string username)
    {
        User foundUser = _context.Users.FirstOrDefault(u => u.Username == username);

        return foundUser;
    }

    public async Task<User> CreateUser(string username, string password)
    {
        User foundUser = _context.Users.First(u => u.Username == username);
        if (foundUser != null)
        {
            Console.WriteLine("user " + username + " already exists");
            return null;
        }

        User user = new User(username, password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        Console.WriteLine("Created user " + username);
        return user;
    }

    public async Task<bool> RemoveUser(string username)
    {
        User foundUser = _context.Users.First(u => u.Username == username);

        if (foundUser == null)
        {
            Console.WriteLine("User " + username + " not found ");
            return false;
        }

        _context.Users.Remove(foundUser);
        await _context.SaveChangesAsync();
        Console.WriteLine("User " + username + " removed ");
        return true;
    }

    public void ApproveUser(User user, bool approve)
    {
        User foundUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
        if (foundUser == null)
        {
            Console.WriteLine("user " + user.Username + " not found");
            return;
        }

        user.Approved = approve;
        int rowsChanged = _context.SaveChanges();
        Console.WriteLine("Rows Modified: " + rowsChanged);
    }
}
