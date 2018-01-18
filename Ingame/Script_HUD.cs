using UnityEngine;
using System.Collections;

public class Script_HUD : MonoBehaviour {


	public Vector4 pos;

	public Texture2D blackScreen;
	public Texture2D bomb_Sprite;
	public Texture2D AP_Sprite;
	public Texture2D health_sprite;
	public Texture2D hud_2_Sprite;
	public Texture2D turnFinish0_Sprite;
	public Texture2D turnFinish1_Sprite;
	public Texture2D melee_Sprite;
	public Texture2D rifle_Sprite;
	public Texture2D run_Sprite;
	public Texture2D bg_Pause;
	public Texture2D black_Pause;
	public Texture2D bt_Continuar;
	public Texture2D bt_Retornar;
	public Texture2D bt_Sair;
	public Texture2D bg_GameOver;
	public Texture2D bg_LogoGameOver;
	public Texture2D health_Bar;
	public Texture2D AP_Bar;
	public Font _font;
	public GUIStyle btnActions_GuiSkin;
	public GUIStyle btnTurnFinish_GuiSkin;
	public GUIStyle btnPause_GuiSkin;
	public LayerMask clickMouse_layermask;
	public static bool isCombatMode;
	public static int AP;
	public static bool isPlayerTurn = true;
	static public int health_Player;
	public static bool pause = false;
	public AudioClip soundTrack;
	public AudioClip storm;
	public AudioClip sd_clickAction;
	public AudioClip sd_cheat;
	public AudioClip sd_damaged;
	public AudioClip sd_dead;
	GUIStyle instrucoesGUI;
	GameObject player;
	RaycastHit clickMouse;
	GameObject[] enemyBiped;
	GameObject[] enemyWorm;
	int APinHUD;
	bool ableToFinish;
	bool nextStage = false;
	float blackScreen_anim;
	float cheatInput = 0f;
	float gameOverCounter = 0;
	bool[] activeCheat;
	bool godMode = false;
	float healthRegen = 0;
	bool lastScene = false;
	int round = 0;
	bool oneTimeClick = true;

