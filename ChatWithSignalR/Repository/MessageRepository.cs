using ChatWithSignalR.Controllers;
using ChatWithSignalR.Hubs;
using ChatWithSignalR.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatWithSignalR
{
   
    public class MessageRepository : IMessageRepository
    {
        string connectionString = "";

        private readonly IHubContext<ChatHub> _contextHub;
        public MessageRepository(IConfiguration configuration,IHubContext<ChatHub> contextHub)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            _contextHub = contextHub;
        }
        
       
        List<Message> messages;
        public List<Message> GetAllMessages()
        {

            messages = new List<Message>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                

                string commandText = "select  Id, Name, ChatId  from dbo.Messages";

                SqlCommand cmd = new SqlCommand(commandText, conn);

                SqlDependency dependency = new SqlDependency(cmd);

                dependency.OnChange += new OnChangeEventHandler(GetAllMessagesDep_OnChange);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var Message = new Message
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        ChatId = Convert.ToInt32(reader["ChatId"])
                    };

                    messages.Add(Message);
                }
            }
            return messages;

        }
        List<Message> notifications;
        public List<Message> GetAllNotifications()
        {
            notifications = new List<Message>();
            SingletonDbConnect conn = SingletonDbConnect.getDbInstance();
            using (HomeController.command = new SqlCommand("select Id, Name, ChatId from dbo.Messages where ChatId = 28", conn.getDbConnection()))
                {

                    HomeController.command.Notification = null;

                    if (HomeController.dependency == null)
                    {
                    SqlDependency dependency = new SqlDependency(HomeController.command);

                    dependency.OnChange += new OnChangeEventHandler(GetAllNotificationsDep_OnChange);
                   // HomeController.dependency = new SqlDependency(HomeController.command);
                    //    HomeController.dependency.OnChange += new OnChangeEventHandler(GetAllNotificationsDep_OnChange);
                    }
                    var reader = HomeController.command.ExecuteReader();


                    while (reader.Read())
                    {
                        var Message = new Message
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            ChatId = Convert.ToInt32(reader["ChatId"])
                        };

                        notifications.Add(Message);
                    }
                }
               
            return notifications;
        }

        private void GetAllNotificationsDep_OnChange(object sender, SqlNotificationEventArgs e)
        {

            //// 1. unsubscribe the event
            //SqlDependency dependency = sender as SqlDependency;
            //dependency.OnChange -= GetAllNotificationsDep_OnChange;



            _contextHub.Clients.All.SendAsync("added");

         
            //GetAllNotifications();
        }
            private void GetAllMessagesDep_OnChange(object sender, SqlNotificationEventArgs e)
            {
           
            _contextHub.Clients.All.SendAsync("refreshMessages");
         
            }


        }
       

    
}
