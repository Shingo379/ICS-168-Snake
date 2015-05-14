using UnityEngine;
using System.Collections;

public class Food
{
	public Food(int start_x, int start_y)
	{
		x = start_x;
		y = start_y;
	}
	public int x;
	public int y;
}


public class SpawnFood : MonoBehaviour {
	// Food Prefab
	public GameObject foodPrefab;
	static int x;
	static int y;
	// Borders
	public Transform borderTop;
	public Transform borderBottom;
	public Transform borderLeft;
	public Transform borderRight;
	public static SpawnFood Instance;
	// Spawn one piece of food
	void Spawn() {
		// x position between left & right border
		//int x = (int)Random.Range(borderLeft.position.x,
		//borderRight.position.x);
		
		// y position between top & bottom border
		//int y = (int)Random.Range(borderBottom.position.y,
		//borderTop.position.y);
		
		// Instantiate the food at (x, y)
		//Debug.Log("spawnx: " + x + "    spawny: " +y);
		
		Startgame.Client.SendGameData("<FOOD>" + "<EOF>");
		Instantiate(foodPrefab,
		            new Vector2(x, y),
		            Quaternion.identity); // default rotation
	}
	
	// Use this for initialization
	public void Start () {
		// Spawn food every 4 seconds, starting in 3
		//Instance = this;
		InvokeRepeating("Spawn", 3, 3);
	}
	public static void Get_Info(int X, int Y){
		x = X;
		y = Y;
	}
}