	void Start () {
		if(Application.loadedLevelName == "scenario4"){
			instrucoesGUI = new GUIStyle(GUIStyle.none);
			instrucoesGUI.fontSize = 30;
			lastScene = true;
			round = 0;
		}
		pause = false;
		if(!GameObject.Find("soundTrackInGame")){
			GameObject soundTrackObj = new GameObject ();
			soundTrackObj.name = "soundTrackInGame";
			AudioSource[] auxAudio = new AudioSource[2];
			auxAudio[0] = soundTrackObj.AddComponent<AudioSource>();
			auxAudio[1] = soundTrackObj.AddComponent<AudioSource>();
			auxAudio[0].clip = soundTrack;
			auxAudio[0].loop = enabled;
			auxAudio[0].Play ();
			auxAudio[0].volume = 0.25f;
			auxAudio[1].clip = storm;
			auxAudio[1].loop = enabled;
			auxAudio[1].Play ();
			auxAudio[1].volume = 0.25f;
			DontDestroyOnLoad (soundTrackObj);
		}
		Script_Player.grenadeEnabled = true;
		isPlayerTurn = true;
		activeCheat = new bool[]{false,false,false};
		enemyBiped = GameObject.FindGameObjectsWithTag ("biped");
		enemyWorm = GameObject.FindGameObjectsWithTag("worm");
		nextStage = false;
		blackScreen_anim = 0;
		player = GameObject.FindWithTag ("Player");
		isCombatMode = false;
		health_Player = 100;
		APinHUD = 0;
		AP = 10;
		GUIStyle.none.font = _font;
		GUIStyle.none.normal.textColor = Color.white;
	}
	void Update(){
		cheatInput += Time.deltaTime;
		if(Input.GetKeyDown(KeyCode.N)){
			cheatInput = 0;
			activeCheat[0] = true;
			activeCheat[1] = false;
			activeCheat[2] = false;
		}
		else if(Input.GetKeyDown(KeyCode.O) && activeCheat[0]){
			cheatInput = 0;
			activeCheat[0] = false;
			activeCheat[1] = true;
			activeCheat[2] = false;
		}
		else if(Input.GetKeyDown(KeyCode.O) && activeCheat[1]){
			cheatInput = 0;
			activeCheat[0] = false;
			activeCheat[1] = false;
			activeCheat[2] = true;
		}
		else if(Input.GetKeyDown(KeyCode.B) && activeCheat[2]){
			godMode = !godMode;
			AudioSource.PlayClipAtPoint(sd_cheat,new Vector3(transform.position.x,11.33f,transform.position.z));
			activeCheat[0] = false;
			activeCheat[1] = false;
			activeCheat[2] = false;
		}
		else if(cheatInput > 5){
			for (int i = 0; i < activeCheat.Length; i++) {
				activeCheat[i] = false;
			}
		}
		if(godMode){
			health_Player = 100;
			AP = 10;
			Script_Player.grenadeEnabled = true;
		}


		isCombatMode = false;
		foreach(GameObject item in enemyBiped){
			if(Vector3.Distance(player.transform.position,item.transform.position) < 12){
				if(item.gameObject.name != "dead"){
					isCombatMode = true;
				}
			}
		}
		foreach(GameObject item in enemyWorm){
			if(Vector3.Distance(player.transform.position,item.transform.position) < 18){
				if(item.gameObject.name != "dead"){
					isCombatMode = true;
				}
			}
		}
		if(Input.GetKeyDown(KeyCode.Escape)){
			pause = !pause;
			AudioSource.PlayClipAtPoint(sd_clickAction,new Vector3(transform.position.x,11.33f,transform.position.z));
			if(pause){
				Time.timeScale = 0;
			}
			else{
				Time.timeScale = 1;
			}
		}
		else if(isCombatMode){
			ableToFinish = true;
			bool[] auxArray = player.GetComponent<Script_Player>().CurrentStatus();
			foreach(bool item in auxArray){
				if(item){
					ableToFinish = false;
				}
			}
			if(ableToFinish){
				oneTimeClick = true;
			}
			if(!isPlayerTurn){
				bool auxBool = true;				
				foreach(GameObject item in enemyBiped){
					if(item.GetComponent<Script_Bipede>()){
						if(item.GetComponent<Script_Bipede>().AmIdone()){
							auxBool = false;
						}
					}
				}
				foreach(GameObject item in enemyWorm){
					if(item.GetComponent<Script_Worm>()){
						if(item.GetComponent<Script_Worm>().AmIdone()){
							auxBool = false;
						}
					}
				}

				if(auxBool){
					AudioSource.PlayClipAtPoint(sd_clickAction,new Vector3(transform.position.x,11.33f,transform.position.z));
					AP = 10;
					isPlayerTurn = true;
					ableToFinish = true;
				}
			}
		}
		else {
			AP = 10;
			healthRegen += Time.deltaTime;
			if(healthRegen > 1f){
				healthRegen = 0;
				health_Player++;
				if(health_Player >= 100){
					health_Player = 100;
				}
			}
		}
		if(nextStage && blackScreen_anim > 0){
			blackScreen_anim -= 400f*Time.deltaTime;

		}
		else if(!nextStage && blackScreen_anim < 720){
			blackScreen_anim += 400f*Time.deltaTime;
		}
	}

