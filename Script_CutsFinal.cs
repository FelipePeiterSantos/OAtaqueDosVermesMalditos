using UnityEngine;
using System.Collections;

public class Script_CutsFinal : MonoBehaviour {

	public Texture2D gameOver_sprite;
	float cooldown = 0;
	bool gameOver = false;

	void Start(){
		Screen.lockCursor = true;
		Screen.showCursor = false;
		if(GameObject.Find("soundTrackInGame")){
			Destroy (GameObject.Find("soundTrackInGame"));
		}
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.LoadLevel("Menu Principal");
		}
		if(gameOver){
			cooldown += Time.deltaTime;
			if(cooldown > 10){
				Application.LoadLevel("Menu Principal");
			}
		}
	}

	void OnGUI(){
		if(gameOver){
			GUI.DrawTexture (new Rect(358,212,gameOver_sprite.width,gameOver_sprite.height),gameOver_sprite);
		}
	}


	public void Finished(){
		gameOver = true;
	}
}
