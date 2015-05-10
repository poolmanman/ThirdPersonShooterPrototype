using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour {

	public Slider playerHealthBar;
	GameObject player;
	Stats playerStats;


	// Use this for initialization
	void Start () {
		print ("Start");
		player = GameObject.FindGameObjectWithTag("Player"); // this is terrible drag and drop u lazy fuk
		playerStats = player.GetComponent<Stats>();
	}
	
	// Update is called once per frame
	void Update () {
		print ("update you fuk");
		print(playerStats.ReturnHealthFraction());
		playerHealthBar.value = playerStats.ReturnHealthFraction();
	}


}
