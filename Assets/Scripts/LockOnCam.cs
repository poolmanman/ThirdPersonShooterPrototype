using UnityEngine;
using System.Collections;

public class LockOnCam : MonoBehaviour {

	Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(target);
	}

	public void SwitchTarger(Transform newTarget){
		target = newTarget;
	}
}
