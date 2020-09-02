using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChatWithSignalR.Models;
using ChatWithSignalR.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ChatWithSignalR.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private AppDbContext _appDbContext;
        public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {
            var chats = _appDbContext.Chats
                .Include(x => x.Users)
                .Where(x=> !x.Users
                .Any(y=>y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value))
                .ToList();
            return View(chats);
        }
        


        [HttpGet("{id}")]
        public IActionResult Chat(int id)
        {
            
            var chat = _appDbContext.Chats.Include(x => x.Messages)
                .FirstOrDefault(x => x.Id == id);

            var messages = _appDbContext.Messages
                .Include(x => x.Text)
                .Where(x => x.ChatId == id)
                .ToList();
            chat.Messages = messages;


            return View(chat);
        }
        [HttpPost]
        public async Task<IActionResult> CreateMessage(int chatId, string message)
        {
            var Message = new Message
            {
                ChatId = chatId,
                Text = message,
                Name =User.Identity.Name,
                Timestamp = DateTime.Now
            };
            _appDbContext.Messages.Add(Message);

            await _appDbContext.SaveChangesAsync();

           
            return RedirectToAction("Chat", new { id = chatId});
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(string name)
        {
            var chat = new Chat
            {
                Name = name,
                Type = ChatType.Room
            };
            chat.Users.Add(new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Admin
            });
            _appDbContext.Chats.Add(chat);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> JoinRoom(int id)
        {
            var chatUser = new ChatUser
            {
                ChatId = id,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Member
            };
            _appDbContext.ChatUsers.Add(chatUser);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Chat","Home", new { id = id});
        }
    }
}
