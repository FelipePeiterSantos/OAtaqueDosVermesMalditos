using UnityEngine;
using System.Collections;

public class MenusButtons : MonoBehaviour {

	public Animator playerAnimator;
	public GameObject[] canvas;
	public AudioClip soundMenu;
	public AudioClip soundBtnJogar;
	public AudioClip sd_buttons;
	public GUIStyle guiStyle;
	public string[] textButton;
	public Texture2D cursorSprite;

	public Vector4 pos;
	bool inMenuScreen = true;
	void Awake(){
		Screen.fullScreen = false;
		if(Application.loadedLevelName == "Menu Principal"){
			Cursor.SetCursor (cursorSprite,Vector2.zero,CursorMode.ForceSoftware);
		}
	}
	
	void Start(){
		AudioSource.PlayClipAtPoint(sd_buttons,transform.position);
		Screen.lockCursor = false;
		Screen.showCursor = true;
		Time.timeScale = 1;
		Script_Objetivos.objectives = 0;
		Script_Objetivos.objetivesCompleted = 0;
		if (GameObject.Find ("soundTrackInGame")) {
			Destroy(GameObject.Find ("soundTrackInGame"));
		}
		if(!GameObject.Find("soundTrack")){
			GameObject soundTrack = new GameObject ();
			soundTrack.name = "soundTrack";
			soundTrack.transform.position = Camera.main.transform.position;
			soundTrack.AddComponent<AudioSource> ().clip = soundMenu;
			soundTrack.GetComponent<AudioSource> ().loop = true;
			soundTrack.GetComponent<AudioSource>().Play ();
			DontDestroyOnLoad(soundTrack);
		}	
	}

	void OnGUI(){
		if(inMenuScreen && Application.loadedLevelName == "Menu Principal"){
			if(GUI.Button(new Rect(845,215,328,112),"JOGAR",guiStyle)){
				OnPlayBtn();
				AudioSource.PlayClipAtPoint(soundBtnJogar,transform.position);
			}
			if(GUI.Button(new Rect(845,335,328,112),"INSTRUCOES",guiStyle)){
				Application.LoadLevel("Instrucoes");
			}
			if(GUI.Button(new Rect(845,455,328,112),"CREDITOS",guiStyle)){
				OnCreditsBtn();
			}
			if(GUI.Button(new Rect(845,580,328,112),"SAIR",guiStyle)){
				OnExitBtn();
			}
		}
		else if(Application.loadedLevelName == "Credits"){
			GUI.Label (new Rect(940,690,0,0),"Pressione ESC para voltar.",guiStyle);
			if(Input.GetKeyDown(KeyCode.Escape)){
				Application.LoadLevel("Menu Principal");
			}
		}
	}

	public void OnPlayBtn(){
		StartCoroutine ("PlayGame");
		inMenuScreen = false;
		playerAnimator.SetBool ("PLAY",true);
		foreach(GameObject item in canvas){
			item.SetActive(false);
		}
	}

	public void OnCreditsBtn(){
		Application.LoadLevel ("Credits");
	}

	public void OnReturnBtn(){
		Application.LoadLevel ("Menu Principal");
	}

	public void OnExitBtn(){
		Application.Quit();
	}

	IEnumerator PlayGame(){
		yield return new WaitForSeconds (1.5f);
		Application.LoadLevel("cutsceneInicial");
		Destroy (GameObject.Find ("soundTrack"));
	}
}
