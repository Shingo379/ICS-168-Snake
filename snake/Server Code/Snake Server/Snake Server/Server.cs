using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Timers;

// State object for reading client data asynchronously
public class Player
{
    public Player(string i, float start_x, float start_y, float start_dirx, float start_diry, string session)
    {
        ID = i;
        x = start_x;
        y = start_y;
        dir_x = start_dirx;
        dir_y = start_diry;
        session = game_session;
    }
    public string ID;//id within server
    public string secondID;//1-3, id within session
    public float x;
    public float y;
    public float dir_x;
    public float dir_y;
    public int foodX;
    public int foodY;
    public string game_session;
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
public class Game_State
{
    public Game_State(Boolean gs, Boolean inf)
    {
        game_start = gs;
        ineedfood = inf;
    }
    public Boolean game_start;
    public Boolean ineedfood;
}
public class StateObject
{
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
        public static Dictionary<String, String> playerName = new Dictionary<String,String>();
        public static Dictionary<String, Dictionary<String, Player>> playersBySession = new Dictionary<string, Dictionary<string, Player>>();
        public static Dictionary<String, Game_State> sessionState = new Dictionary<String, Game_State>();
        //public static Player p1 = new Player("0", -21, 15, 1, 0, "default");
        //public static Player p2 = new Player("1", 21, 15, -1, 0, "default");
        //public static Player p3 = new Player("2", -21, -15, 1, 0, "default");
        //public static Player p4 = new Player("3", 21, -15, -1, 0, "default");
        //private static bool gamestart = false;
        public static Database b = new Database();
        private static bool iNeedFood = false;
        private static List<string> dc_clients = new List<string>();
        static System.Timers.Timer heartbeat = new System.Timers.Timer(5000);
        //public static Database d = new Database();

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
            Player p = new Player(id, 21, 15, -1, 0, "default");
            players.Add(id, p);
            /*
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
            }*/

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            //Database d = new Database();
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            //try
            //{
                int bytesRead = handler.EndReceive(ar);
                Console.WriteLine(iNeedFood.ToString());
                foreach (string s in sessionState.Keys)
                {
                    if (sessionState[s].ineedfood)
                    {
                        int foodX = SpawnFoodCoords(35, -35);
                        int foodY = SpawnFoodCoords(25, -25);
                        foreach (string x in playersBySession[s].Keys)
                        {
                            Food ffood = new Food(foodX, foodY);
                            Console.WriteLine("FOOD COORDS:");
                            Console.WriteLine("x:" + ffood.x + "  y:" + ffood.y);
                            string food = JsonConvert.SerializeObject(ffood);
                            Send(clients[x], "<FOOD>" + food + "<EOF>");
                        }
                        sessionState[s].ineedfood = false;
                    }
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
                        content = content.Substring(0, content.IndexOf("<EOF>"));
                        Console.WriteLine(content);
                        if (content.Contains("<LOGIN>"))
                        {
                            Console.WriteLine("hellotestinglogin");
                            content = content.Substring(content.IndexOf("<LOGIN>") + 7);
                            string[] info = content.Split(new string[] { "<SEP>" }, StringSplitOptions.None);
                            Console.WriteLine(info[0]);
                            Boolean result = b.GetInfo(info[0], info[1]);
                            if (result == true)
                            {
                                Console.WriteLine("sending");
                                //string id = (clients.Count-1).ToString() + "<ID>";
                                playerName.Add((clients.Count - 1).ToString(), info[0]);
                                players[(clients.Count - 1).ToString()].game_session = info[2];
                                players[(clients.Count - 1).ToString()].ID = (clients.Count - 1).ToString();
                                if (playersBySession.ContainsKey(info[2]))
                                {
                                    players[(clients.Count - 1).ToString()].secondID = (playersBySession[info[2]].Count).ToString();
                                    playersBySession[info[2]].Add((playersBySession[info[2]].Count).ToString(), players[(clients.Count - 1).ToString()]);
                                }
                                else
                                {
                                    Dictionary<string, Player> temp = new Dictionary<string, Player>();
                                    players[(clients.Count - 1).ToString()].secondID = (temp.Count).ToString();
                                    temp.Add((temp.Count).ToString(), players[(clients.Count - 1).ToString()]);
                                    playersBySession.Add(info[2], temp);
                                    Game_State temp_state = new Game_State(false, false);
                                    sessionState.Add(info[2], temp_state);
                                }
                                Send(handler, (clients.Count - 1).ToString() + "<P#>" + (playersBySession[info[2]].Count - 1) + "<ID>Success<EOF>");
                                //int ID = clients.Count-1;
                                //int checkPlayersinSession = 0;
                                foreach (string a in playersBySession.Keys)
                                {
                                    if (playersBySession[a].Count == 3)
                                    {
                                        //Dictionary<string, Socket>.KeyCollection keyColl = clients.Keys;
                                        //foreach (string s in keyColl)
                                        //{
                                        heartbeat.Elapsed += SendHeartbeat;
                                        heartbeat.Enabled = true;
                                        sessionState[a].game_start = true;
                                        foreach (string s in playersBySession[a].Keys)
                                        {
                                            Console.WriteLine("SENDING TO: " + s);
                                            Send(clients[playersBySession[a][s].ID], "GAMESTART<EOF>");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Send(handler, "Failed<EOF>");
                            }
                        }
                        else if (content.Contains("<FOOD>"))
                        {
                            string[] temp = content.Split(new string[] { "<FOOD>" }, StringSplitOptions.None);
                            sessionState[players[temp[0]].game_session].ineedfood = true;
                            //iNeedFood = true;
                            Console.WriteLine("HELLO INEEDFOOD");
                        }
                        else if (content.Contains("<GAME>"))
                        {
                            content = content.Substring(content.IndexOf("<GAME>") + 6);
                            string[] move = content.Split(new string[] { "<SEP>" }, StringSplitOptions.None);
                            playersBySession[players[move[0]].game_session][move[1]].dir_x = Convert.ToSingle(move[2]);
                            playersBySession[players[move[0]].game_session][move[1]].dir_y = Convert.ToSingle(move[3]);
                            playersBySession[players[move[0]].game_session][move[1]].x = Convert.ToSingle(move[4]);
                            playersBySession[players[move[0]].game_session][move[1]].y = Convert.ToSingle(move[5]);
                            foreach (string a in playersBySession.Keys)
                            {
                                foreach (Player p in playersBySession[players[move[0]].game_session].Values)
                                {
                                    if (p.ID != move[0])
                                    {
                                        Send(clients[p.ID], "<GAME>" + content + "<EOF>");
                                    }
                                }
                            }

                        }
                        else if (content.Contains("<SCORE1>"))
                        {
                            string temp_id = content.Split(new string[] { "<ID>" }, StringSplitOptions.None)[0];
                            content = content.Substring(content.IndexOf("<SCORE1>") + 8);
                            b.InsertScore(playerName[temp_id], content);
                            //insert(content);
                            foreach (Player p in playersBySession[players[temp_id].game_session].Values)
                            {
                                if (p.ID != temp_id)
                                    Send(clients[p.ID], "<SCORE1>" + content + "<EOF>");
                                if (content == "10")
                                {
                                    Console.WriteLine("p1 won");
                                    Send(clients[p.ID], "<GAMEOVER>Player 1<EOF>");
                                }
                            }
                        }
                        else if (content.Contains("<SCORE2>"))
                        {
                            string temp_id = content.Split(new string[] { "<ID>" }, StringSplitOptions.None)[0];
                            content = content.Substring(content.IndexOf("<SCORE2>") + 8);
                            b.InsertScore(playerName[temp_id], content);
                            //insert(content);
                            foreach (Player p in playersBySession[players[temp_id].game_session].Values)
                            {
                                if (p.ID != temp_id)
                                    Send(clients[p.ID], "<SCORE2>" + content + "<EOF>");
                                if (content == "10")
                                    Send(clients[p.ID], "<GAMEOVER>Player 2<EOF>");
                            }
                        }
                        else if (content.Contains("<SCORE3>"))
                        {
                            string temp_id = content.Split(new string[] { "<ID>" }, StringSplitOptions.None)[0];
                            content = content.Substring(content.IndexOf("<SCORE3>") + 8);
                            b.InsertScore(playerName[temp_id] ,content);
                            //insert(content);
                            foreach (Player p in playersBySession[players[temp_id].game_session].Values)
                            {
                                if (p.ID != temp_id)
                                    Send(clients[p.ID], "<SCORE3>" + content + "<EOF>");
                                if (content == "10")
                                    Send(clients[p.ID], "<GAMEOVER>Player 3<EOF>");
                            }
                        }
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
            //}
            //catch { }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            try
            {
                handler.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), handler);
            }
            catch
            {
                //handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                foreach (string s in clients.Keys)
                {
                    if (clients[s] == handler)
                    {
                        if (!dc_clients.Contains(s))
                            dc_clients.Add(s);
                    }
                }
                Console.WriteLine("a user has disconnected");
            }
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
            int result = r.Next(lowerRange, upperRange);
            Console.WriteLine("THIS IS SPAWNFOODCORDSRANDOMINT:");
            Console.WriteLine(result);
            return result;
        }
        private static void SendHeartbeat(Object source, ElapsedEventArgs e)
        {
            if (players.Count == 0)
            {
                heartbeat.Enabled = false;
            }
            else
            {
                for (int s = 0; s < dc_clients.Count; s++)
                {
                    Console.WriteLine("removing:" + dc_clients[s]);
                    string this_session = players[dc_clients[s]].game_session;
                    playersBySession[players[dc_clients[s]].game_session].Remove(players[dc_clients[s]].secondID);
                    players.Remove(dc_clients[s]);
                    //clients.Remove(dc_clients[s]);
                    foreach (Player x in playersBySession[this_session].Values)
                    {
                        Send(clients[x.ID], "<DISCONNECT>" + dc_clients[s].ToString() + "<EOF>");
                    }
                }
                dc_clients.Clear();
                foreach (string s in playersBySession.Keys)
                {
                    if (sessionState[s].game_start)
                    {
                        foreach (Player x in playersBySession[s].Values)
                            Send(clients[x.ID], "<HEARTBEAT><EOF>");
                    }
                }
            }
        }
    }
}