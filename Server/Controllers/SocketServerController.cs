using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services.Interfaces;
using SharedLibrary.Requests;

namespace Server.Controllers;

/*[Authorize]
[ApiController]
[Route("[controller]")]*/
public sealed class SocketServerController : ControllerBase
{
    private readonly ISocketServerService _socketServer;
    private readonly GameDbContext _context;
    private RoomRequest _room;
    

    public SocketServerController(ISocketServerService socketServer, GameDbContext context)
    {
        _socketServer = socketServer;
        _context = context;
    }

    [HttpPost("createRoom")]
    public IActionResult CreateRoom(RoomRequest room)
    {
        Console.WriteLine(room.Port);
       var (success, content) = _socketServer.Start(room.MaxPlayersCount, room.Port);
       if (success == false)
       {
           return Unauthorized(content);
       }

       _room = room;
       return Ok(content);
    }

    [HttpGet("get")]
    public IActionResult GetRoomPort()
    {
        if (_room.Port == decimal.Zero)
        {
            return NotFound("No rooms");
        }
        return Ok(_room);
    }
}