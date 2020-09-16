using ChatWithSignalR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatWithSignalR
{
    public interface IMessageRepository
    {
        List<Message> GetAllMessages();

        List<Message> GetAllNotifications();
    }
}
