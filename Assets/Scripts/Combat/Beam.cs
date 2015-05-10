using UnityEngine;
using System.Collections;

public class Beam : MonoBehaviour {

	public Vector3 startMarker;
	public Vector3 endMarker;

	public Vector3 startScale;
	public Vector3 endScale;
	Transform cam;
	Transform player;
	float speed = 20.0F;
	private float startTime;
	Ray m_ray;
	Vector3 pt;
	private float journeyLength;
	private float scaleJourneyLength;
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
		float distScaled = (Time.time - startTime) * speed;
		float fracScaled = distScaled / scaleJourneyLength;
		float fracJourney = distCovered / journeyLength;
		transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney);
		transform.localScale = Vector3.Lerp(startScale, endScale, fracScaled);
		
		if(transform.position == endMarker){
			gameObject.SetActive(false);
		}
	}

	//Deals 3 damage for now
	void OnTriggerEnter(Collider other){
		if(other.gameObject.GetComponent<Stats>() != null){
			other.GetComponent<Stats>().TakeDamager(3);
		}
	}
	
	public void Reset(){
		//		m_ray = new Ray(new Vector3(player.position.x, player.position.y + 2f, player.position.z), new Vector3(cam.forward.x, cam.forward.y, cam.forward.z) * 40);
		//		print ("D = " + m_ray.direction);
		//		pt = m_ray.GetPoint(40f);
		startTime = Time.time;
		startMarker = new Vector3(player.position.x, player.position.y + 2f, player.position.z);
		startScale = transform.localScale;
		endMarker = cam.forward * 20f;
		endScale = new Vector3(startScale.x, startScale.y, startScale.z * 20f);
		gameObject.transform.position = startMarker;
		journeyLength = Vector3.Distance(startMarker, endMarker);
		scaleJourneyLength = Vector3.Distance(startScale, endScale);
		
	}	
	
	void OnEnable(){
		//		m_ray = new Ray(new Vector3(player.position.x, player.position.y + 2f, player.position.z), new Vector3(cam.forward.x, cam.forward.y, cam.forward.z) * 40f);
		//		pt = new Vector3((m_ray.origin.x + m_ray.direction.x), (m_ray.origin.y + m_ray.direction.y),(m_ray.origin.z + m_ray.direction.z));
		
		Reset();
	}
}
