using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;

// State object for reading client data asynchronously
public class Player {
    public Player(string i, float start_x, float start_y, float start_dirx, float start_diry)
    {
        ID = i;
        x = start_x;
        y = start_y;
        dir_x = start_dirx;
        dir_y = start_diry;
    }
    public string ID;
    public float x;
    public float y;
    public float dir_x;
    public float dir_y;
    public int foodX;
    public int foodY;
}
public class Food
{
    public Food(int start_x, int start_y)
    {
        x = start_x;
        y = start_y;
    }
    public int x;
    public int y;
}
public class StateObject {
    // Client  socket.
    public Socket workSocket = null;
    // Size of receive buffer.
    public const int BufferSize = 1024;
    // Receive buffer.
    public byte[] buffer = new byte[BufferSize];
// Received data string.
    public StringBuilder sb = new StringBuilder();  
}
namespace Snake_Server
{
    public class AsynchronousSocketListener
    {
        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static Dictionary<String, Socket> clients = new Dictionary<String, Socket>();
        public static Dictionary<String, Player> players = new Dictionary<String, Player>();
        public static Player p1 = new Player("0", -21, 15, 1, 0);
        public static Player p2 = new Player("1", 21, 15, -1, 0);
        public static Player p3 = new Player("2", -21, -15, 1, 0);
        public static Player p4 = new Player("3", 21, -15, -1, 0);
        public static Database d = new Database();
        //private static bool gamestart = false;
        private static bool iNeedFood = false;

        public AsynchronousSocketListener()
        {
        }

        public void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            //Listen to external IP address
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            //IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 11000);

            // Listen to any IP Address
            IPEndPoint any = new IPEndPoint(IPAddress.Any, 11000);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(20);

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection..");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);
                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;

            // Games have bidirectional communication (as opposed to request/response)
            // So I need to store all clients sockets so I can send them messages later
            // TODO: store in meaningful way,such as Dictionary<string,Socket>
            string id = clients.Count.ToString();// + "<ID>";
            clients.Add(id, handler);
            if (id == "0")
                players.Add(id, p1);
            else if (id == "1")
                players.Add(id, p2);
            else if (id == "2")
                players.Add(id, p3);
            else if (id == "3")
                players.Add(id, p4);
            Dictionary<string, Socket>.KeyCollection keyColl =
            clients.Keys;
            foreach (string s in keyColl)
            {
                Console.WriteLine("key = {0}", s);
            }

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);
            Console.WriteLine(iNeedFood.ToString());
            if (iNeedFood)
            {
                int foodX = SpawnFoodCoords(35, -35);
                int foodY = SpawnFoodCoords(25, -25);
                foreach (string s in players.Keys)
                {
                    Food ffood = new Food(foodX, foodY);
                    Console.WriteLine("FOOD COORDS:");
                    Console.WriteLine("x:" + ffood.x + "  y:" + ffood.y);
                    string food = JsonConvert.SerializeObject(ffood);
                    Send(clients[s], "<FOOD>" + food + "<EOF>");
                }
                iNeedFood = false;
            }
            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the  
                    // client. Display it on the console.
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                    Console.WriteLine("\n\n");
                    content = content.Substring(0,content.IndexOf("<EOF>"));
                    Console.WriteLine(content);
                    if (content.Contains("<LOGIN>"))
                    {
                        content = content.Substring(content.IndexOf("<LOGIN>") + 7);
                        string[] info = content.Split(new string[]{"<SEP>"}, StringSplitOptions.None);
                        Console.WriteLine(info[0]);
                        Boolean result = d.GetInfo(info[0], info[1]);
                        if (result == true)
                        {
                            Console.WriteLine("sending");
                            string id = (clients.Count-1).ToString() + "<ID>";
                            Send(handler, id + "Success<EOF>");
                            if (clients.Count == 2)
                            {
                                Dictionary<string, Socket>.KeyCollection keyColl = clients.Keys;
                                foreach (string s in keyColl)
                                {
                                    Send(clients[s], "GAMESTART<EOF>");
                                    //gamestart = true;
                                }
                            }
                        }
                        else
                        {
                            Send(handler, "Failed<EOF>");
                        }
                    }
                    if (content.Contains("<FOOD>"))
                    {

                        iNeedFood = true;
                        Console.WriteLine("HELLO INEEDFOOD");
                        //Random rX = new Random();
                        //Random rY = new Random();
                        /*
                        int foodX = SpawnFoodCoords(35,-35);
                        int foodY = SpawnFoodCoords(25, -25);
                        //Console.WriteLine("FOOD COORDS:");
                        //Console.WriteLine("x:" + foodX + "  y:" + foodY);
                        foreach (string s in players.Keys)
                        {
                            Food ffood = new Food(foodX, foodY);
                            Console.WriteLine("FOOD COORDS:");
                            Console.WriteLine("x:" + ffood.x + "  y:" + ffood.y);
                            string food = JsonConvert.SerializeObject(ffood);
                            Send(clients[s], "<FOOD>" + food + "<EOF>");
                        }*/
                    }
                    if (content.Contains("<GAME>"))
                    {
                        content = content.Substring(content.IndexOf("<GAME>") + 6);
                        Player p = JsonConvert.DeserializeObject<Player>(content);
                        Console.WriteLine("Player {0} is at {1},{2} moving {3},{4}", p.ID, p.x, p.y, p.dir_x, p.dir_y);
                        players[p.ID] = p;
                        Dictionary<string, Player>.KeyCollection keyColl =
                            players.Keys;
                        //int foodX = SpawnFoodCoords(35,-35);
                        //int foodY = SpawnFoodCoords(25, -25);
                        //Console.WriteLine("FOOD COORDS:");
                        //Console.WriteLine("x:" + foodX + "  y:" + foodY);
                        foreach (string s in keyColl)
                        {
                            Console.WriteLine("YOU ARE HERE");
                            Console.WriteLine(s);
                            Console.WriteLine(p.ID);
                            //Food ffood = new Food(foodX, foodY);
                            //string food = JsonConvert.SerializeObject(ffood);
                            //Send(clients[s], "<FOOD>" + food + "<EOF>");
                            if (s != p.ID)
                            {
                                //players[p.ID].foodX = foodX;
                                //players[p.ID].foodY = foodY;
                                string output = JsonConvert.SerializeObject(players[p.ID]);
                                Send(clients[s], "<GAME>" + output + "<EOF>");
                            }
                        }
                    }
                    //else if (content.Contains("<COORD>"))
                    // Setup a new state object
                    StateObject newstate = new StateObject();
                    newstate.workSocket = handler;

                    // Call BeginReceive with a new state object
                    handler.BeginReceive(newstate.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), newstate);
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
               
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private static int SpawnFoodCoords(int upperRange, int lowerRange)
        {
            Random r = new Random();
            return r.Next(lowerRange, upperRange);


        }
    }
}