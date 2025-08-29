using Cstream.Models;
using Cstream.Data;

namespace Cstream.Services;

public static class VideoStreamService
{
    static List<VideoStream> VideoStreams { get; }

    public VideoStreamService(CstreamContext context)
    {
        VideoStreams = new List<VideoStream> { };
    }

    public static List<VideoStream> GetAll => VideoStreams;

    public static VideoStream Get(int id) => VideoStreams.FirstOrDefault(s => s.ID == id);

    public static VideoStream CreateStream(string username, SessionDescriptionInit sdp)
    {
        User foundUser = UserService.Get(username);
        if (foundUser == null)
        {
            Console.WriteLine("user " + username + " does not exists");
            return null;
        }
        // Check if a user is already doing a stream
        VideoStream runningStream = VideoStreams.FirstOrDefault(s => s.Streamer.Username == username);
        if (runningStream != null)
        {
            Console.WriteLine("user " + username + " is already running a stream");
            return null;
        }

        VideoStream stream = new VideoStream(foundUser, sdp);
        VideoStreams.Add(stream);
        return stream;
    }

    public static bool AddAudienceToStream(int streamID, string username)
    {
        User audience = UserService.Get(username);
        if (audience == null)
        {
            Console.WriteLine("user with " + username + " not found");
            return false;
        }

        VideoStream stream = Get(streamID);
        if (stream == null)
        {
            Console.WriteLine("stream " + streamID + " not found");
            return false;
        }



        bool alreayJoined = stream.Audiences.Contains(audience);
        if (alreayJoined) return true;

        stream.Audiences.Add(audience);
        return true;
    }

    public static void EndStream(int id)
    {
        VideoStream foundStream = Get(id);

        if (foundStream == null)
        {
            Console.WriteLine("stream with id " + id + " not found ");
            return;
        }

        VideoStreams.Remove(foundStream);
        Console.WriteLine("Stream ended");
    }
}
