using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatWithSignalR.Data;
using ChatWithSignalR.Models;
using ChatWithSignalR.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatWithSignalR.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ChatController : Controller
    {
        IHubContext<ChatHub> _chat;
        public ChatController(IHubContext<ChatHub> chat)
        {
            _chat = chat;
        }
        [HttpPost("[action]/{connectionId}/{roomName}")]
        public async Task<IActionResult> JoinRoom(string connectionId, string roomName)
        {
            await _chat.Groups.AddToGroupAsync(connectionId,roomName);
            return Ok();
        }
        [HttpPost("[action]/{connectionId}/{roomName}")]
        public async Task<IActionResult> LeaveRoom(string connectionId, string roomName)
        {
            await _chat.Groups.AddToGroupAsync(connectionId, roomName);
            return Ok();
        }

        public async Task<IActionResult> SendMessage(string message,int chatId ,string roomName,[FromServices] AppDbContext appDbContext)
        {

            var Message = new Message
            {
                ChatId = chatId,
                Text = message,
                Name = User.Identity.Name,
                Timestamp = DateTime.Now
            };
            appDbContext.Messages.Add(Message);

            await appDbContext.SaveChangesAsync();
            await _chat.Clients.Group(roomName).SendAsync("RecieveMessage",Message);
            return Ok();
        }

        public IActionResult Index()
        {
            return View();
        }

    }
}
