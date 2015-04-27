using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
//using System.Data.SqLite;

public class Server : MonoBehaviour {
	
	public int maxConnections = 512;
	public int serverPort = 9955;
	
	public GameObject target;
	
	public string username = "";
	public string password = "";
	bool RegisterUI = false;
	bool LoginUI = false;
	
	
	
	
	
	void OnGUI()
	{
		
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			if(GUI.Button(new Rect(100,100,100,25),"Start Client"))
			{
				Network.Connect ("localhost", serverPort);
			}
			if(GUI.Button(new Rect(100,125,100,25),"Start Server"))
			{
				Network.InitializeServer(maxConnections, serverPort, true);
			}
		}
		else {
			if(Network.peerType == NetworkPeerType.Client)
			{
				if(RegisterUI == true && LoginUI == false)
				{
					username = GUI.TextArea(new Rect(100,125,110,25),username);
					password = GUI.TextArea(new Rect(100,150,110,25),password);
					//TEST
					//					SqliteConnection.CreateFile ("MyDatabase.sqlite");
					//					SqliteConnection m_dbConnection;
					//					m_dbConnection = 
					//						new SqliteConnection ("Data Source= MyDatabase.sqlite;Version=3;");
					//					m_dbConnection.Open();
					//					string sql = "create table userinfo (name varchar(20), password str)";
					//					SqliteCommand command = new SqliteCommand (sql, m_dbConnection);
					//					command.ExecuteNonQuery ();
					//					sql = "insert into userinfo(name, password) values ('username', 'password')";
					//					command = new SqliteCommand(sql,m_dbConnection);
					//					command.ExecuteNonQuery ();
					//TEST
					if(GUI.Button(new Rect(100,175,110,25),"Register"))
					{
						GetComponent<NetworkView>().RPC("Register",RPCMode.Server,username,password);
						RegisterUI = false;
					}
				}
				else if(LoginUI == true && RegisterUI == false)
				{
					username = GUI.TextArea(new Rect(100,125,110,25),username);
					password = GUI.TextArea(new Rect(100,150,110,25),password);
					if(GUI.Button(new Rect(100,175,110,25),"Login"))
					{
						GetComponent<NetworkView>().RPC("Login",RPCMode.Server,username,password);
					}
					if(GUI.Button (new Rect(100,300,110,25),"Back"))
					{
						LoginUI = false;
					}
				}
				else {
					
					GUI.Label(new Rect(100,100,100,25),"Client");
					
					if(GUI.Button(new Rect(100,125,110,25),"Login"))
					{
						LoginUI = true;
					}
					
					if(GUI.Button(new Rect(100,150,110,25),"Register"))
					{
						RegisterUI = true;
					}
					
					
					if(GUI.Button(new Rect(100,175,110,25),"Logout"))
					{
						Network.Disconnect(250);	
					}
				}
				
			}
			if(Network.peerType == NetworkPeerType.Server)
			{
				GUI.Label(new Rect(100,100,100,25),"Server");
				GUI.Label(new Rect(100,125,100,25),"Connections: " + Network.connections.Length);
				
				if(GUI.Button(new Rect(100,150,100,25),"Logout"))
				{
					Network.Disconnect(250);	
				}
			}
		}
	}
	
	[RPC]
	void Login(string Username, string Password)
	{
		if(Network.isServer)
		{
			bool checkUsername = PlayerPrefs.HasKey(Username);
			bool checkPassword = PlayerPrefs.HasKey(Password);
			
			if(checkPassword == true && checkUsername == true)
			{
				GetComponent<NetworkView>().RPC("LoadLevel",RPCMode.Others);
			}	
		}
	}
	
	[RPC]
	void LoadLevel()
	{
		if(Network.isClient)
		{
			if(Application.loadedLevel == 0)
			{
				Application.LoadLevel(1);
			}
		}
	}
	
	[RPC]
	void Register(string Username,string Password)
	{
		if(Network.isServer)
		{
			PlayerPrefs.SetString(Username,Username);
			PlayerPrefs.SetString(Password,Password);
		}
	}
	
}
