using UnityEngine;
using System.Collections;

public class Script_Grenade : MonoBehaviour {
	
	public GameObject grenade_particleExplosion;
	public AudioClip sd_bomb;
	GameObject[] enemyBiped;
	GameObject[] enemyWorm;
	//GameObject[] scriptBidepes;
	float grenade_countdown = 4f;

	Vector3 grenade_target;

	void Grenade_target(Vector3 receiver_gameobject){
		enemyBiped = GameObject.FindGameObjectsWithTag ("biped");
		enemyWorm = GameObject.FindGameObjectsWithTag("worm");
		transform.LookAt (receiver_gameobject);
		grenade_target = receiver_gameobject;
		this.rigidbody.AddForce(Vector3.up * 500f);
		this.rigidbody.AddRelativeTorque (Vector3.right * 5f);
	}

	void Update () {
		if(!Script_HUD.pause){
			grenade_countdown -= Time.deltaTime;
			transform.position = Vector3.Lerp(transform.position,new Vector3(grenade_target.x,transform.position.y,grenade_target.z),Time.deltaTime * 1f);
			if(grenade_countdown <= 0){
				Destroy(this.gameObject);
				AudioSource.PlayClipAtPoint(sd_bomb,new Vector3(transform.position.x,11.33f,transform.position.z));
				GameObject aux = Instantiate(grenade_particleExplosion,transform.position, Quaternion.identity) as GameObject;
				aux.SetActive(true);
				if(Script_HUD.isCombatMode){
					foreach(GameObject item in enemyBiped){
						if(item.GetComponent<Script_Bipede>()){
							item.SendMessage("GrenadedInCombat",transform.position);
						}
					}
					foreach(GameObject item in enemyWorm){
						if(item.GetComponent<Script_Worm>()){
							item.SendMessage("GrenadedInCombat",transform.position);
						}
					}
				}
				else{
					foreach(GameObject item in enemyBiped){
						if(item.GetComponent<Script_Bipede>()){
							item.SendMessage("Status_Agro",transform.position);
						}
					}
				}
			}
		}
	}

	void OnCollisionEnter(Collision coll){
		transform.rigidbody.AddForce (coll.relativeVelocity*20f);
	}
}
