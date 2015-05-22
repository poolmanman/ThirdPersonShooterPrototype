using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TurretEnemy : MonoBehaviour {
	public enum enemyStates {idle, attacking, dead};
	public enemyStates myState;
	float agroRange = 15f;
	public GameObject player;
	bool agro = false;
	bool stateInit = true;
	Stats m_stats;
	Animator m_animator;
	public GameObject prefabProjectile;
	bool shotReady = true;
	float cooldownTime = 1f;

	Slider healthBar;



	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		m_stats = gameObject.GetComponent<Stats>();
		myState = enemyStates.idle;
		m_animator = gameObject.GetComponent<Animator>();
		healthBar = GetComponentInChildren<Slider>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		DeathCheck();
		healthBar.value = m_stats.ReturnHealthFraction();
		switch(myState){
			case enemyStates.idle:
				if(stateInit){
					stateInit = false;
				}
				AgroCheck();
				break;
			case enemyStates.attacking: 
				if(stateInit){
					stateInit = false;
				}
			transform.LookAt(player.transform);
				if(shotReady){
					Shoot();
				}
				break;
			case enemyStates.dead:
				if(stateInit){
					m_animator.SetBool("Dead", true);
					stateInit = false;
				}
				break;
		}


	}

	void OnDrawGizmos(){
		Gizmos.color = new Color(0f,255f,242f, 0.2f);
		Gizmos.DrawSphere(transform.position, agroRange);
	}

	void Shoot(){
		print("Shoot - " + prefabProjectile.ToString());
		GameObject obj = StaticPool.GetObj(prefabProjectile);
		obj.GetComponent<ProjectileTwo>().Reset(gameObject, transform.forward);
		StartCoroutine("Cooldown", cooldownTime);
	}

	IEnumerator Cooldown(float time){
		shotReady = false;
//		print(shotReady.ToString());
		yield return new WaitForSeconds(time);
		
		shotReady = true;
//		print(shotReady.ToString());
	}

	void AgroCheck(){
		if(Vector3.Distance(player.transform.position, transform.position) < agroRange){
			transform.LookAt(player.transform.position);
			myState = enemyStates.attacking;
		}
	}

	void DeathCheck(){
		if(m_stats.health <=0){
			stateInit = true;
			myState = enemyStates.dead;
		}
	}

}
