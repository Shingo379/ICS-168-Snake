using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //AsynchronousSocketListener a = new AsynchronousSocketListener();
            //a.start();
            Program p = new Program();
        }
        public Program()
        {
            AsynchronousSocketListener.StartListening();
            System.Console.ReadLine();
        }
    }
}
