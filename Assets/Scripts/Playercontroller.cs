using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Cameras;
using UnityEngine.UI;

public class Playercontroller : MonoBehaviour {

	enum playerState{idle, run, jump}
	playerState myState;

	public float speed;
	Transform cam;
	Vector3 moveDir;
	bool shotReady = true;
	float cooldownTime = 1f;
	public GameObject prefabProjectile;
	public GameObject lookTransform;
	Animator m_animator;
	bool init = true;

	public GameObject targetMarker; //set in inspector
	GameObject target;
	GameObject[] enemies;
	List<GameObject> enemiesInSight;

	Stats m_Stats;

	public FreeLookCam camScript;

	Rigidbody body;

	bool ascending = false;

	StaticPool staticPool;
	void Awake(){
		staticPool = new StaticPool();
	}
	// Use this for initialization
	void Start () {

		targetMarker.SetActive(false);
		cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
		m_animator = gameObject.GetComponent<Animator>();
		body = gameObject.GetComponent<Rigidbody>();
		m_Stats = gameObject.GetComponent<Stats>();
	}
	
	// Update is called once per frame
	void Update () {
		//print(cam.forward);

	}

	void FixedUpdate(){
		Debug.DrawLine(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), cam.forward * 40f);
		Debug.DrawLine(cam.position, cam.forward * 40, Color.red);
//		if(Input.GetButton("Fire1") && shotReady){
//			Shoot();
//		}
		//lock on
		if(Input.GetButtonDown("R3")){
			LockOn();
			camScript.lockedOn = true;
			camScript.target = target;
		}
		//unlock
		if(Input.GetButtonDown("L3")){
			if(target != null){
				camScript.lockedOn = false;
				camScript.target = null;
				targetMarker.transform.SetParent(null, true);
				targetMarker.SetActive(false);
				target = null;	
			}
		}
		switch(myState){
		case playerState.idle:
			if(init){
				init = false;
			}
			if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0){
				m_animator.SetBool("Run", true);
				init = true;
				myState = playerState.run;
				break;
			}
			if(Input.GetButtonDown("Jump")){
				m_animator.SetBool("Jump" ,true);
				init = true;
				myState = playerState.jump;
				break;
			}
			break;
		case playerState.run:
			if(init){
				init = false;
			}
			moveDir = (Vector3.forward * Input.GetAxisRaw("Vertical")) + (Vector3.right * Input.GetAxisRaw("Horizontal"));
			transform.Translate(moveDir * speed, Space.Self);
//			body.velocity += moveDir * speed;
			lookTransform.transform.forward = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal"));

