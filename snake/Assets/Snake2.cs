using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.UI;

public class Snake2 : MonoBehaviour {
	//	AsynchronousClient aclient = new AsynchronousClient ();
	//	Socket client;
	//bool c = aclient.Send(
	// Current Movement Direction
	// (by default it moves to the right)
	string ID = string.Empty;
	Vector2 dir = Vector2.right;
	bool change = false;
	Vector2 temp = Vector2.right;
	
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
			if (coll.name.StartsWith ("FoodPrefab")){
				ate = true;
				Destroy(coll.gameObject);
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
				
			}/*
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
			} */

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
		yield return new WaitForSeconds(0);
		transform.position = new Vector2(-21,15);
		transform.rotation = Quaternion.identity;
		Debug.Log ("respawn");
		rend.enabled = true;
	}
	
	// Use this for initialization
	void Start () {
		// Move the Snake every 300ms
		if (gameObject.name == "Head") {
			string ID = "0";
		}
		else if (gameObject.name == "Head 1") {
			string ID = "1";
		}
		InvokeRepeating("Move", 0.15f, 0.15f);    
	}
	
	// Update is called once per frame
	void Update () {
		// Move in a new Direction?
		
		//string coords = Startgame.Receive ();
		
		
		//int x
		//coords.Split (":", System.StringSplitOptions.None);
		
		
		//		if (Input.GetKey(KeyCode.RightArrow) && dir != -Vector2.right)
		//			dir = Vector2.right;
		//		else if (Input.GetKey(KeyCode.DownArrow) && dir != Vector2.up)
		//			dir = -Vector2.up;    // '-up' means 'down'
		//		else if (Input.GetKey(KeyCode.LeftArrow) && dir != Vector2.right)
		//			dir = -Vector2.right; // '-right' means 'left'
		//		else if (Input.GetKey(KeyCode.UpArrow) && dir != -Vector2.up)
		//			dir = Vector2.up;
		
		//Startgame.Client.SendGameData (dir.x.ToString() + ":" + dir.y.ToString()+"<EOF>");
	}
	
	void Move() {
		// Save current position (gap will be here)
		Vector2 v = transform.position;
		
		// Move head into new direction (now there is a gap)
		if (change) {
			temp.x = temp.x - v.x;
			temp.y = temp.y - v.y;
			transform.Translate(temp);
			change = false;
		}
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
	
	public void Get_Info(float dir_x, float dir_y, float x, float y){
		Debug.Log ("updating positions");
		dir.x = dir_x;
		dir.y = dir_y;
		temp.Set(x, y);
		change = true;
		//transform.Translate (temp);
	}
	public void Playerscore1(int Pscores1)
	{
		scores1 = Pscores1;
		player1Score.text = "Player 1: " + Pscores1;
		if (Pscores1 == 15) {
			//Application.LoadLevel ();
		}
		//Debug.Log ("ONE POINT");
	}
	public void Playerscore2 (int Pscores2)
	{
		player2Score.text = "Player 2: " + Pscores2;
		if (Pscores2 == 15) {
			//Application.LoadLevel ();
		}
	}
	public void Playerscore3 (int Pscores3)
	{
		player2Score.text = "Player 3: " + Pscores3;
		if (Pscores3 == 15) {
			//Application.LoadLevel ();
		}
	}
}
