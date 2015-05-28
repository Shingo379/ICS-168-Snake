using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Mono.Data.Sqlite;

public class SnakeManager : MonoBehaviour {

	public static Boolean receiving = false;
	public static string winner = string.Empty;
	
	public class Other_Player{
		public Other_Player(string i, float start_x, float start_y, float start_dirx, float start_diry, string session)
		{
			ID = i;
			x = start_x;
			y = start_y;
			dir_x = start_dirx;
			dir_y = start_diry;
			session = Session;
		}
		public string ID;
		public float x;
		public float y;
		public float dir_x;
		public float dir_y;
		public string Session;
	}
	
	public static Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
	public static Dictionary<string, Snake2> players2 = new Dictionary<string, Snake2>();
	
	// Use this for initialization
	void Start () {
		for (int i = 0; i < 4; i++) {
			if (Startgame.Client.ID != i.ToString()){
				if (i == 0){
					players.Add(i.ToString(), GameObject.Find("Head"));
					players2.Add(i.ToString(), (Snake2)GameObject.Find("Head").GetComponent(typeof(Snake2)));
				}
				else if (i == 1){
					players.Add(i.ToString(), GameObject.Find("Head 1"));
					players2.Add(i.ToString(), (Snake2)GameObject.Find("Head 1").GetComponent(typeof(Snake2)));
				}
				else if (i == 2){
					players.Add(i.ToString(), GameObject.Find("Head 2"));
					players2.Add(i.ToString(), (Snake2)GameObject.Find("Head 2").GetComponent(typeof(Snake2)));
				}
				//else if (i == 3){
				//	players.Add(i.ToString(), GameObject.Find("Head 3"));
				//}
			}
		}
		SqliteConnection.CreateFile ("scores.sqlite");
		SqliteConnection m_dbConnection;
		m_dbConnection = 
			new SqliteConnection ("Data Source= scores.sqlite;Version=3;");
		m_dbConnection.Open();
		string sql1 = "create table Tscores(name varchar, scores int)";
		SqliteCommand command = new SqliteCommand (sql1, m_dbConnection);
		command.ExecuteNonQuery ();
		//command.Dispose ();
		//m_dbConnection.Close ();
		//InvokeRepeating ("Receive_Manager", 0.15f, 0.15f);
	}
	
	// Update is called once per frame
	void Update () {
		if (!receiving) {
			receiving = true;
			string text = Startgame.Receive ();

		}
	}

	public static void Snake2_Info(string ID, float dir_x, float dir_y, float x, float y)
	{
		players2[ID].Get_Info (dir_x, dir_y, x, y);
	}

	public static void Snake2_Score(string name, int score)
	{
		Snake2 instanceOfSnake2 = (Snake2)GameObject.Find (name).GetComponent (typeof(Snake2));
		instanceOfSnake2.Playerscore1 (score);
	}

	public static void Snake2_dc(string name)
	{
		players2 [name].CancelInvoke ();
		//Destroy (players [name]);
	}

	public static void endgame(string player)
	{
		winner = player;
		Application.LoadLevel ("endgame");
	}

	void onStart()
	{
		SqliteConnection.CreateFile ("scores.sqlite");
		SqliteConnection m_dbConnection;
		m_dbConnection = 
			new SqliteConnection ("Data Source= scores.sqlite;Version=3;");
		m_dbConnection.Open();
		string sql1 = "create table Tscores(name varchar, scores)";
		SqliteCommand command = new SqliteCommand (sql1, m_dbConnection);
		command.ExecuteNonQuery ();
		command.Dispose ();
		m_dbConnection.Close ();
	}
	void InsertScore(string names, int score)
	{
		SqliteConnection m_dbConnection;
		m_dbConnection = 
			new SqliteConnection ("Data Source= scores.sqlite;Version=3;");
		m_dbConnection.Open ();
		string sql = "insert into Tscores (name, scores) values ('" + names + "', '" + score + "')";
		SqliteCommand command = new SqliteCommand (sql, m_dbConnection);
		command.ExecuteNonQuery ();
		command.Dispose ();
		m_dbConnection.Close ();
		
		//command1.Dispose();
		
	}
	void Receive_Manager() {
		
	}
}