			if(Input.GetButtonDown("Jump")){
				m_animator.SetBool("Jump" ,true);
				init = true;
				myState = playerState.jump;
			}
			if(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0){
				m_animator.SetBool("Run", false);
				init = true;
				myState = playerState.idle;
			}
			break;
		case playerState.jump:
			if(init){
//				print(init);
//				body.AddForce(Vector3.up * 10000f);
				init = false;

			}
			m_animator.SetBool("Jump" ,false);
			ascending = true;
//			StartCoroutine(Jump(Time.time, transform.position, new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z), 10f, 10f));
//			StartCoroutine(JumpTwo(transform.position.y + 5f));
			print ("joomper");
			StartCoroutine(JumpThree(transform.position.y, transform.position.y + 5f));
			moveDir = (Vector3.forward * Input.GetAxisRaw("Vertical")) + (Vector3.right * Input.GetAxisRaw("Horizontal"));
			transform.Translate(moveDir * speed, Space.Self);
			lookTransform.transform.forward = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal"));
			if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0){
				m_animator.SetBool("Run", true);
				init = true;
				myState = playerState.run;
			}
			if(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0){
				m_animator.SetBool("Run", false);
				init = true;
				myState = playerState.idle;
			}
			break;
		}
		if(Input.GetButton("Fire1") && shotReady){
			print(cam.forward);
			Shoot();
			print(cam.forward);
		}
		transform.forward = new Vector3(cam.forward.x, 0f, cam.forward.z);


	}
	

	IEnumerator Cooldown(float time){
		shotReady = false;
		print(shotReady.ToString());
		yield return new WaitForSeconds(time);
		m_animator.SetLayerWeight(1, 0f);
		m_animator.SetBool("Shoot", false);

		shotReady = true;
		print(shotReady.ToString());
	}

	IEnumerator Jump(float startTime, Vector3 startMarker, Vector3 endMarker, float speed, float journeyLength){
		while(ascending){
			print ("fooker");
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney);
			if(transform.position == endMarker){
				ascending = false;
				print("dooker");

			}
			yield return new WaitForFixedUpdate();
		}
		float newStartTime = Time.time;
		while(!ascending){
			float distCovered = (Time.time - newStartTime) * speed;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp(endMarker, startMarker, fracJourney);
			yield return new WaitForFixedUpdate();
		}
		yield break;

	}
	IEnumerator JumpTwo(float height){
		while(transform.position.y <= height){
			print ("fooker");
			transform.Translate(Vector3.up * 0.3f);
			yield return new WaitForFixedUpdate();
		}
		yield return new WaitForFixedUpdate();
		while(transform.position.y > 0f){
			transform.Translate(-Vector3.up * 0.2f);
			yield return new WaitForFixedUpdate();
		}
		yield break;
		
	}
	//using gravity and forces ya
	IEnumerator JumpThree(float initHeight, float endHeight){
		while(transform.position.y <= endHeight*0.75f){
			print ("fooker");
			body.AddForce(Vector3.up * 100f, ForceMode.Force);
			yield return new WaitForFixedUpdate();
		}
		while(transform.position.y >= initHeight){
			print ("fooker");
			body.AddForce(-Vector3.up * 200f, ForceMode.Force);
			yield return new WaitForFixedUpdate();
		}
		yield break;
		
	}

	//call on key down
	void Shoot(){
		print("Shoot - " + prefabProjectile.ToString());
		GameObject obj = StaticPool.GetObj(prefabProjectile);
		obj.GetComponent<ProjectileTwo>().Reset(gameObject, Camera.main.transform.forward);
		m_animator.SetBool("Shoot", true);
		if(myState == playerState.run){
			m_animator.SetLayerWeight(1, 1f);
		}
		StartCoroutine("Cooldown", cooldownTime);
	}

	//returns a list of all GameObjects with tag enemy that are visible to the camera
	List<GameObject> GetTargets(){
		GameObject[] temp = GameObject.FindGameObjectsWithTag("Enemy");
		List<GameObject> listToReturn = new List<GameObject>();
		foreach(GameObject obj in temp){
			if(obj.GetComponentInChildren<Renderer>().isVisible){
				listToReturn.Add(obj);
			}
		}
		return listToReturn;
	}

	//sort a list of gameobjects by their distance to the player
	//i read that using sqrMag is more performant that vector3.distance so i tried it
	List<GameObject> SortByDistance(List<GameObject> input){
		List<GameObject> temp = new List<GameObject>(input);
		List<GameObject> listToReturn;
		for(int i = 0; i < input.Count-1; i++){
			float mag1 = (temp[i].transform.position - transform.position).sqrMagnitude;
			float mag2 = (temp[i+1].transform.position - transform.position).sqrMagnitude;
			if(mag2 < mag1){
				GameObject obj = temp[i];
				temp[i] = temp[i+1];
				temp[i+1] = obj;
				i=0;
			}
		}
		listToReturn = new List<GameObject>(temp);
		return listToReturn;
	}

	//Choses which target to lock on to. 
	void LockOn(){
		//get a list of target visible targets, sort them by distance;
		List<GameObject> temp = new List<GameObject>(SortByDistance(GetTargets()));
		//if already locked on
		if(target != null){
			//loop through list to find our current target
			for(int i = 0; i < temp.Count; i++){
				if(temp[i] == target && i < temp.Count-1){
					print ("in check fired");
					target = temp[i+1];
					targetMarker.transform.SetParent(target.transform, false);
					targetMarker.transform.localPosition = new Vector3(0f,2f,0f);
					return;
				}
				else if(temp[i]==target){
					print ("else check fired");
					target = temp[0];
					targetMarker.transform.SetParent(target.transform, false);
					targetMarker.transform.localPosition = new Vector3(0f,2f,0f);
					return;
				}
			}
		}
		//else find closest target and lock on
		else{
			print ("no target");
			target = temp[0];
			targetMarker.SetActive(true);
			targetMarker.transform.SetParent(target.transform, false);
			targetMarker.transform.localPosition = new Vector3(0f,2f,0f);
		}
	}


}

