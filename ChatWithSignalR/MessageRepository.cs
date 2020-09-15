using ChatWithSignalR.Hubs;
using ChatWithSignalR.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<Message> GetAllMessages()
        {
            var messages = new List<Message>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlDependency.Start(connectionString);

                string commandText = "select Id, Name, ChatId from dbo.Messages";

                SqlCommand cmd = new SqlCommand(commandText, conn);

                SqlDependency dependency = new SqlDependency(cmd);

                dependency.OnChange += new OnChangeEventHandler(dbChangeNotification);

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
        private void dbChangeNotification(object sender, SqlNotificationEventArgs e)
        {
            _contextHub.Clients.All.SendAsync("refreshMessages");
        }


    }
}