	void OnGUI(){
		if(health_Player <= 0){
			GUI.DrawTexture (new Rect(0,0,Screen.width,Screen.height),bg_GameOver);
			GUI.DrawTexture (new Rect(330,200,bg_LogoGameOver.width,bg_LogoGameOver.height),bg_LogoGameOver);
			gameOverCounter += Time.deltaTime;
			pause = true;
			if(gameOverCounter > 10){
				Application.LoadLevel (0);
			}
		}
		else if(pause && health_Player > 0){
			GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),black_Pause);
			GUI.DrawTexture(new Rect(305,250,bg_Pause.width,bg_Pause.height),bg_Pause);
			if(GUI.Button(new Rect(405,285,451,44),"",btnPause_GuiSkin)){
				pause = false;
				Time.timeScale = 1;
				AudioSource.PlayClipAtPoint(sd_clickAction,new Vector3(transform.position.x,11.33f,transform.position.z));
			}
			if(GUI.Button(new Rect(405,340,451,44),"",btnPause_GuiSkin)){
				Application.LoadLevel(0);
			}
			if(GUI.Button(new Rect(405,395,451,44),"",btnPause_GuiSkin)){
				Application.Quit();
			}
			GUI.DrawTexture(new Rect(530,295,bt_Continuar.width,bt_Continuar.height),bt_Continuar);
			GUI.DrawTexture(new Rect(475,350,bt_Retornar.width,bt_Retornar.height),bt_Retornar);
			GUI.DrawTexture(new Rect(585,405,bt_Sair.width,bt_Sair.height),bt_Sair);
		}
		else if(isCombatMode){
			if(lastScene){
				GUI.Label(new Rect(260,0,0,0),"Sobreviva 5 de "+round.ToString()+" rounds para escapar do Ninho.",instrucoesGUI);
			}
			GUI.Label (new Rect(175,655,40,20),"VIDA:",GUIStyle.none);
			GUI.Label (new Rect(410,655,400,200),"AP:",GUIStyle.none);
			if(ableToFinish && isPlayerTurn){
				if(Script_Player.grenadeEnabled){
					if(GUI.Button (new Rect(770,615,130,105),"",btnActions_GuiSkin) || Input.GetKeyDown(KeyCode.Alpha1)){
						if(oneTimeClick){
							AudioSource.PlayClipAtPoint(sd_clickAction,new Vector3(transform.position.x,11.33f,transform.position.z));
							oneTimeClick = false;
						}
						player.SendMessage("StatusAction","grenade");
					}
					GUI.DrawTexture (new Rect(800,645,60,55),bomb_Sprite);
					GUI.Label (new Rect(785,685,25,22),"7pts",GUIStyle.none);
					GUI.Label (new Rect(780,605,0,0),"Granada",GUIStyle.none);
				}
				if(GUI.Button (new Rect(1020,615,130,105),"",btnActions_GuiSkin) || Input.GetKeyDown(KeyCode.Alpha3)){
					if(oneTimeClick){
						AudioSource.PlayClipAtPoint(sd_clickAction,new Vector3(transform.position.x,11.33f,transform.position.z));
						oneTimeClick = false;
					}
					player.SendMessage("StatusAction","rifle");
				}
				else if(GUI.Button (new Rect(1145,615,130,105),"",btnActions_GuiSkin) || Input.GetKeyDown(KeyCode.Alpha4)){
					if(oneTimeClick){
						AudioSource.PlayClipAtPoint(sd_clickAction,new Vector3(transform.position.x,11.33f,transform.position.z));
						oneTimeClick = false;
					}
					player.SendMessage("StatusAction","melee");
				}
				else if(GUI.Button (new Rect(895,615,130,105),"",btnActions_GuiSkin) || Input.GetKeyDown(KeyCode.Alpha2)){
					if(oneTimeClick){
						AudioSource.PlayClipAtPoint(sd_clickAction,new Vector3(transform.position.x,11.33f,transform.position.z));
						oneTimeClick = false;
					}
					player.SendMessage("StatusAction","run");
				}
				else if (GUI.Button (new Rect(1080,0,185,90),"",btnTurnFinish_GuiSkin) || Input.GetKeyDown(KeyCode.F)){
					AudioSource.PlayClipAtPoint(sd_clickAction,new Vector3(transform.position.x,11.33f,transform.position.z));
					isPlayerTurn = false;
					enemyBiped = GameObject.FindGameObjectsWithTag("biped");
					enemyWorm = GameObject.FindGameObjectsWithTag("worm");
					foreach (GameObject enemy in enemyBiped){
						if(enemy.GetComponent<Script_Bipede>()){
							enemy.GetComponent<Script_Bipede>().SendMessage("EnemyTurn");
						}
					}
					foreach (GameObject enemy in enemyWorm){
						if(enemy.GetComponent<Script_Worm>()){
							enemy.GetComponent<Script_Worm>().SendMessage("EnemyTurn");
						}
					}
					if(lastScene){
						if(round < 5){
							round++;
						}
						else{
							Application.LoadLevel("cutsceneFinal");
						}
					}
				}

				GUI.DrawTexture (new Rect(1035,650,100,45),rifle_Sprite);
				GUI.DrawTexture (new Rect(1155,635,100,75),melee_Sprite);
				GUI.DrawTexture (new Rect(925,630,72,74),run_Sprite);
				GUI.Label (new Rect(910	,685,18,22),"1pt",GUIStyle.none);
				GUI.Label (new Rect(1035,685,25,22),"4pts",GUIStyle.none);
				GUI.Label (new Rect(1160,685,25,22),"6pts",GUIStyle.none);
				GUI.Label (new Rect(905,605,25,22),"Correr",GUIStyle.none);
				GUI.Label (new Rect(1030,605,25,22),"Atirar",GUIStyle.none);
				GUI.Label (new Rect(1155,605,25,22),"Arma branca",GUIStyle.none);
			}

			GUI.DrawTexture(new Rect(165,670,health_Bar.width,health_Bar.height),health_Bar);
			GUI.DrawTexture(new Rect(400,670,AP_Bar.width,AP_Bar.height),AP_Bar);
			for (int i = 0; i < AP; i++) {
				GUI.DrawTexture (new Rect(414+APinHUD,679,AP_Sprite.width,AP_Sprite.height),AP_Sprite);
				APinHUD += 8;
			}
			APinHUD = 0;
			for (int i = 0; i < health_Player/4; i++) {
				GUI.DrawTexture (new Rect(180+APinHUD,679,health_sprite.width,health_sprite.height),health_sprite);
				APinHUD += 8;
			}
			APinHUD = 0;
		}
		else{
			GUI.Label (new Rect(175,655,40,20),"VIDA:",GUIStyle.none);
			GUI.DrawTexture(new Rect(165,670,health_Bar.width,health_Bar.height),health_Bar);
			for (int i = 0; i < health_Player/4; i++) {
				GUI.DrawTexture (new Rect(180+APinHUD,679,health_sprite.width,health_sprite.height),health_sprite);
				APinHUD += 8;
			}
			APinHUD = 0;

			if(Script_Player.grenadeEnabledFreeMode){
				GUI.Label(new Rect(175,615,0,0),"Granada disponivel!",GUIStyle.none);
			}
			else{
				GUI.Label(new Rect(175,615,0,0),"Regenerando Granada: "+((int)Script_Player.grenadeFreeModeCooldown).ToString()+"%",GUIStyle.none);
			}
		}
		GUI.DrawTexture (new Rect(0,520,hud_2_Sprite.width,hud_2_Sprite.height),hud_2_Sprite);
		GUI.DrawTexture (new Rect (0, 0+blackScreen_anim, Screen.width, Screen.height+20), blackScreen);
	}

	public void Damaged(int dano){
		player.SendMessage("Animations","damage");
		health_Player -= dano;
		AudioSource.PlayClipAtPoint(sd_damaged,new Vector3(transform.position.x,11.33f,transform.position.z));
		if(health_Player <= 0){
			AudioSource.PlayClipAtPoint(sd_dead,new Vector3(transform.position.x,11.33f,transform.position.z));
			health_Player = 0;
			player.SendMessage("Animations","death");
		}
	}

	public void GoToNextStage(){
		nextStage = true;
	}
}
