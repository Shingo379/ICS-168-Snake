using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Snake_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
        }
        public Program()
        {
            AsynchronousSocketListener AS = new AsynchronousSocketListener();
            AS.StartListening();
            System.Console.ReadLine();
        }
    }
}
