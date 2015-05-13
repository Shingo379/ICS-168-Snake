using UnityEngine;
using System.Collections;

public class SnakeSpawn : MonoBehaviour {
	public Snake script1;
	public Snake2 script2;
	//script1 = gameObject.GetComponent("Snake") as Snake;
	// Use this for initialization
	void Start () {
		if (gameObject.name == "Head") {
			if (Startgame.Client.ID == "0") {
				script1 = GetComponent<Snake> ();
				script1.enabled = true;
			} else {
				script2 = GetComponent<Snake2> ();
				script2.enabled = true;
			}
		}
		else if (gameObject.name == "Head 1") {
			if (Startgame.Client.ID == "1") {
				script1 = GetComponent<Snake> ();
				script1.enabled = true;
			} else {
				script2 = GetComponent<Snake2> ();
				script2.enabled = true;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
