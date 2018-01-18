using UnityEngine;
using System.Collections;

public class Script_Camera : MonoBehaviour {

	GameObject player;
	Transform currentCamera;
	float sizeTerrain;
	
	void Start () {
		sizeTerrain = GameObject.Find ("cameraLimitMap").transform.position.x;
		player = GameObject.FindWithTag ("Player");
		if(!player){
			player = new GameObject();
			player.transform.position = new Vector3(9.06f,1.12f,8.4f);
		}
		else{
			transform.position = new Vector3(player.transform.position.x,transform.position.y,player.transform.position.z);
		}
		currentCamera = transform.FindChild ("Main Camera");
	}

	void Update () {
		if(Input.GetKey(KeyCode.Space)){
			transform.position = new Vector3(player.transform.position.x,transform.position.y,player.transform.position.z);
		}
		if(Input.GetAxisRaw("Horizontal") != 0){
			transform.position = Vector3.Lerp (transform.position,new Vector3(transform.position.x+2*Input.GetAxis("Horizontal"),transform.position.y,transform.position.z+2*Input.GetAxis("Horizontal")),0.1f);
		}
		if(Input.GetAxisRaw("Vertical") != 0){
			transform.position = Vector3.Lerp (transform.position,new Vector3(transform.position.x-2*Input.GetAxis("Vertical"),transform.position.y,transform.position.z+2*Input.GetAxis("Vertical")),0.1f);
		}
		if(transform.position.x < 10){
			transform.position = new Vector3(10,transform.position.y,transform.position.z);
		}
		else if(transform.position.x > sizeTerrain-10){
			transform.position = new Vector3(sizeTerrain-10,transform.position.y,transform.position.z);
		}
		if(transform.position.z < 10){
			transform.position = new Vector3(transform.position.x,transform.position.y,10);
		}
		else if(transform.position.z > sizeTerrain-10){
			transform.position = new Vector3(transform.position.x,transform.position.y,sizeTerrain-10);
		}
	}
}
