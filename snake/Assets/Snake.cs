﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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

	// Keep Track of Tail
	List<Transform> tail = new List<Transform>();

	// Grow in next movement?
	bool ate = false;
	
	// Tail Prefab
	public GameObject tailPrefab;

	void OnTriggerEnter2D(Collider2D coll) {
		// Food?
		if (coll.name.StartsWith("FoodPrefab")) {
			// Get longer in next Move call
			ate = true;
			
			// Remove the Food
			Destroy(coll.gameObject);
		}
		// Collided with Tail or Border
		else {
			Application.LoadLevel ("start_menu");
			// ToDo 'You lose' screen
		}
	}

	// Use this for initialization
	void Start () {
		p = new Current_Player (Startgame.Client.ID, transform.position.x,
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
			string output = JsonConvert.SerializeObject(p);
			//UnityEngine.Debug.Log(output);
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
}
