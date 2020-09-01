using ChatWithSignalR.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var chats = _appDbContext.Chats.ToList();
            return View(chats);
        }
    }
}
