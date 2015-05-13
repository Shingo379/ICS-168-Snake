using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Snake_Server
{
    public class Database
    {
        public void Start()
        {
            SQLiteConnection.CreateFile("MyDatabase.sqlite");
            SQLiteConnection m_dbConnection;
            m_dbConnection =
                new SQLiteConnection("Data Source= MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();
            string sql = "create table userinfo(name varchar not null unique, password varchar not null, primary key(name))";
            //string sql = "create table userinfo (name varchar(20), password str)";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public void FillInfo(string username, string password)
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection =
                new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();

            string sql = "insert into userinfo(name, password) values ('" + username + "', '" + password + "')";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }
        
        public Boolean GetInfo(string username, string password)
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection =
                new SQLiteConnection("Data Source= MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();
            try
            {
                string sql = "select password from userinfo where name= '" + username + "'";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
                SQLiteDataReader dr = command.ExecuteReader();
                if (dr.Read())
                {
                    string result = (string)dr["password"];
                    Console.WriteLine("result:" + result);
                    Console.WriteLine("password:" + password);
                    if (password == result)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    FillInfo(username, password);
                    return true;
                }
            }
            catch
            {
                //Console.WriteLine("Error");
            }
            return false;
        }
    }
}
