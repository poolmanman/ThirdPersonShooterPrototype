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
		if(gameObject.tag == "Player"){
			StartShake();
		}
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
	public IEnumerator Shake(float duration, float magnitude){
		float elapsed = 0.0f;
		
		//		Vector3 originalCamPos = Camera.main.transform.parent.position;
		
		while (elapsed < duration) {
			elapsed += Time.deltaTime;          
			
			float percentComplete = elapsed / duration;         
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);
			
			// map value to [-1, 1]
			float x = Random.value * 2.0f - 1.0f;
			float y = Random.value * 2.0f - 1.0f;
			x *= magnitude * damper;
			y *= magnitude * damper;
			
			Camera.main.transform.parent.position = new Vector3(Camera.main.transform.parent.position.x + x, Camera.main.transform.parent.position.y + y, Camera.main.transform.parent.position.z);
			//			originalCamPos = new Vector3(Camera.main.transform.parent.position.x - x, Camera.main.transform.parent.position.y - y, Camera.main.transform.parent.position.z);
			yield return null;
		}
		
		Camera.main.transform.parent.localPosition = new Vector3(0.5f,1.2f,1.2f);
	}
	
	public void StartShake(){
		StartCoroutine(Shake(0.25f,0.2f));
		print("SHAKE");
	}
}
