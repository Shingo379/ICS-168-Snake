using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Chatbox : MonoBehaviour {
	public InputField chat;
	public InputField session;
	public Text chat_text;
	List<string> messages = new List<string>();
	public string username;
	public string id;
	bool receiving;
	public static bool in_lobby = false;
	public static string input = string.Empty;
	public Transform button;
	// Use this for initialization
	void Start () {
		username = Startgame.user;
		id = Startgame.id;
		in_lobby = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!receiving) {
			input = Startgame.Receive ();
			receiving = true;
		}
		//Debug.Log (input);
		if (input == "<HEARTBEAT>") {
			Debug.Log (input);
			Application.LoadLevel ("game");
		} else if (input == "Success" || input == "<GAMESTART>") {
			button.GetComponent<Button>().interactable = false;
			receiving = false;
			input = string.Empty;
		}
		else if (input != string.Empty) {
			Debug.Log("input is:" + input);
			Display(input);
			receiving = false;
			input = string.Empty;
		}
	}

	public void Enter() {
		if (chat.text != string.Empty) {
			//Debug.Log(chat.text);
			//Display (username + ":" + chat.text);
			UnityEngine.Debug.Log(Startgame.Client.p_num +"<MESSAGE>" + username + ":" + chat.text);
			Startgame.Client.SendGameData (id +"<MESSAGE>" + username + ":" + chat.text + "<EOF>");
			chat.text = string.Empty;
		}
	}

	public void Display(string text){
		if (messages.Count == 9)
			messages.RemoveAt (0);
		messages.Add (text);
		Debug.Log ("added:" + text);
		string asd = string.Join ("\n", messages.ToArray ());
		chat_text.text = asd;
		//Startgame.Client.SendGameData ("<MESSAGE>" + text);
	}

	public void Join(){
		Startgame.Client.SendGameData ("<LOGIN>" + username + "<SEP>" + id + "<SEP>" + session.text + "<EOF>");

	}

	public static void Begin(){
		in_lobby = false;
		Application.LoadLevel ("game");
	}
}
