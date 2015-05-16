using UnityEngine;
using System.Collections;

public class AttackCollider : MonoBehaviour {
	public PlayerController2 player;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "Enemy"){
			player.AddToAttackList(other.gameObject.GetComponent<Stats>());
		}
	}
	void OnTriggerExit(Collider other){
		if(other.gameObject.tag == "Enemy"){
			player.RemoveFromAttackList(other.gameObject.GetComponent<Stats>());
		}
	}

}
