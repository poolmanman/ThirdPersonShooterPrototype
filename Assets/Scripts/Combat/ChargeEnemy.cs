using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChargeEnemy : MonoBehaviour {
	public enum enemyStates {idle, charge, attacking, dead};
	public enemyStates myState;
	float agroRange = 15f;
	GameObject player;
	bool agro = false;
	bool stateInit = true;
	Stats m_stats;
	Animator m_animator;
	public GameObject prefabProjectile;
	bool shotReady = true;
	float cooldownTime = 1f;

	Slider healthBar;
	NavMeshAgent myNavMeshAgent;

	void Start(){
		player = GameObject.FindGameObjectWithTag("Player");
		m_stats = gameObject.GetComponent<Stats>();
		m_animator = gameObject.GetComponent<Animator>();
		myNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
		healthBar = GetComponentInChildren<Slider>();
		myState = enemyStates.idle;
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
		case enemyStates.charge:
			if(stateInit){
				m_animator.SetBool("Charge", true);
				stateInit = false;
			}
			myNavMeshAgent.SetDestination(player.transform.position);
		
			transform.LookAt(player.transform);
			if(AttackRangeCheck()){
				myState = enemyStates.attacking;
			}

			break;
		case enemyStates.attacking: 
			if(stateInit){
				myNavMeshAgent.Stop();
				m_animator.SetBool("Attacking", true);
				stateInit = false;
			}
			if(!AttackRangeCheck()){
				stateInit = true;
				m_animator.SetBool("Attacking", false);
				myState = enemyStates.charge;
			}

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
//		print("Shoot - " + prefabProjectile.ToString());
		GameObject obj = StaticPool.GetObj(prefabProjectile);
		obj.GetComponent<ProjectileTwo>().Reset(gameObject, transform.forward);
		StartCoroutine("Cooldown", cooldownTime);
	}
	

	IEnumerator Cooldown(float time){
		shotReady = false;
		yield return new WaitForSeconds(time);
		shotReady = true;
	}
	
	void AgroCheck(){
		if(Vector3.Distance(player.transform.position, transform.position) < agroRange){
			transform.LookAt(player.transform.position);
			myState = enemyStates.charge;
			stateInit = true;
		}
	}

	bool AttackRangeCheck(){
		if(Vector3.Distance(player.transform.position, transform.position) < 6f){
			m_animator.SetBool("Attacking", true);
			return true;
			myState = enemyStates.attacking;
		}
		else return false;
	}
	
	void DeathCheck(){
		if(m_stats.health < 0){
			stateInit = true;
			myState = enemyStates.dead;
		}
	}
	
}
