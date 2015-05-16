using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Cameras;
using UnityEngine.UI;

public class PlayerController2 : MonoBehaviour {

	public enum playerState{idle, run, dash, jump, attack, speacial};
	public playerState myState;

	public float speed;
	Transform cam;
	Vector3 moveDir;
	bool shotReady = true;
	float cooldownTime = 0.15f;
	public GameObject prefabProjectile;
	public GameObject lookTransform;
	Animator m_animator;
	bool init = true;

	float timeSinceInput = 0f;

	public GameObject targetMarker; //set in inspector
	GameObject target;
	GameObject[] enemies;
	List<GameObject> enemiesInSight;

	Stats m_Stats;

	public FreeLookCam camScript;

	Rigidbody m_body;

	bool ascending = false;
	bool dashAvailiable = true;
	bool dashing = false;
	float dashLength = 5f;
	Vector3 dashStartPos;
	Vector3 dashEndPos;
	Vector3 lastMoveDir = Vector3.zero;

	float attackTimer = 0f;
	float attackExitTime = 0.1f;
	public GameObject attackCollider;
	List<Stats> enemiesToMeleeAttack = new List<Stats>();
	public GameObject slashFXPrefab;
	public GameObject attackLightFX;

	public Slider dashCooldownSlider;

	StaticPool staticPool;
	void Awake(){
		staticPool = new StaticPool();
		cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
	}
	// Use this for initialization
	void Start () {

		targetMarker.SetActive(false);

		m_animator = gameObject.GetComponent<Animator>();
		m_body = gameObject.GetComponent<Rigidbody>();
		m_Stats = gameObject.GetComponent<Stats>();
	}

	void Update(){

//		print("Time: " + Time.time + "\n" + "Horz: " +  Input.GetAxis("Horizontal") + "\nVert: " + Input.GetAxis("Vertical"));
//		moveDir = (Vector3.forward * Input.GetAxisRaw("Vertical")) + (Vector3.right * Input.GetAxisRaw("Horizontal"));
//		moveDir = (new Vector3(transform.forward.x, 0f, transform.forward.z) * Input.GetAxisRaw("Vertical")) + (new Vector3(transform.right.x, 0f, transform.right.z) * Input.GetAxisRaw("Horizontal"));
//		if(moveDir.magnitude >= 1f && dashAvailiable){
////			StartCoroutine(Dash(Time.time, moveDir));
//			StartCoroutine(Dash2(Time.time, moveDir));
//		}

	}

	void FixedUpdate(){

		timeSinceInput += Time.deltaTime;
		//lock on
		if(Input.GetButtonDown("R3")){
			timeSinceInput = 0f;
			LockOn();
			camScript.lockedOn = true;
			camScript.target = target;
		}
		//unlock
		if(Input.GetButtonDown("L3")){
			timeSinceInput = 0f;
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
			if(Input.GetButton("Circle")){
				init = true;
				myState = playerState.attack;
			}
			break;
		case playerState.run:
			if(init){
				init = false;
			}
			lastMoveDir = moveDir;
			moveDir = (new Vector3(transform.forward.x, 0f, transform.forward.z) * Input.GetAxisRaw("Vertical")) + (new Vector3(transform.right.x, 0f, transform.right.z) * Input.GetAxisRaw("Horizontal"));
//			if(moveDir.magnitude >= 1f && dashAvailiable && lastMoveDir.magnitude < 0.99f){
//				//			StartCoroutine(Dash(Time.time, moveDir));
//				StartCoroutine(Dash2(Time.time, moveDir));
//			}
			transform.Translate(moveDir * speed, Space.World);

			lookTransform.transform.forward = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal"));

//			if(dashing && dashAvailiable){
//				init = true;
//				myState = playerState.dash;
//			}
			if(Input.GetButtonDown("X") && dashAvailiable){
				init = true;
				myState = playerState.dash;
			}
			if(Input.GetButton("Circle")){
				init = true;
				myState = playerState.attack;
			}
//			else if(dashing){
//				dashing = false;
//			}
			if(Input.GetButton("Jump")){
				m_animator.SetBool("Jump" ,true);
				init = true;
				myState = playerState.jump;
			}
			if(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0){
				m_animator.SetBool("Run", false);
				init = true;
				lookTransform.transform.forward = lastMoveDir;
				myState = playerState.idle;
			}
			break;

		case playerState.dash:
			if(init){
				m_animator.SetLayerWeight(2,1);
				dashStartPos = transform.position;
				dashAvailiable = false;

				init = false;
			}
//			m_body.AddForce(moveDir * 25f, ForceMode.VelocityChange);
			m_body.AddForce(new Vector3(moveDir.x * 25f, 0f, moveDir.z * 25f), ForceMode.VelocityChange);

			if(Vector3.Distance(transform.position, dashStartPos) > dashLength){
				m_body.velocity = Vector3.zero;
				init = true;
				dashing = false;
				StartCoroutine(DashCoolDown(0.25f));
				m_animator.SetLayerWeight(2,0);
				myState = playerState.run;
			}
			break;
		case playerState.jump:
			if(init){
				init = false;
			}
			m_animator.SetBool("Jump" ,false);
			ascending = true;
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
		case playerState.attack:
			if(init){
				m_animator.SetBool("Attack", true);
//				attackCollider.SetActive(true);
				init = false;
			}
			if(Input.GetButton("Circle")){
				m_animator.SetInteger("RandomAttack", Random.Range(0,3));
				attackTimer = 0f;
			}
			if(attackTimer > attackExitTime){
				m_animator.SetBool("Attack", false);
//				attackCollider.SetActive(false);
				myState = playerState.idle;
			}
			attackTimer += Time.deltaTime;
			break;
		}
		if(Input.GetButton("Fire1") && shotReady){
			Shoot();
		}
		transform.forward = new Vector3(cam.forward.x, 0f, cam.forward.z);


	}
	

