using UnityEngine;
using System.Collections;

public class Script_Objetivos : MonoBehaviour {
	
	public string nextStage; 
	public GameObject scanParticle;
	public Rect pos;
	public AudioClip sdEFX;

	public static int objectives = 0;
	public static int objetivesCompleted = 0;
	float progressScan = 0f;
	bool isScanning = false;
	static bool stageClear = false;
	bool playSound = true;
	bool guiOFF = false;
	Script_HUD hud;

	void Start(){
		guiOFF = false;
		if(this.gameObject.name == "ninho"){
			scanParticle.SetActive(false);
			isScanning = false;
			progressScan = 0;
			objectives++;
			objetivesCompleted = 0;
		}
		if(this.gameObject.name == "ExitFase"){
			stageClear = false;
			hud = GameObject.Find("HUD").GetComponent<Script_HUD>();
		}
	}
	
	void OnTriggerEnter(Collider coll){
		if(this.gameObject.name == "ExitFase"){
			if(coll.gameObject.tag == "Player"){
				if(objetivesCompleted == objectives){
					hud.SendMessage("GoToNextStage");
					StartCoroutine("NextStage",4);
					guiOFF = true;

				}
			}
		}
	}

	void OnTriggerStay(Collider coll){
		if(!Script_HUD.pause){
			if(this.gameObject.name == "ninho"){
				if(coll.gameObject.tag == "Player" && !Script_HUD.isCombatMode && objetivesCompleted != objectives){
					Debug.Log("scan");
					isScanning = true;
					progressScan += Time.deltaTime * 10;
					scanParticle.SetActive(true);
					if(playSound){
						playSound = false;
						AudioSource.PlayClipAtPoint(sdEFX,new Vector3(transform.position.x,11.33f,transform.position.z));
					}
					if(progressScan >= 100){
						isScanning = false;
						scanParticle.SetActive(false);
					}
				}
			}
		}
	}
	void OnTriggerExit(Collider coll){
		if(this.gameObject.name == "ninho"){
			if(coll.gameObject.tag == "Player"){
				if(isScanning){
					progressScan = 0;
					isScanning = false;
					scanParticle.SetActive(false);
				}
				else if(progressScan >= 100){
					isScanning = false;
					scanParticle.SetActive(false);
					objetivesCompleted++;
					if(this.gameObject.GetComponent<SphereCollider>()){
						this.gameObject.GetComponent<SphereCollider>().enabled = false;
					}
				}
			}
		}
	}

	void OnGUI(){
		if(!Script_HUD.pause && !guiOFF){
			Vector3 relativePosition = Camera.main.transform.InverseTransformPoint (transform.position);
			relativePosition.z = Mathf.Max (relativePosition.z, 1);

			Vector3 hudObjetive = Camera.main.WorldToScreenPoint( Camera.main.transform.TransformPoint(relativePosition)//transform.position
			                                                     );
			hudObjetive.y = Screen.height - hudObjetive.y;


	 		if(hudObjetive.x < 125){
				hudObjetive.x = 125;
			}
			else if(hudObjetive.x > Screen.width - 125){
				hudObjetive.x = Screen.width - 125;
			}

			if(hudObjetive.y < 90){
				hudObjetive.y = 90;
			}
			else if(hudObjetive.y > Screen.height - 30){
				hudObjetive.y = Screen.height - 30;
			}

			if(this.gameObject.name == "ninho"){
				if(this.gameObject.GetComponent<SphereCollider>().enabled){
					GUI.Box (new Rect(hudObjetive.x-45,hudObjetive.y-50,90,20),"ESCANEAR",GUIStyle.none);
				}
				if(isScanning){
					GUI.Label(new Rect(610,455,125,25),"Escaneando: "+((int)progressScan).ToString()+"%",GUIStyle.none);
				}
			}

			else if(this.gameObject.name == "ExitFase"){
				GUI.Box (new Rect(hudObjetive.x-45,hudObjetive.y-25,90,20),"SAIDA",GUIStyle.none);
			}
		}
	}

	IEnumerator NextStage(int delay){
		AudioSource.PlayClipAtPoint(sdEFX,new Vector3(transform.position.x,11.33f,transform.position.z));
		yield return new WaitForSeconds (delay);
		objectives = 0;
		objetivesCompleted = 0;
		Application.LoadLevel(nextStage);
	}
}
