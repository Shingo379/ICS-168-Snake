using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Newtonsoft.Json;

// State object for receiving data from remote device.
public class StateObject {
	// Client socket.
	public Socket workSocket = null;
	// Size of receive buffer.
	public const int BufferSize = 256;
	// Receive buffer.
	public byte[] buffer = new byte[BufferSize];
	// Received data string.
	public StringBuilder sb = new StringBuilder();
	public String response = String.Empty;
}

public class AsynchronousClient {
	// The port number for the remote device.
	private const int port = 11000;
	
	// ManualResetEvent instances signal completion.
	private static ManualResetEvent connectDone = 
		new ManualResetEvent(false);
	private static ManualResetEvent sendDone = 
	    new ManualResetEvent(false);
	private static ManualResetEvent receiveDone = 
	    new ManualResetEvent(false);
	
	// The response from the remote device.
	//private static String response = String.Empty;
	static string[] stringSeparators = new string[] { "<EOF>" };
	public string ID = String.Empty;
	public String p_num = String.Empty;
	public Socket client;
	public Boolean StartClient(string username, string password, string session) {
		// Connect to a remote device.
		try {
			// Establish the remote endpoint for the socket.
			// The name of the 
			// remote device is "host.contoso.com".
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
			
			// Create a TCP/IP socket.
			client = new Socket(AddressFamily.InterNetwork,
			                           SocketType.Stream, ProtocolType.Tcp);
			
			// Connect to the remote endpoint.
			client.BeginConnect( remoteEP, 
			                    new AsyncCallback(ConnectCallback), client);
			connectDone.WaitOne(5000);
			connectDone.Reset();
			
			// Send test data to the remote device.

			//UnityEngine.Debug.Log ("test");
			Send(client, "<LOGIN>" + username + "<SEP>" + password + "<SEP>" + session + "<EOF>");
			     sendDone.WaitOne(5000);
			
			// Receive the response from the remote device.
			//Receive(client);
			   //receiveDone.WaitOne(5000);
			StateObject recv_so = new StateObject();
			recv_so.workSocket = client;

			Receive(recv_so);
			receiveDone.WaitOne(5000);
			// Write the response to the console.
			UnityEngine.Debug.Log (recv_so.response);
			//Console.WriteLine("Response received : {0}", response);
			if (recv_so.response == "Success")
			{
				return true;
			}
			//else
			//{
				//Release the socket.
				//try {
				//	client.Shutdown(SocketShutdown.Both);
				//}
				//catch (SocketException e) {
				//	Console.WriteLine ("Socket closed remotely");
				//}
				//client.Close();
			//}
		} catch (Exception e) {
			UnityEngine.Debug.Log(e.ToString ());
			Console.WriteLine(e.ToString());
		}
		return false;
	}

	public void SendGameData(String data)
	{
		try {
			Send (client, data);
		}
		catch (Exception e) {
			Console.WriteLine (e.ToString ());
		}
	}
	
