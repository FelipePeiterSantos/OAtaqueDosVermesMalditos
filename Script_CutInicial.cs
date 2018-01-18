using UnityEngine;
using System.Collections;

public class Script_CutInicial : MonoBehaviour {
	
	public Texture2D jornal;
	public GUIStyle guiStyle;
	public AudioClip sd_jornal;
	public AudioClip narracao;
	float _x;
	float _y;
	float _wh;
	float cooldownSound = 0;
	Vector2 mouseDown;

	void Start(){
		AudioSource.PlayClipAtPoint(sd_jornal,transform.position);
		AudioSource.PlayClipAtPoint(narracao,transform.position);
		StartCoroutine ("PlayGame",70f);
		_x = 280;
		_y = 0;
		_wh = 0.7f;
	}

	void Update(){
		cooldownSound += Time.deltaTime;
		if(Input.mouseScrollDelta.y != 0){
			_wh += 0.02f * Input.mouseScrollDelta.y;
			if(cooldownSound > 5){
				cooldownSound = 0;
				AudioSource.PlayClipAtPoint(sd_jornal,transform.position);
			}
		}
		if(_wh < 0.7f){
			_wh = 0.7f;
		}
		else if(_wh > 1.5f){
			_wh = 1.5f;
		}
		if(Input.GetMouseButtonDown(0)){
			mouseDown.x = Input.mousePosition.x;
			mouseDown.y = Screen.width - Input.mousePosition.y;
			if(cooldownSound > 5){
				cooldownSound = 0;
				AudioSource.PlayClipAtPoint(sd_jornal,transform.position);
			}
		}
		else if(Input.GetMouseButton(0)){
			_x = _x + (Input.mousePosition.x - mouseDown.x);
			_y = _y + (Screen.width - Input.mousePosition.y) - mouseDown.y;
			mouseDown.x = Input.mousePosition.x;
			mouseDown.y = Screen.width - Input.mousePosition.y;
		}
		
		if(_x < 280+(-870*(_wh-0.7f)/0.8f)){
			_x = 280+(-870*(_wh-0.7f)/0.8f);
		}
		else if (_x > 280){
			_x = 280;
		}
		if(_y < -817*(_wh-0.7f)/0.8f){
			_y = -817*(_wh-0.7f)/0.8f;
		}
		else if(_y > 0){
			_y = 0;
		}
	}

	void OnGUI(){
		GUI.DrawTexture (new Rect(_x,_y,jornal.width * _wh, jornal.height * _wh),jornal);
		GUI.Label (new Rect(930,690,0,0),"Pressione ESC para jogar.",guiStyle);
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.LoadLevel("scenario1");
		}
	}
	IEnumerator PlayGame(float delay){
		yield return new WaitForSeconds (delay);
		Application.LoadLevel("scenario1");
	}
}
