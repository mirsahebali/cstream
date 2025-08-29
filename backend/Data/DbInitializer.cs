using Cstream.Models;

namespace Cstream.Data;


public static class DbInitializer
{
    public static void Initialize(CstreamContext context)
    {
        // look for any user
        if (context.Users.Any())
        {
            return; // DB is seeded
        }

        var users = new User[]{
          new User("admin", "youlikerusttho?"),
          new User("admin1", "youlikerusttho,don'tyou?"),
        };

        context.Users.AddRange(users);
        context.SaveChanges();

        var videoStreams = new VideoStream[]{
          new VideoStream(users[0], null),
        };

        context.VideoStreams.AddRange(videoStreams);
        context.SaveChanges();
    }
}
