using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChatWithSignalR.Models;
using ChatWithSignalR.Data;
using System.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ChatWithSignalR.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IMessageRepository _messageRepository;
        private AppDbContext _appDbContext;
        public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext,IMessageRepository messageRepository)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _messageRepository = messageRepository;
        }
        public IActionResult GetMessages()
        {
            return Ok(_messageRepository.GetAllMessages());
        }
        public IActionResult Index()
        {
          
            var chats = _appDbContext.Chats
                .Include(x => x.Users)
                .Where(x=> !x.Users.Any(y=>y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value) && x.Type == ChatType.Room)
                .ToList();
            return View(chats);
        }
        


        [HttpGet("{id}")]
        public IActionResult Chat(int id)
        {
            var result = _appDbContext.Chats
               .Include(x => x.Users)
               .Where(x => x.Users.Any(y => y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value) && x.Id == id)
               .ToList();
            //verificam daca suntem in chatul respectiv
            if (result.Count == 0)
            {
                return RedirectToAction("Privacy");
            }
            else
            {
                var chat = _appDbContext.Chats.Include(x => x.Messages)
                            .FirstOrDefault(x => x.Id == id);

                return View(chat);
            }
           
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

        public IActionResult Find()
        {
            var users = _appDbContext.Users
                .Where(x => x.Id != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .ToList();
            return View(users);
        }

        public IActionResult Private()
        {
            var chats = _appDbContext.Chats
               .Include(x => x.Users)
                  .ThenInclude(x => x.User)
               .Where(x => x.Type == ChatType.Private
                  && x.Users.Any(y => y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value))
               .ToList();
            return View(chats);
        }


        public async Task<IActionResult> CreatePrivateRoom(string userId)
        {
            //chaturile private in care este userul actual
            var chatsMy = _appDbContext.Chats
                .Include(x => x.Users)
                   .ThenInclude(x => x.User)
                .Where(x => x.Type == ChatType.Private
                   && x.Users.Any(y => y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value))
                .Select(x => x.Id)
                .ToList();
            //chaturile private in care se afla userul cu care vrea sa comunice
            var chatsFrend = _appDbContext.Chats
               .Include(x => x.Users)
                  .ThenInclude(x => x.User)
               .Where(x => x.Type == ChatType.Private
                  && x.Users.Any(y => y.UserId == userId))
               .Select(x => x.Id)
               .ToList();

            var result = chatsMy.Intersect(chatsFrend);

            if (result.Count() == 0)
            {

                var chat = new Chat
                {
                    Type = ChatType.Private
                };
                chat.Users.Add(new ChatUser
                {
                    UserId = userId
                });

                chat.Users.Add(new ChatUser
                {
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
                });
                _appDbContext.Chats.Add(chat);

                await _appDbContext.SaveChangesAsync();
                return RedirectToAction("Chat", new { id = chat.Id });
            }
            else
            {
                return RedirectToAction("Index");
            }
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
