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
using System.Security.Claims;
using System.Threading;

namespace ChatWithSignalR.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ChatController : Controller
    {
        IHubContext<ChatHub> _chat;
        private AppDbContext _appDbContext;
        public ChatController(IHubContext<ChatHub> chat, AppDbContext appDbContext)
        {
           
            _chat = chat;
            _appDbContext = appDbContext;
        }
        [HttpGet("[action]")]
        public IActionResult GetCurrentUserId()
        {
           
            return Ok(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        [HttpGet("[action]")]
        public IActionResult GetCurrentUserName()
        {
            var user = _appDbContext.Users
               .Where(x => x.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value)
               .FirstOrDefault();
            return Ok(user.UserName);
        }


        [HttpPost("[action]/{connectionId}/{roomId}")]
        public async Task<IActionResult> JoinRoom(string connectionId, string roomId)
        {
            await _chat.Groups.AddToGroupAsync(connectionId, roomId);
            return Ok();
        }
        [HttpPost("[action]/{connectionId}/{roomId}")]
        public async Task<IActionResult> LeaveRoom(string connectionId, string roomId)
        {
            await _chat.Groups.AddToGroupAsync(connectionId, roomId);
            return Ok();
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(string message,int roomId, [FromServices] AppDbContext appDbContext)
        {

            var Message = new Message
            {
                ChatId = roomId,
                Text = message,
                Name = User.Identity.Name,
                Timestamp = DateTime.Now
            };
            appDbContext.Messages.Add(Message);
            

            await appDbContext.SaveChangesAsync();
            await _chat.Clients.Group(roomId.ToString()).SendAsync("RecieveMessage", new
            {
                Text = Message.Text,
                Name = Message.Name,
                Timestamp = Message.Timestamp.ToString("dd/MM/yyyy hh:mm:ss"),
                CurrentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString()
            });
            return Ok();
        }

        public IActionResult Index()
        {

            return View();
        }

    }
}
