using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class endgame : MonoBehaviour {

	public Text Win;

	// Use this for initialization
	void Start () {
		Win.text = SnakeManager.winner + " is the winner";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
