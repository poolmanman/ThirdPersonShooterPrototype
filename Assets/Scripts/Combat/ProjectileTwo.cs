using UnityEngine;
using System.Collections;

public class ProjectileTwo : MonoBehaviour {
	Transform cam;
	Transform player;
	Rigidbody m_body;
	Vector3 startPos;

	void Awake(){
//		cam = GameObject.FindGameObjectWithTag("MainCamera");
		cam = GameObject.Find("Cam").transform;
		player = GameObject.Find("Player").transform;
		m_body = gameObject.GetComponent<Rigidbody>();
	}
	// Use this for initialization
	void Start () {
//
//		Reset();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//destroy once iv traveled given distance;
		if(Vector3.Distance(transform.position, startPos) > 60f){
			gameObject.SetActive(false);
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.GetComponent<Stats>() != null){
			other.GetComponent<Stats>().TakeDamager(3);
		}
		gameObject.SetActive(false);
	}
	
	public void Reset(GameObject whoShotMe, Vector3 dir){
		m_body.velocity = Vector3.zero;
		transform.position = new Vector3(whoShotMe.transform.position.x, whoShotMe.transform.position.y + 1.5f, whoShotMe.transform.position.z) + whoShotMe.transform.forward * 1f;
		startPos = transform.position;
		transform.forward = dir;
//		print (cam.forward);
		m_body.AddForce(transform.forward * 700f);
	}

	void OnEnable(){
//		Reset();
	}
}
