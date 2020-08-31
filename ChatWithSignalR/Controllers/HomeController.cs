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

namespace ChatWithSignalR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppDbContext _appDbContext;
        public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRoom(string name)
        {
            _appDbContext.Chats.Add(new Chat
            {
                Name = name,
                Type = ChatType.Room
            });
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet("{id}")]
        public IActionResult Chat(int id)
        {
            var chat = _appDbContext.Chats
                .Include(x=>x.Messages)
                .FirstOrDefault(x => x.Id == id);
            return View(chat);
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
    }
}
