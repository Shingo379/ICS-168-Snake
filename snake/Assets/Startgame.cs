using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Text;
using System.Security.Cryptography;

public class Startgame : MonoBehaviour {
	public static AsynchronousClient Client = new AsynchronousClient();
	public static StateObject so = new StateObject ();
	public static StateObject recv_so = new StateObject ();
	public InputField username;
	public InputField password;
	public InputField Session;
	Boolean start = false;
	Boolean login = false;
	Text t;

	static string GetMd5Hash(MD5 md5Hash, string input)
	{
		
		// Convert the input string to a byte array and compute the hash. 
		byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
		
		// Create a new Stringbuilder to collect the bytes 
		// and create a string.
		StringBuilder sBuilder = new StringBuilder();
		
		// Loop through each byte of the hashed data  
		// and format each one as a hexadecimal string. 
		for (int i = 0; i < data.Length; i++)
		{
			sBuilder.Append(data[i].ToString("x2"));
		}
		
		// Return the hexadecimal string. 
		return sBuilder.ToString();
	}
	// Use this for initialization
	void Start () {
		/*string conn = "URI=file:" + Application.dataPath + "/login.s3db"; //Path to database.
		IDbConnection dbconn;
		dbconn = (IDbConnection) new SqliteConnection(conn);
		dbconn.Open(); //Open connection to the database.
		IDbCommand dbcmd = dbconn.CreateCommand();
		string sqlQuery = "INSERT INTO logins(username,password) VALUES ('unityname','unitypass');";
		dbcmd.CommandText = sqlQuery;
		dbcmd.ExecuteNonQuery();
		sqlQuery = "SELECT username,password " + "FROM logins";
		dbcmd.CommandText = sqlQuery;
		IDataReader reader = dbcmd.ExecuteReader();

		while (reader.Read())
		{
			string name = reader.GetString(0);
			string pass = reader.GetString(1);

			Debug.Log( "username= "+name+"  password ="+pass);
		}

		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		dbconn.Close();
		dbconn = null;*/
	}
	// Update is called once per frame
	public void Send (String data)
	{
		Client.SendGameData (data);
	}

	public static string Receive ()
	{
		recv_so.sb.Length = 0;
		recv_so.sb.Capacity = 0;
		Client.Receive (recv_so);
		return recv_so.response;
	}


	void Update () {
		if (login == true) {
			string temp = Receive ();
			//UnityEngine.Debug.Log (temp);
			if (temp == "<HEARTBEAT>")
			{
				login = false;
				start = true;
			}
		}
		if (start == true)
			Application.LoadLevel ("game");
	}
	public void LoadScene()  {
		MD5 md5Hash = MD5.Create();
		string hash_pass = GetMd5Hash (md5Hash, password.text);
		Boolean client = Client.StartClient (username.text, hash_pass, Session.text);
		so.workSocket = Client.client;
		recv_so.workSocket = Client.client;
		if (client == true) {
			t = GetComponentInChildren<Text>();
			t.text = "Connected";
			login = true;
		} 
	}

}
