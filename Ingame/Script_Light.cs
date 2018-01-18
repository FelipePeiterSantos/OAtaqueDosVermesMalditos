using UnityEngine;
using System.Collections;

public class Script_Light : MonoBehaviour {

	void Update(){
		if(!Script_HUD.pause){
			int rnd = Random.Range (0,100);
			float rndLight = Random.Range (1f,2.2f);
			
			if(rnd == 10){
				GetComponent<Light>().intensity = rndLight;
				StartCoroutine("Thunder");
			}
		}
	}
	IEnumerator Thunder(){
		float rndDelay = Random.Range (0.01f, 0.2f);
		yield return new WaitForSeconds(rndDelay);
		GetComponent<Light>().intensity = 0.2f;
	}
}
