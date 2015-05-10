using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public Vector3 startMarker;
	public Vector3 endMarker;
	Transform cam;
	Transform player;
	float speed = 90.0F;
	private float startTime;
	Ray m_ray;
	Vector3 pt;
	private float journeyLength;
	void Awake(){
		cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
		player = GameObject.Find("Player").GetComponent<Transform>();
	}
	void Start() {;
//		startMarker = GameObject.Find("Player").transform.position;
//		endMarker = GameObject.Find("Player").transform.forward * 40f;
//		m_ray = new Ray(new Vector3(player.position.x, player.position.y + 2f, player.position.z), new Vector3(cam.forward.x, cam.forward.y, cam.forward.z) * 40f);
//		pt = new Vector3((m_ray.origin.x + (m_ray.direction.x * 40f)), (m_ray.origin.y + (m_ray.direction.y * 40f)),(m_ray.origin.z + (m_ray.direction.z * 40f)));
		Reset();
		journeyLength = Vector3.Distance(startMarker, endMarker);
	}
	void Update() {
//		print (endMarker);
//		float distCovered = (Time.time - startTime) * speed;
//		float fracJourney = distCovered / journeyLength;
//		transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney);
//
//		if(transform.position == endMarker){
//			gameObject.SetActive(false);
//		}

	}

	void FixedUpdate(){
		print (endMarker);
		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = distCovered / journeyLength;
		transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney);
		
		if(transform.position == endMarker){
			gameObject.SetActive(false);
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.GetComponent<Stats>() != null){

		}
	}

	public void Reset(){
//		m_ray = new Ray(new Vector3(player.position.x, player.position.y + 2f, player.position.z), new Vector3(cam.forward.x, cam.forward.y, cam.forward.z) * 40);
//		print ("D = " + m_ray.direction);
//		pt = m_ray.GetPoint(40f);
		startTime = Time.time;
		startMarker = new Vector3(player.position.x, player.position.y + 2f, player.position.z);
		endMarker = cam.forward * 40f;
		transform.forward = cam.forward;
		gameObject.transform.position = startMarker;
		journeyLength = Vector3.Distance(startMarker, endMarker);

	}	

	void OnEnable(){
//		m_ray = new Ray(new Vector3(player.position.x, player.position.y + 2f, player.position.z), new Vector3(cam.forward.x, cam.forward.y, cam.forward.z) * 40f);
//		pt = new Vector3((m_ray.origin.x + m_ray.direction.x), (m_ray.origin.y + m_ray.direction.y),(m_ray.origin.z + m_ray.direction.z));

		Reset();
	}


}
