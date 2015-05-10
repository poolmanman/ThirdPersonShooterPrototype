using UnityEngine;
using System.Collections;

public class Stats : MonoBehaviour {

	public int health;
	int maxHealth;

	void Start(){
		maxHealth = health;
	}

	public void TakeDamager(int dmgAmt){
		health -= dmgAmt;
	}

	public void Heal(int healAmt){
		health += healAmt;
	}

	public float ReturnHealthFraction(){
		return (float)health/(float)maxHealth;
	}

	public Stats ReturnSelf(){
		return this;
	}
}
