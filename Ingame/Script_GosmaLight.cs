using UnityEngine;
using System.Collections;

public class Script_GosmaLight : MonoBehaviour {

	public float minLight = 2;
	public float maxLight = 8;
	float aux;
	bool invert = true;
	Transform[] lights;

	void Start(){

		aux = minLight;
 		int lightsChild = transform.childCount;
		lights = new Transform[lightsChild];
		for (int i = 0; i < lights.Length; i++) {
			lights[i] = transform.GetChild(i);
		}
	}

	void Update () {

		if(invert){
			aux +=  10f * Time.deltaTime;
			if(aux > maxLight){
				invert = false;
			}
		}
		else{
			aux -=  10f * Time.deltaTime;
			if(aux < minLight){
				invert = true;
			}
		}
		for (int i = 0; i < lights.Length; i++) {
			if(lights[i].GetComponent<Light>()){
				lights[i].GetComponent<Light>().intensity = aux;
			}
		}
	}
}
