using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Timers;
using UnityEngine.UI;

public class Current_Player{
	public Current_Player(string i, float start_x, float start_y, float start_dirx, float start_diry)
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
public class Snake : MonoBehaviour {
	//	AsynchronousClient aclient = new AsynchronousClient ();
	//	Socket client;
	//bool c = aclient.Send(
	// Current Movement Direction
	// (by default it moves to the right)
	Vector2 dir = Vector2.right;
	Vector2 old_dir;
	Current_Player p;
	
	public int scores1 = 0;
	public int scores2 = 0;
	public int scores3 = 0;
	public int hit = 5;
	
	public Text player1Score, player2Score, player3Score;
	
	// Keep Track of Tail
	List<Transform> tail = new List<Transform>();
	
	// Grow in next movement?
	bool ate = false;
	
	// Tail Prefab
	public GameObject tailPrefab;
	
	void OnTriggerEnter2D(Collider2D coll) {
		if (enabled) {
			// Food?
			//first player scores get updated. need to send the info after score++ to server side and receive it to be updated?
			if ((this.gameObject.name == "Head") && (coll.name.StartsWith ("FoodPrefab"))) {
				//if (coll.name.StartsWith("FoodPrefab"))
				// Get longer in next Move call
				ate = true;
			
				scores1 ++;
				// Remove the Food
				Destroy (coll.gameObject);
				Debug.Log ("sending");
				Startgame.Client.SendGameData(Startgame.Client.p_num+"<ID>" + "<SCORE1>" + scores1 + "<EOF>");
				player1Score.text = "Player 1: " + scores1;
			}
		//the second player scores get updated. need to send the info after score++ to server side and receive it to be updated?
			else if ((gameObject.name == "Head 1") && (coll.name.StartsWith ("FoodPrefab"))) {
					ate = true;
					scores2 ++;
					Destroy (coll.gameObject);
					Startgame.Client.SendGameData(Startgame.Client.p_num+"<ID>" + "<SCORE2>" + scores2 + "<EOF>");
					player2Score.text = "Player 2: " + scores2;
				
				}
			else if ((gameObject.name == "Head 2") && (coll.name.StartsWith ("FoodPrefab"))) {
				ate = true;
				scores3 ++;
				Destroy (coll.gameObject);
				Startgame.Client.SendGameData(Startgame.Client.p_num+"<ID>" + "<SCORE3>" + scores3 + "<EOF>");
				player3Score.text = "Player 3: " + scores3;				
			}
			// Player 2 collides with player 1
			else if ((this.gameObject.name == "Head 1") && (coll.name.StartsWith ("TailPrefabP1"))) {
					//scores2 -= hit;
					StartCoroutine (DieAndRespawn ());
					//Application.LoadLevel("game");
					//player2Score.text = "Player 2: " + scores2;
				}
			// player 1 collides with player 2
			else if ((this.gameObject.name == "Head") && (coll.name.StartsWith ("TailPrefabP2"))) {
					//scores1 -= hit;
					StartCoroutine (DieAndRespawn ());
					//Application.LoadLevel("game");
					//player1Score.text = "Player 1: " + scores1;
				
				}
			else if ((this.gameObject.name == "Head 2") && (coll.name.StartsWith ("TailPrefabP1"))) {
				//scores1 -= hit;
				StartCoroutine (DieAndRespawn ());
				//Application.LoadLevel("game");
				//player1Score.text = "Player 1: " + scores1;
				
			}
			else if ((this.gameObject.name == "Head 2") && (coll.name.StartsWith ("TailPrefabP2"))) {
				//scores1 -= hit;
				StartCoroutine (DieAndRespawn ());
				//Application.LoadLevel("game");
				//player1Score.text = "Player 1: " + scores1;
				
			}
			else if ((this.gameObject.name == "Head") && (coll.name.StartsWith ("TailPrefabP3"))) {
				//scores1 -= hit;
				StartCoroutine (DieAndRespawn ());
				//Application.LoadLevel("game");
				//player1Score.text = "Player 1: " + scores1;
				
			} else if ((this.gameObject.name == "Head 1") && (coll.name.StartsWith ("TailPrefabP3"))) {
				//scores1 -= hit;
				StartCoroutine (DieAndRespawn ());
				//Application.LoadLevel("game");
				//player1Score.text = "Player 1: " + scores1;
				
			} 
			else if ((this.gameObject.name == "Head") && (coll.name.StartsWith ("TailPrefabP1"))) {
				//scores1 -= hit;
				StartCoroutine (DieAndRespawn ());
				//Application.LoadLevel("game");
				//player1Score.text = "Player 1: " + scores1;
			
			} else if ((this.gameObject.name == "Head 1") && (coll.name.StartsWith ("TailPrefabP2"))) {
				//scores1 -= hit;
				StartCoroutine (DieAndRespawn ());
				//Application.LoadLevel("game");
				//player1Score.text = "Player 1: " + scores1;
			
			} 
			else if ((this.gameObject.name == "Head") && (coll.name.StartsWith ("TailPrefabP3"))) {
				//scores1 -= hit;
				StartCoroutine (DieAndRespawn ());
				//Application.LoadLevel("game");
				//player1Score.text = "Player 1: " + scores1;
			
		}
			/*
			else if ((this.gameObject.name == "Head") && (coll.name.StartsWith ("BorderTop")) || (coll.name.StartsWith ("BorderLeft")) || (coll.name.StartsWith ("BorderRight")) || (coll.name.StartsWith ("BorderBottom"))) {
				//scores1 -= hit;
				StartCoroutine (DieAndRespawn ());
				//Application.LoadLevel ("game");
			} else if ((this.gameObject.name == "Head 1") && (coll.name.StartsWith ("BorderTop")) || (coll.name.StartsWith ("BorderLeft")) || (coll.name.StartsWith ("BorderRight")) || (coll.name.StartsWith ("BorderBottom"))) {
				//scores2 -= hit;
				StartCoroutine (DieAndRespawn ());
				//Application.LoadLevel ("game");
			}
			else if ((this.gameObject.name == "Head 2") && (coll.name.StartsWith ("BorderTop")) || (coll.name.StartsWith ("BorderLeft")) || (coll.name.StartsWith ("BorderRight")) || (coll.name.StartsWith ("BorderBottom"))) {
				//scores1 -= hit;
				StartCoroutine (DieAndRespawn ());
				//Application.LoadLevel ("game");
			}
			*/

			else if ((coll.name.StartsWith ("BorderTop")) || (coll.name.StartsWith ("BorderLeft")) || (coll.name.StartsWith ("BorderRight")) || (coll.name.StartsWith ("BorderBottom")))
			{
				StartCoroutine (DieAndRespawn());
			} 

		}
	}
	IEnumerator DieAndRespawn() {
		Debug.Log ("dead");
		Renderer rend = GetComponent<Renderer> ();
		rend.enabled = false;
		yield return new WaitForSeconds(2);
		transform.position = new Vector2(-21,15);
		transform.rotation = Quaternion.identity;
		Debug.Log ("respawn");
		rend.enabled = true;
	}
	
