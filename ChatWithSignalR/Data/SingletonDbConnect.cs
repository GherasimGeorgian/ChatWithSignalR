using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace ChatWithSignalR
{
    public class SingletonDbConnect
    {
        private static SingletonDbConnect dbInstance;
        private static string connString = "Server=(localdb)\\MSSQLLocalDB;Database=ChatWithSignalR3;MultipleActiveResultSets=true;Integrated Security = False;";
        private readonly SqlConnection conn = new SqlConnection(connString);

        private SingletonDbConnect()
        {
        }

        public static SingletonDbConnect getDbInstance()
        {
            if (dbInstance == null)
            {
                dbInstance = new SingletonDbConnect();
            }
            return dbInstance;
        }

        public SqlConnection getDbConnection()
        {
            try
            {
                conn.Close();
                conn.Open();
            }
            catch (SqlException e)
            {

            }
            finally
            {
            }
            return conn;
        }

    }
}
