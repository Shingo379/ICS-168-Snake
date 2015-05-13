using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

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
	public Socket client;
	public Boolean StartClient(string username, string password) {
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
			Send(client, "<LOGIN>" + username + "<SEP>" + password + "<EOF>");
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


				if (message.Length == 2)
				{
					receiveDone.Set();
					UnityEngine.Debug.Log (message[0]);
					if (message[0].Contains("<ID>"))
					{
						string[] temp = message[0].Split(new string[] {"<ID>"}, StringSplitOptions.None);
						ID = temp[0];// + "<ID>";
						state.response = temp[1];
					}
					else if (message[0].Contains("<GAME>"))
					{
						UnityEngine.Debug.Log ("GAME");
						//string[] temp = message[0].Split(new string[] {"<GAME>"}, StringSplitOptions.None);
						//UnityEngine.Debug.Log(temp[1]);
						state.response = message[0];
					}
					else
						state.response = message[0];
					//string coords = message[0].Split (":", StringSplitOptions.None);

					//state.workSocket.Shutdown(SocketShutdown.Both);
					//state.workSocket.Close();
					
				}
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

