using Microsoft.AspNetCore.SignalR;
using Cstream.Models;
using Cstream.Services;



namespace Cstream.Hubs;


public class VideoStreamHub : Hub
{
    private readonly UserService _service;

    public VideoStreamHub(UserService service)
    {
        _service = service;
    }

    public async Task StartStream(string username, SessionDescriptionInit offer)
    {
        Console.WriteLine("Starting stream of " + username);
        User foundUser = _service.Get(username);
        if (foundUser == null)
        {
            Console.WriteLine("user " + username + " not found");
            return;
        }

        VideoStream stream = VideoStreamService.CreateStream(username, offer);
        if (stream == null)
        {
            Console.WriteLine("Unable to create stream");
            return;
        }
        int id = stream.ID;
        await Clients.Caller.SendAsync("SetData::" + username, id);
        Console.WriteLine("Wrote stream data back to " + "SetData::" + username);
    }
    public async Task JoinStream(int id, string username)
    {
        VideoStream stream = VideoStreamService.Get(id);
        if (stream == null)
        {
            await Clients.All.SendAsync("JoinStreamData::" + username, null);
            return;
        }

        Console.WriteLine(username + " is joining stream " + stream.ID);
        await Clients.All.SendAsync("JoinStreamData::" + username, stream.SDP);
        User audience = _service.Get(username);
        if (audience == null)
        {
            Console.WriteLine("no user " + username + " found");
            return;
        }
        stream.Audiences.Add(audience);

        return;
    }

    public async Task JoinStreamAnswer(string username, int streamID, SessionDescriptionInit answer)
    {
        bool success = VideoStreamService.AddAudienceToStream(streamID, username);
        if (!success)
        {
            return;
        }

        VideoStream stream = VideoStreamService.Get(streamID);
        if (stream == null)
        {
            return;
        }
        await Clients.Caller.SendAsync("JoinStreamAnswer::" + stream.Streamer.Username, username, answer);
        await Clients.Caller.SendAsync("JoinStreamData::" + stream.Streamer.Username, username, answer);

        foreach (var audience in stream.Audiences)
        {
            await Clients.All.SendAsync("JoinStreamAnswer::" + audience.Username, stream.SDP);
            await Clients.All.SendAsync("JoinStreamData::" + audience.Username, stream.SDP);
            Console.WriteLine("Sent event: " + "JoinStreamAnswer::" + audience.Username);
            Console.WriteLine("Sent event: " + "JoinStreamData::" + audience.Username);
        }

        Console.WriteLine(username + " has answered with SDP stream " + streamID);
        return;
    }
    public async Task NewIceCandidate(int streamID, IceCandidateDto eventCandidate)
    {
        await Clients.All.SendAsync("NewIceCandidate::" + streamID, eventCandidate);
        return;
    }


}
