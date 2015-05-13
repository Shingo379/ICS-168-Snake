using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SnakeManager : MonoBehaviour {

	public class Other_Player{
		public Other_Player(string i, float start_x, float start_y, float start_dirx, float start_diry)
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
	}

	public static Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 4; i++) {
			if (Startgame.Client.ID != i.ToString()){
				if (i == 0){
					players.Add(i.ToString(), GameObject.Find("Head"));
				}
				else if (i == 1){
					players.Add(i.ToString(), GameObject.Find("Head 1"));
				}
				//else if (i == 2){
				//	players.Add(i.ToString(), GameObject.Find("Head 2"));
				//}
				//else if (i == 3){
				//	players.Add(i.ToString(), GameObject.Find("Head 3"));
				//}
			}
		}
		InvokeRepeating ("Receive_Manager", 0.15f, 0.15f);
	}
	
	// Update is called once per frame
	void Update () {

	}
	void Receive_Manager() {
		string text = Startgame.Receive ();
		if (text.Contains("<GAME>")) {
			string[] temp = text.Split(new string[] {"<GAME>"}, StringSplitOptions.None);
			Other_Player p = JsonConvert.DeserializeObject<Other_Player> (temp[1]);
			Snake2 other = (Snake2)players [p.ID].GetComponent (typeof(Snake2));
			other.Get_Info (p.x, p.y, p.dir_x, p.dir_y);
		}
	}
}
