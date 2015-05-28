using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace Snake_Server
{
    public class Database
    {
        public Database()
        {
            if (!File.Exists("MyDatabase.sqlite"))
            {
                SQLiteConnection.CreateFile("MyDatabase.sqlite");
                SQLiteConnection m_dbConnection;
                m_dbConnection =
                    new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
                m_dbConnection.Open();
                string sql = "create table userinfo(name varchar not null unique, password varchar not null, scores varchar, primary key(name))";
                //string sql1 = "create table Tscores(name varchar, scores)";
                //sql = "create table Tscores(scores)";
                //string sql = "create table userinfo (name varchar(20), password str)";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
                //command = new SQLiteCommand(sql1, m_dbConnection);
                //command.ExecuteNonQuery();

                command.Dispose();
                m_dbConnection.Close();
                //SQLiteConnection.CreateFile("scores.sqlite");
                //SQLiteConnection m_dbConnection1;
                //m_dbConnection1 =
                //    new SQLiteConnection("Data Source=scores.sqlite;Version=3;");
                //m_dbConnection1.Open();
                //string sql1 = "create table Tscores(scores varchar)";
                //SQLiteCommand command1 = new SQLiteCommand(sql1, m_dbConnection1);
                //command1.ExecuteNonQuery();
            }
        }

        public void FillInfo(string username, string password)
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection =
                new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();

            string sql = "insert into userinfo(name, password, scores) values ('" + username + "', '" + password + "', '0')";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            command.Dispose();
            m_dbConnection.Close();
        }
        public void InsertScore(string name, string score)
        {
            Console.WriteLine("calling insertscore");
            using (SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;"))
            {
                string sql = "update userinfo set scores = '" + score + "' where name = '" + name + "';";
                Console.WriteLine(sql);
                using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                {
                    
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            Console.WriteLine("done insertscore");
        }
        public Boolean GetInfo(string username, string password)
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection =
                new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
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
            finally
            {
                m_dbConnection.Close();
            }
            return false;
        }
    }
}