	private static void ConnectCallback(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			Socket client = (Socket) ar.AsyncState;
			
			// Complete the connection.
			client.EndConnect(ar);
			UnityEngine.Debug.Log("connected to");
			UnityEngine.Debug.Log(client.RemoteEndPoint.ToString());
			Console.WriteLine("Socket connected to {0}",
			                  client.RemoteEndPoint.ToString());
			
			// Signal that the connection has been made.
			connectDone.Set();
		} catch (Exception e) {
			Console.WriteLine(e.ToString());
		}
	}
	
	public void Receive(StateObject state) {
		try {
			//UnityEngine.Debug.Log ("calling client.receive");
			// Create the state object.
			//StateObject state = new StateObject();
			//state.workSocket = client;
			Socket client = state.workSocket;
			
			// Begin receiving the data from the remote device.
			client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
			                    new AsyncCallback(ReceiveCallback), state);
		} catch (Exception e) {
			UnityEngine.Debug.Log(e.ToString ());
			Console.WriteLine(e.ToString());
		}
	}
	
	private void ReceiveCallback( IAsyncResult ar ) {
		try {
			//UnityEngine.Debug.Log ("right here");
			// Retrieve the state object and the client socket 
			// from the asynchronous state object.
			StateObject state = (StateObject) ar.AsyncState;
			Socket client = state.workSocket;
			
			// Read data from the remote device.
			int bytesRead = client.EndReceive(ar);
			
			if (bytesRead > 0) {
				// Found a 
				state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
				string content = state.sb.ToString();
				UnityEngine.Debug.Log(content);
				
				String[] message = content.Split(stringSeparators, StringSplitOptions.None);


				if (message.Length >= 2)
				{
					SnakeManager.receiving = false;
					for (int i = 0; i < message.Length-1; i++) {
						if (message[0].Contains("<P#>"))
						{
							string[] temp = message[0].Split(new string[] {"<P#>"}, StringSplitOptions.None);
							p_num = temp[0];
							string[] identify = temp[1].Split(new string[] {"<ID>"}, StringSplitOptions.None);
							ID = identify[0];// + "<ID>";
							state.response = identify[1];
						}
						else if (message [i].Contains ("<FOOD>")) {
							string[] temp = message [i].Split (new string[] {"<FOOD>"}, StringSplitOptions.None);
							Food f = JsonConvert.DeserializeObject<Food> (temp [1]);
							//SpawnFood sf = new SpawnFood();
							//Debug.Log("x: " + f.x + "    y: " +f.y);
							SpawnFood.Get_Info (f.x, f.y);
							
						} else if (message [i].Contains ("<GAME>")) {
							string[] temp = message [i].Split (new string[] {"<GAME>"}, StringSplitOptions.None);
							string[] move = temp [1].Split (new string[] {"<SEP>"}, StringSplitOptions.None);
							//Snake2 other = (Snake2)players [move [1]].GetComponent (typeof(Snake2));
							SnakeManager.Snake2_Info(move[1], Convert.ToSingle (move [2]), Convert.ToSingle (move [3]), Convert.ToSingle (move [4]), Convert.ToSingle (move [5]));
							//other.Get_Info (Convert.ToSingle (move [2]), Convert.ToSingle (move [3]), Convert.ToSingle (move [4]), Convert.ToSingle (move [5]));
							
						} else if (message [i].Contains ("<SCORE1>")) {
							string[] temp = message [i].Split (new string[] {"<SCORE1>"}, StringSplitOptions.None);
							//GameObject Snake2 = GameObject.Find ("Head");
							//Snake2 p1 = GameObject.Find("Head");
							int num = int.Parse (temp [1]);
							SnakeManager.Snake2_Score("Head", num);
							//InsertScore ("player1", num);
							//Snake instanceOfSnake = GameObject.Find ("Head").GetComponent<Snake>();
							//instanceOfSnake.Playerscore1(num);
							//Debug.Log ("score sent");
							//Snake2 = GetComponent<Snake2>().Playerscore1(num);
							
						} else if (message [i].Contains ("<SCORE2>")) {
							string[] temp = message [i].Split (new string[] {"<SCORE2>"}, StringSplitOptions.None);
							//GameObject Snake2 = GameObject.Find ("Head");
							//Snake2 p1 = GameObject.Find("Head");
							int num = int.Parse (temp [1]);
							SnakeManager.Snake2_Score("Head 1", num);
							//InsertScore ("player2", num);
							//Snake instanceOfSnake = GameObject.Find ("Head 1").GetComponent<Snake>();
							//instanceOfSnake.Playerscore1(num);
							//Debug.Log ("score sent");
							//Snake2 = GetComponent<Snake2>().Playerscore1(num);
							
						} else if (message [i].Contains ("<SCORE3>")) {
							string[] temp = message [i].Split (new string[] {"<SCORE2>"}, StringSplitOptions.None);
							//GameObject Snake2 = GameObject.Find ("Head");
							//Snake2 p1 = GameObject.Find("Head");
							int num = int.Parse (temp [1]);
							SnakeManager.Snake2_Score("Head 2", num);
							//InsertScore ("player3", num);
							//Snake instanceOfSnake = GameObject.Find ("Head 1").GetComponent<Snake>();
							//instanceOfSnake.Playerscore1(num);
							//Debug.Log ("score sent");
							//Snake2 = GetComponent<Snake2>().Playerscore1(num);
							
						} else if (message [i].Contains ("<DISCONNECT>")) {
							string[] temp = message [i].Split (new string[] {"<DISCONNECT>"}, StringSplitOptions.None);
							SnakeManager.Snake2_dc(temp [1]);
						}
						else if (message[i].Contains ("<GAMEOVER>")){
							string[] temp = message [i].Split (new string[] {"<GAMEOVER>"}, StringSplitOptions.None);
							SnakeManager.endgame(temp[1]);
						}
						else
							state.response = message[i];
					}
					state.sb.Length = 0;
					state.sb.Capacity = 0;
				}
					/*receiveDone.Set();
					UnityEngine.Debug.Log (message[0]);
					if (message[0].Contains("<P#>"))
					{
						string[] temp = message[0].Split(new string[] {"<P#>"}, StringSplitOptions.None);
						p_num = temp[0];
						string[] identify = temp[1].Split(new string[] {"<ID>"}, StringSplitOptions.None);
						ID = identify[0];// + "<ID>";
						state.response = identify[1];
					}
					else
						state.response = message[0];
					//string coords = message[0].Split (":", StringSplitOptions.None);

					//state.workSocket.Shutdown(SocketShutdown.Both);
					//state.workSocket.Close();
					
				}
				else if (message.Length > 2)
				{
					for (int i = 1; i < message.Length-1; i++)
						message[0] = message[0] + "<EOF>" + message[i];
					state.response = message[0];
				}*/
				else
				{
					// Get the rest of the data.
					client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
					                    new AsyncCallback(ReceiveCallback), state);
				}
			} else {
				Console.WriteLine("Connection close has been requested.");
				// Signal that all bytes have been received.
				
			}
		} catch (Exception e) {
			Console.WriteLine(e.ToString());
		}
	}
	
	private static void Send(Socket client, String data) {
		// Convert the string data to byte data using ASCII encoding.
		byte[] byteData = Encoding.ASCII.GetBytes(data);
		
		// Begin sending the data to the remote device.
		client.BeginSend(byteData, 0, byteData.Length, 0,
		                 new AsyncCallback(SendCallback), client);
	}
	
	private static void SendCallback(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			Socket client = (Socket) ar.AsyncState;
			
			// Complete sending the data to the remote device.
			int bytesSent = client.EndSend(ar);
			UnityEngine.Debug.Log("sent");
			UnityEngine.Debug.Log(bytesSent);
			Console.WriteLine("Sent {0} bytes to server.", bytesSent);
			
			// Signal that all bytes have been sent.
			sendDone.Set();
		} catch (Exception e) {
			Console.WriteLine(e.ToString());
		}
	}
}

