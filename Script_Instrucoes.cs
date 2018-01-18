using UnityEngine;
using System.Collections;

public class Script_Instrucoes : MonoBehaviour {

	public GameObject rawImage1;
	public GameObject rawImage2;
	public GameObject rawImage3;
	public GameObject rawImage4;
	public Texture2D arrow;
	public Texture2D arrow1;
	public GUIStyle guiStyle;
	public AudioClip sd_buttons;
	int window = 0;
	float cooldown = 0;
	GUIStyle GUI_ESC;

	void Start(){
		AudioSource.PlayClipAtPoint(sd_buttons,transform.position);
		GUI_ESC = new GUIStyle ();
		GUI_ESC.font = guiStyle.font;
		GUI_ESC.normal.textColor = Color.black;
		GUI_ESC.fontSize = 30;
	}

	void OnGUI(){
		cooldown += Time.deltaTime;
		GUI.Label (new Rect(825,690,0,0),"Pressione ESC para voltar.",GUI_ESC);
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.LoadLevel("Menu Principal");
		}
		switch (window) {
		case 0:
			rawImage1.SetActive(true);
			rawImage2.SetActive(false);
			rawImage3.SetActive(false);
			rawImage4.SetActive(false);
			if(GUI.Button(new Rect(1180,305,85,85),arrow,GUIStyle.none) || Input.GetAxisRaw("Horizontal") > 0){
				if(cooldown > 1){
					window++;
					cooldown = 0;
					AudioSource.PlayClipAtPoint(sd_buttons,transform.position);
				}
			}
			break;
		case 1:
			rawImage1.SetActive(false);
			rawImage2.SetActive(true);
			rawImage3.SetActive(false);
			rawImage4.SetActive(false);
			if(GUI.Button(new Rect(1180,305,85,85),arrow,GUIStyle.none) || Input.GetAxisRaw("Horizontal") > 0){
				if(cooldown > 1){
					window++;
					cooldown = 0;
					AudioSource.PlayClipAtPoint(sd_buttons,transform.position);
				}
			}
			if(GUI.Button(new Rect(0,305,85,85),arrow1,GUIStyle.none) || Input.GetAxisRaw("Horizontal") < 0){
				if(cooldown > 1){
					window--;
					cooldown = 0;
					AudioSource.PlayClipAtPoint(sd_buttons,transform.position);
				}
			}
			break;
		case 2:
			rawImage1.SetActive(false);
			rawImage2.SetActive(false);
			rawImage3.SetActive(true);
			rawImage4.SetActive(false);
			if(GUI.Button(new Rect(1180,305,85,85),arrow,GUIStyle.none) || Input.GetAxisRaw("Horizontal") > 0){
				if(cooldown > 1){
					window++;
					cooldown = 0;
					AudioSource.PlayClipAtPoint(sd_buttons,transform.position);
				}
			}
			if(GUI.Button(new Rect(0,305,85,85),arrow1,GUIStyle.none) || Input.GetAxisRaw("Horizontal") < 0){
				if(cooldown > 1){
					window--;
					cooldown = 0;
					AudioSource.PlayClipAtPoint(sd_buttons,transform.position);
				}
			}
			break;
		case 3:
			rawImage1.SetActive(false);
			rawImage2.SetActive(false);
			rawImage3.SetActive(false);
			rawImage4.SetActive(true);
			if(GUI.Button(new Rect(0,305,85,85),arrow1,GUIStyle.none) || Input.GetAxisRaw("Horizontal") < 0){
				if(cooldown > 1){
					window--;
					cooldown = 0;
					AudioSource.PlayClipAtPoint(sd_buttons,transform.position);
				}
			}
			break;
		}
	}
}
