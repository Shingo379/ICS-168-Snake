using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Text;
using System.Security.Cryptography;


public class Startgame : MonoBehaviour {
	
	public InputField username;
	public InputField password;
	
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
	void Update () {

	}
	public void LoadScene()  {
		MD5 md5Hash = MD5.Create();
		string hash_pass = GetMd5Hash (md5Hash, password.text);
		Boolean client = AsynchronousClient.StartClient (username.text, hash_pass);
		if (client == true) {
			Application.LoadLevel ("game");
		} 
	}

}
