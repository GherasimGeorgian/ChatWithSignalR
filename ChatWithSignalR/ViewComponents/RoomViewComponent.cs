using ChatWithSignalR.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatWithSignalR.ViewComponents
{
    public class RoomViewComponent : ViewComponent
    {
        private  AppDbContext _appDbContext;
        public RoomViewComponent(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public IViewComponentResult Invoke()
        {
            
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var chats = _appDbContext.ChatUsers
                    .Include(x => x.Chat)
                    .Where(x => x.UserId == userId)
                    .Select(x => x.Chat)
                    .ToList();
                return View(chats);

        
        }
    }
}