	// Use this for initialization
	void Start () {
		p = new Current_Player (Startgame.Client.p_num, transform.position.x,
		                        transform.position.y, dir.x, dir.y);
		// Move the Snake every 300ms
		InvokeRepeating("Move", 0.15f, 0.15f);    
	}
	
	// Update is called once per frame
	void Update () {
		// Move in a new Direction?
		
		old_dir = dir;
		
		if (Input.GetKey(KeyCode.RightArrow) && dir != -Vector2.right)
			dir = Vector2.right;
		else if (Input.GetKey(KeyCode.DownArrow) && dir != Vector2.up)
			dir = -Vector2.up;    // '-up' means 'down'
		else if (Input.GetKey(KeyCode.LeftArrow) && dir != Vector2.right)
			dir = -Vector2.right; // '-right' means 'left'
		else if (Input.GetKey(KeyCode.UpArrow) && dir != -Vector2.up)
			dir = Vector2.up;
		
		if (old_dir != dir) {
			Update_Player();
			//string output = JsonConvert.SerializeObject(p);
			//UnityEngine.Debug.Log(output);
			string output = p.ID + "<SEP>" + Startgame.Client.ID + "<SEP>" + p.dir_x + "<SEP>" + p.dir_y + "<SEP>" + p.x + "<SEP>" + p.y;
			Startgame.Client.SendGameData("<GAME>" + output + "<EOF>");
		}
	}
	
	void Move() {
		// Save current position (gap will be here)
		Vector2 v = transform.position;
		
		// Move head into new direction (now there is a gap)
		transform.Translate(dir);
		
		// Ate something? Then insert new Element into gap
		if (ate) {
			// Load Prefab into the world
			GameObject g =(GameObject)Instantiate(tailPrefab,
			                                      v,
			                                      Quaternion.identity);
			
			// Keep track of it in our tail list
			tail.Insert(0, g.transform);
			
			// Reset the flag
			ate = false;
		}
		// Do we have a Tail?
		else if (tail.Count > 0) {
			// Move last Tail Element to where the Head was
			tail.Last().position = v;
			
			// Add to front of list, remove from the back
			tail.Insert(0, tail.Last());
			tail.RemoveAt(tail.Count-1);
		}
	}
	void Update_Player() {
		p.x = transform.position.x;
		p.y = transform.position.y;
		p.dir_x = dir.x;
		p.dir_y = dir.y;
	}
	public void Playerscore1(int Pscores1)
	{
		player1Score.text = "Player 1: " + Pscores1;
		//Debug.Log ("ONE POINT");
		if (Pscores1 == 5) 
		{
			//Application.LoadLevel();
		}
	}
	public void Playerscore2 (int Pscores2)
	{
		player2Score.text = "Player 2: " + Pscores2;
		if (Pscores2 == 5) 
		{
			//Application.LoadLevel();
		}
	}
	public void Playerscore3(int Pscores3)
	{
		player3Score.text = "Player 3: " + Pscores3;
		//Debug.Log ("ONE POINT");
		if (Pscores3 == 5) 
		{
			//Application.LoadLevel();
		}
	}
}