	void Shoot(){
		m_animator.SetBool("Shoot", true);

		if(myState == playerState.run){
			m_animator.SetLayerWeight(1, 1f);
		}
		StartCoroutine("Cooldown", cooldownTime);
	}

	IEnumerator Cooldown(float time){
		shotReady = false;
		yield return new WaitForSeconds(time/2f);
		GameObject obj = StaticPool.GetObj(prefabProjectile);
		obj.GetComponent<ProjectileTwo>().Reset(gameObject, Camera.main.transform.forward);
		yield return new WaitForSeconds(time/2);
		m_animator.SetLayerWeight(1, 0f);
		m_animator.SetBool("Shoot", false);
		shotReady = true;
	}
	
	//using gravity and forces ya
	IEnumerator JumpThree(float initHeight, float endHeight){
		while(transform.position.y <= endHeight*0.75f){
			m_body.AddForce(Vector3.up * 20f, ForceMode.Force);
			yield return new WaitForFixedUpdate();
		}
		while(transform.position.y >= initHeight){
			m_body.AddForce(-Vector3.up * 10f, ForceMode.Force);
			yield return new WaitForFixedUpdate();
		}
		yield break;
		
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
					target = temp[i+1];
					targetMarker.transform.SetParent(target.transform, false);
					targetMarker.transform.localPosition = new Vector3(0f,2f,0f);
					return;
				}
				else if(temp[i]==target){
					target = temp[0];
					targetMarker.transform.SetParent(target.transform, false);
					targetMarker.transform.localPosition = new Vector3(0f,2f,0f);
					return;
				}
			}
		}
		//else find closest target and lock on
		else{
			target = temp[0];
			targetMarker.SetActive(true);
			targetMarker.transform.SetParent(target.transform, false);
			targetMarker.transform.localPosition = new Vector3(0f,2f,0f);
		}
	}

	//takes Time.time and moveDir
	//uses time to tell how long its been since the initial inpout
	//uses the getaxis to determine which direction im looking for
	//Dash recharge time is included in this function
	IEnumerator Dash(float time, Vector3 direction){
		float elapsedTime = Time.time - time;
		float endTime = 0.2f;
		float dashDuration = 0.25f;
		float dashTime = 0f;
		float dashCooldown = 3f;

		float correctInputThresh = 0.75f; //how similar the new input needs to be from direction to start dashing
		float exitInputThresh = -0.5f; //how far away the control stick needs to face before we kill the coroutine
		float resetInputMag = 0.9f; //magnitiude of last input most be atleast this small for the current oen to trigger a dash
		bool correctInput = false;
		List<Vector3> inputs = new List<Vector3>();
		inputs.Add(direction);
		inputs.Add(direction);
		//waiting on input
		while(elapsedTime < endTime && !correctInput){
			if(moveDir.magnitude > 1f){
				inputs.Add(moveDir.normalized);
			}
			else{
				inputs.Add(moveDir);
			}
//			print (inputs[inputs.Count -1].magnitude);
//			print (Vector3.Dot(inputs[inputs.Count - 1].normalized, direction.normalized) + "    " + inputs[inputs.Count -1] + "     " + inputs[inputs.Count -2]);
			if(Vector3.Dot(inputs[inputs.Count - 1].normalized, direction.normalized) < exitInputThresh){
				StopCoroutine("Dash");
			}
			if(Vector3.Dot (inputs[inputs.Count - 1].normalized, direction.normalized) > correctInputThresh && inputs[inputs.Count - 1].magnitude >= 1f && inputs[inputs.Count - 2].magnitude < resetInputMag && dashAvailiable){
				correctInput = true;
				dashing = true;
				dashAvailiable = false;
				dashTime = Time.time;
//				print ("DASH!      " + Time.time);

			}
			else if(!dashAvailiable){
				StopCoroutine("Dash");
			}

			yield return new WaitForEndOfFrame();
		}
		//dashing in a direction
		while(Time.time - dashTime < dashDuration){
			m_animator.SetLayerWeight(2,1);
			m_animator.SetBool("Dash", true);
			speed = 0.75f;
			moveDir = direction;
			yield return new WaitForEndOfFrame();
		}
		dashing = false;
		m_animator.SetBool("Dash", false);
		speed = 0.25f;
		m_animator.SetLayerWeight(2,0);
		dashTime = Time.time;

		while(Time.time - dashTime < dashCooldown){
			dashCooldownSlider.value = ((Time.time - dashTime)/dashCooldown);
			yield return new WaitForEndOfFrame();
		}
		dashAvailiable = true;

	}
	IEnumerator Dash2(float time, Vector3 direction){
		float elapsedTime = Time.time - time;
		float endTime = 0.1f; //the time im looking for inputs
		float correctInputThresh = 0.75f; //how similar the new input needs to be from direction to start dashing
		float exitInputThresh = -0.5f; //how far away the control stick needs to face before we kill the coroutine
		float resetInputMag = 0.8f; //magnitiude of last input most be atleast this small for the current oen to trigger a dash
		bool correctInput = false;
		List<Vector3> inputs = new List<Vector3>();
		inputs.Add(direction);
		inputs.Add(direction);

		while(elapsedTime < endTime && !correctInput){
			print(moveDir);
			if(moveDir.magnitude > 1f){
				inputs.Add(moveDir.normalized);
			}
			else{
				inputs.Add(moveDir);
			}

			if(Vector3.Dot (inputs[inputs.Count - 1].normalized, direction.normalized) > correctInputThresh && inputs[inputs.Count - 1].magnitude >= 1f && inputs[inputs.Count - 2].magnitude < resetInputMag){
				correctInput = true;
				dashing = true;
			}
			
			yield return new WaitForFixedUpdate();
		}
	
	}

	IEnumerator DashCoolDown(float time){
		float startTime = Time.time;
		while((Time.time - startTime) < time){
			dashCooldownSlider.value = (Time.time - startTime)/time;
			yield return new WaitForFixedUpdate();
		}
		dashCooldownSlider.value = 1f;
		dashAvailiable = true;
	}

	//called by attack collider
	public void AddToAttackList(Stats enemy){
		enemiesToMeleeAttack.Add(enemy);
	}
	public void RemoveFromAttackList(Stats enemy){
		enemiesToMeleeAttack.Remove(enemy);
	}
	public void MeleeAttack(){
		foreach(Stats enemy in enemiesToMeleeAttack){
			enemy.TakeDamager(3);
		}
		if(enemiesToMeleeAttack.Count > 0){
			GetComponent<Stats>().StartShake();
			GameObject temp = StaticPool.GetObj(slashFXPrefab);
			temp.transform.SetParent(lookTransform.transform, false);
			temp.transform.localPosition = (new Vector3(0f,1f,-1f));
			temp.transform.Rotate(0f,180f,0f);
			temp.GetComponentInChildren<ParticleSystem>().enableEmission = true;
			attackLightFX.SetActive(true);
			StartCoroutine(Lightflicker(0.4f));
		}


	}

	IEnumerator Lightflicker(float duration){
		yield return new WaitForSeconds(duration);
		attackLightFX.SetActive(false);
	}


}

