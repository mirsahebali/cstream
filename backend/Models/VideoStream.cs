namespace Cstream.Models;


public class IceCandidateDto
{
    public string Candidate { get; set; } = string.Empty;
    public string SdpMid { get; set; }
    public int SdpMLineIndex { get; set; }
}

public class SessionDescriptionInit
{
    public required string Sdp { get; set; }
    public required string Type { get; set; }
}

public class VideoStream
{
    public int ID { get; set; }
    public User Streamer { get; set; }
    public DateTime CreatedAt { get; set; }
    public SessionDescriptionInit SDP { get; set; }
    public ICollection<User> Audiences { get; set; }

    public VideoStream(User creator, SessionDescriptionInit sdpValue)
    {
        Streamer = creator;
        CreatedAt = DateTime.Now;
        SDP = sdpValue;

        Random rnd = new Random();
        int roomId = rnd.Next(100000, 999999);
        ID = roomId;

        Audiences = new List<User>() { };
    }
}
