using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
//using System.Data.SqLite;

public class Menu : MonoBehaviour {
	
	public int maxConnections = 512;
	public int serverPort = 9955;
	
	public GameObject target;
	
	public string username = "test123";
	public string password = "test12";
	bool RegisterUI = false;
	bool LoginUI = false;

	void Start()
	{
		SqliteConnection.CreateFile ("MyDatabase.sqlite");
		SqliteConnection m_dbConnection;
		m_dbConnection = 
			new SqliteConnection ("Data Source= MyDatabase.sqlite;Version=3;");
		m_dbConnection.Open();
		string sql = "create table userinfo(name varchar not null unique, password varchar not null, primary key(name))";
		//string sql = "create table userinfo (name varchar(20), password str)";
		SqliteCommand command = new SqliteCommand (sql, m_dbConnection);
		command.ExecuteNonQuery ();


	}
	void FillInfo(string username, string password)
	{
		SqliteConnection m_dbConnection;
		m_dbConnection = 
			new SqliteConnection ("Data Source= MyDatabase.sqlite;Version=3;");
		m_dbConnection.Open ();

		string sql = "insert into userinfo(name, password) values ('" + username + "', '" + password + "')";
		
		SqliteCommand command = new SqliteCommand (sql, m_dbConnection);
		command.ExecuteNonQuery ();
//						string sql2 = "delete from userinfo where name'" + this.username + "EQUALS" + this.username + "'";
//				command = new SqliteCommand(sql2,m_dbConnection);
//				command.ExecuteNonQuery();


	}
	void GetInfo(string username, string password)
	{
		SqliteConnection m_dbConnection;
		m_dbConnection = 
			new SqliteConnection ("Data Source= MyDatabase.sqlite;Version=3;");
		m_dbConnection.Open ();
		try
		{
			string sql = "select * from userinfo where name= '" + this.username + "' and password= '" + this.password + "'";
			SqliteCommand command = new SqliteCommand(sql,m_dbConnection);
			command.ExecuteNonQuery();
			SqliteDataReader dr = command.ExecuteReader();

			int count = 0;
			while(dr.Read())
			{
				count++;
			}
			if(count == 1)
			{

				//GUI.TextArea(new Rect(1,1,200,200),"Username and password is correct");
				//GetComponent<NetworkView>().RPC("LoadLevel",RPCMode.Others);
				LoadLevelScene();
			}
			if(count > 1)
			{
//				string sql2 = "delete from userinfo where name'" + this.username + "EQUALS" + this.username + "'";
//				command = new SqliteCommand(sql2,m_dbConnection);
//				command.ExecuteNonQuery();
				LoginUI = true;
				//GUI.TextArea(new Rect(1,1,200,200),"Duplicate Username and password");
				//GetComponent<NetworkView>().RPC("LoadLevel",RPCMode.Others);
				//LoadLevelScene ();
			}
			if(count < 1)
			{

				FillInfo(username, password);
//				string sql2 = "delete from userinfo where name'" + this.username + "EQUALS" + this.username + "'";
//				command = new SqliteCommand(sql2,m_dbConnection);
//				command.ExecuteNonQuery();
				LoadLevelScene ();
				//GetInfo();
				//GetComponent<NetworkView>().RPC("LoadLevel",RPCMode.Others);
				//GUI.TextArea(new Rect(1,1,200,200),"Username and password is not correct");
			}
//			string sql2 = "select * from userinfo where name = '" + this.username +"'";
//			command = new SqliteCommand(sql2,m_dbConnection);
//			command.ExecuteNonQuery();
//			SqliteDataReader dr1 = command.ExecuteReader();
//			int count2 = 0;
//			while(dr1.Read ())
//			{
//				count2++;
//			}
		}
		catch
		{
			print("Error");
		}
	}

	
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


					if(GUI.Button(new Rect(100,175,110,25),"Register"))
					{	//Write into sqlite the username and password
						FillInfo (username, password);
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
						GetInfo (username, password);
						//FillInfo ();
						//GetComponent<NetworkView>().RPC("Login",RPCMode.Server,username,password);
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
	void LoadLevelScene()
	{
		Application.LoadLevel (1);
	}


	//This basically check username and password if its the same from the register.. read the code and you will understand
	[RPC]
	void Login(string username, string password)
	{
		if(Network.isServer)
		{
			GetInfo(username,password);
//			bool checkUsername = PlayerPrefs.HasKey(Username);
//			bool checkPassword = PlayerPrefs.HasKey(Password);
//			
//			if(checkPassword == true && checkUsername == true)
//			{
//				GetComponent<NetworkView>().RPC("LoadLevel",RPCMode.Others);
//			}	
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
	//need to put the check sqlite database here.
	//this function assigns the temporary variable we input on register to Username and Password 
	//so we need it to access database to check if a username and password is in there
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
