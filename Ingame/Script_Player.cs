using UnityEngine;
using System.Collections;

public class Script_Player : MonoBehaviour {
	
	public LayerMask clickMouse_layermask;
	public GameObject destination_prefab;
	public GameObject grenade;
	public Transform grenade_spawnAtHand;
	public Material lineRenderer;
	public GameObject player_lineRenderer;
	public bool isCombatMode = false;
	public ParticleSystem flameShoot;
	public AudioClip sd_shoot;
	public AudioClip sd_knife;
	public AudioClip sd_grassWalk;
	public AudioClip sd_throwGrenade;
	
	static GameObject destination_instantiate;
	public static bool grenadeEnabled = true;
	public static bool grenadeEnabledFreeMode = true;
	public static float grenadeFreeModeCooldown = 100;

	NavMeshAgent player_navMeshAgent;
	Animator player_animation;
	GameObject[] teste = new GameObject[]{};
	RaycastHit clickMouse;
	LineRenderer player_lineRendererActives;
	GameObject player_gameObject;

	int consumeAP = 0;
	bool followApath = false;
	bool isRun = true;
	bool aux_isPlaying = false;
	bool aux_grenade = false;
	bool aux_rifle = false;
	bool aux_run = false;
	bool aux_melee = false;
	float cd_walkSd = 0;


	void Start () {
		grenadeEnabledFreeMode = true;
		player_gameObject = GameObject.Find ("FBX Player");
		player_navMeshAgent = GetComponent<NavMeshAgent> ();
		player_animation = GetComponentInChildren<Animator> ();
		player_lineRendererActives = GetComponent<LineRenderer> ();
		player_lineRendererActives.SetVertexCount (2);
	}

	void Update () {
		if(!Script_HUD.pause){
			if(player_navMeshAgent.hasPath){
				cd_walkSd += Time.deltaTime;
				if(cd_walkSd > 0.4f){
					cd_walkSd = 0;
					AudioSource.PlayClipAtPoint(sd_grassWalk,new Vector3(transform.position.x,11.33f,transform.position.z));
				}
				player_animation.SetInteger("status",2);
				player_animation.speed = 1.8f;
				player_navMeshAgent.speed = 6f;
				player_gameObject.transform.position = Vector3.Lerp(player_gameObject.transform.position,transform.position,10*Time.deltaTime);
				player_gameObject.transform.localRotation = Quaternion.Lerp(player_gameObject.transform.localRotation,Quaternion.Euler(0,8.75f,0),10*Time.deltaTime);
			}
			else if(!player_navMeshAgent.hasPath){
				player_animation.speed = 1f;
				player_animation.SetInteger("status",0);
				Destroy(destination_instantiate);

			}

			if (Script_HUD.isCombatMode){
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if(Physics.Raycast(ray.origin,ray.direction,out clickMouse,100,clickMouse_layermask)){
					if(!aux_isPlaying){
						if(Input.GetMouseButtonDown(1)){
							StatusAction("reset");
						}
						if (aux_grenade && grenadeEnabled){
							player_lineRendererActives.SetPosition(0,new Vector3(transform.position.x,clickMouse.point.y,transform.position.z));
							player_lineRendererActives.SetPosition(1,clickMouse.point);
							player_lineRendererActives.enabled = true;
							player_lineRenderer.SetActive(false);
							if(7 <= Script_HUD.AP && Vector3.Distance(clickMouse.point,transform.position) < 10){
								lineRenderer.color = Color.blue;
								if(Input.GetMouseButtonDown(0)){
									StatusAction("reset");
									grenadeEnabled = false;
									Script_HUD.AP -= 7;
									transform.LookAt(new Vector3(clickMouse.point.x,transform.position.y,clickMouse.point.z));
									player_animation.SetTrigger("grenade");
									StartCoroutine(ThrowGrenade(clickMouse.point,1f));
								}
							}
							else {
								lineRenderer.color = Color.red;
							}
						}
						else if(aux_melee){
							player_lineRendererActives.enabled = true;
							player_lineRenderer.SetActive(false);
							player_lineRendererActives.SetPosition(0,new Vector3(transform.position.x,clickMouse.point.y,transform.position.z));
							player_lineRendererActives.SetPosition(1,clickMouse.point);
							if(6 <= Script_HUD.AP && Vector3.Distance(clickMouse.point,transform.position) < 4){
								lineRenderer.color = Color.yellow;
								if(Input.GetMouseButtonDown(0)){
									StatusAction("reset");
									RaycastHit click_shoot;
									Ray ray_shoot = Camera.main.ScreenPointToRay (Input.mousePosition);
									if(Physics.Raycast(ray_shoot.origin,ray_shoot.direction,out click_shoot,100,1 << LayerMask.NameToLayer("inimigo"))){
										Script_HUD.AP -= 6;
										StartCoroutine(MeleeDamage(1,click_shoot));
										player_animation.SetTrigger("melee");
										transform.LookAt(new Vector3(clickMouse.point.x,transform.position.y,clickMouse.point.z));
									}
								}
							}
							else {
								lineRenderer.color = Color.red;
							}
						}
						else if(aux_rifle){
							player_lineRendererActives.enabled = true;
							player_lineRenderer.SetActive(false);
							player_lineRendererActives.SetPosition(0,new Vector3(transform.position.x,clickMouse.point.y,transform.position.z));
							player_lineRendererActives.SetPosition(1,clickMouse.point);
							if(4 <= Script_HUD.AP && Vector3.Distance(clickMouse.point,transform.position) < 8){
								lineRenderer.color = Color.yellow;
								if(Input.GetMouseButtonDown(0)){
									StatusAction("reset");
									RaycastHit click_shoot;
									Ray ray_shoot = Camera.main.ScreenPointToRay (Input.mousePosition);
									if(Physics.Raycast(ray_shoot.origin,ray_shoot.direction,out click_shoot,100,1 << LayerMask.NameToLayer("inimigo"))){
										Script_HUD.AP -= 4;
										StartCoroutine(RifleDamage(1,click_shoot));
										AudioSource.PlayClipAtPoint(sd_shoot,new Vector3(transform.position.x,11.33f,transform.position.z));
										player_animation.SetTrigger("rifle");

										transform.LookAt(new Vector3(clickMouse.point.x,transform.position.y,clickMouse.point.z));
									}
								}
							}
							else {
								lineRenderer.color = Color.red;
							}
						}

						else if(aux_run && player_lineRenderer.GetComponent<Script_PathLineRenderer>().lineRenderer_length < Script_HUD.AP*1.25f){
							consumeAP = (int)(player_lineRenderer.GetComponent<Script_PathLineRenderer>().lineRenderer_length/1.25f) + 1;	
							player_lineRendererActives.enabled = false;
							player_lineRenderer.SetActive(true);
							lineRenderer.color = Color.green;
							player_lineRenderer.GetComponent<LineRenderer>().material.color = Color.green;
							if(Input.GetMouseButtonDown(0)){
								StatusAction("reset");
								Script_HUD.AP -= (int)((player_lineRenderer.GetComponent<Script_PathLineRenderer>().lineRenderer_length + 1)/1.25f
								                       );
								player_navMeshAgent.destination = clickMouse.point;
								aux_isPlaying = true;
								player_lineRenderer.SetActive(false);
							}
						}
						else if (aux_run){
							consumeAP = (int)(player_lineRenderer.GetComponent<Script_PathLineRenderer>().lineRenderer_length/1.25f) + 1;
							player_lineRendererActives.enabled = false;
							player_lineRenderer.SetActive(true);
							lineRenderer.color = Color.red;
							player_lineRenderer.GetComponent<LineRenderer>().material.color = Color.red;
						}
						else if(!aux_grenade && !aux_rifle && !aux_run && !aux_melee){
							player_lineRendererActives.enabled = false;
							player_lineRenderer.SetActive(false);
							player_navMeshAgent.ResetPath();
						}
					}
					else if(!player_navMeshAgent.hasPath){
						aux_isPlaying = false;
					}
				}
			}
			else {
				aux_isPlaying = false;
				player_lineRenderer.SetActive(false);
				/*if(Input.GetKey(KeyCode.LeftShift)){
					if(Input.GetMouseButtonDown(1)){
						Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
						if(Physics.Raycast(ray.origin,ray.direction,out clickMouse,100,clickMouse_layermask)){
							GameObject[] aux = new GameObject[teste.Length+1];
							for (int i = 0; i < teste.Length; i++) {
								aux[i] = teste[i];
							}
							aux[aux.Length-1] = Instantiate (destination_prefab,clickMouse.point,Quaternion.identity) as GameObject;
							teste = new GameObject[aux.Length];
							for (int i = 0; i < teste.Length; i++) {
								teste[i] = aux[i];
							}
						}
					}
				}
				else if(Input.GetKeyUp(KeyCode.LeftShift)){
					followApath = true;
					player_navMeshAgent.destination = teste[0].transform.position;
					if(destination_instantiate){
						Destroy(destination_instantiate);
					}
				}*/
				if(Input.GetMouseButtonDown(1)){
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					if(Physics.Raycast(ray.origin,ray.direction,out clickMouse,100,clickMouse_layermask)){
						Debug.DrawRay (ray.origin,ray.direction * 100);
						followApath = false;
						Destroy(destination_instantiate);
						destination_instantiate = Instantiate (destination_prefab,Vector3.zero,Quaternion.identity) as GameObject;
						destination_instantiate.transform.position = clickMouse.point;
						player_navMeshAgent.destination = clickMouse.point;
						for (int i = 0; i < teste.Length; i++) {
							Destroy(teste[i]);
						}
					}
				}
				else if(Input.GetMouseButtonDown(0) && grenadeEnabledFreeMode){
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					if(Physics.Raycast(ray.origin,ray.direction,out clickMouse,100,clickMouse_layermask)){
						if(Vector3.Distance(transform.position,clickMouse.point) < 20){
							Debug.DrawRay (ray.origin,ray.direction * 100);
							Vector3 grenade_target = new Vector3();
							grenade_target = clickMouse.point;
							player_animation.SetTrigger("grenade");
							grenadeEnabledFreeMode = false;
							grenadeFreeModeCooldown = 0;
							StartCoroutine(ThrowGrenade(grenade_target,1f));
						}
					}
				}

				if(Input.GetKeyDown(KeyCode.R)){
					isRun = !isRun;
				}

				if(followApath){
					for (int i = 0; i < teste.Length; i++) {
						if(Vector3.Distance(transform.position,teste[i].transform.position) < 1.5f){
							teste[i].renderer.enabled = false;
							if(teste.Length > i+1){
								player_navMeshAgent.destination = teste[i+1].transform.position;
							}
							else{
								foreach (GameObject item in teste) {
									Destroy(item);
								}
								teste = new GameObject[]{};
								followApath = false;
							}
						}
					}
				}
				if(!grenadeEnabledFreeMode){
					grenadeFreeModeCooldown += Time.deltaTime * 1.66f;
					if(grenadeFreeModeCooldown >= 100){
						grenadeFreeModeCooldown = 100;
						grenadeEnabledFreeMode = true;
					}
				}
			}
		}
	}

	public void StartCombatMode(){
		player_navMeshAgent.ResetPath ();
	}

	IEnumerator ThrowGrenade(Vector3 target, float delayTime){
		yield return new WaitForSeconds (delayTime);
		AudioSource.PlayClipAtPoint(sd_throwGrenade,new Vector3(transform.position.x,11.33f,transform.position.z));
		GameObject aux = Instantiate(grenade,grenade_spawnAtHand.position,Quaternion.identity) as GameObject;
		aux.SendMessage("Grenade_target", target);
	}
	IEnumerator RifleDamage(int delay, RaycastHit click_shoot){
		yield return new WaitForSeconds (delay);
		click_shoot.transform.SendMessage("Damaged",45);
		flameShoot.Play ();
	}
	IEnumerator MeleeDamage(int delay, RaycastHit click_shoot){
		AudioSource.PlayClipAtPoint(sd_knife,new Vector3(transform.position.x,11.33f,transform.position.z));
		yield return new WaitForSeconds (delay);
		click_shoot.transform.SendMessage("Damaged",80);
	}

	public void StatusAction(string status){
		if(status == "grenade"){
			aux_grenade = true;
			aux_rifle = false;
			aux_run = false;
			aux_melee = false;
		}
		else if(status == "rifle"){
			aux_grenade = false;
			aux_rifle = true;
			aux_run = false;
			aux_melee = false;
		}
		else if(status == "melee"){
			aux_grenade = false;
			aux_rifle = false;
			aux_run = false;
			aux_melee = true;
		}

		else if(status == "run"){
			aux_grenade = false;
			aux_rifle = false;
			aux_run = true;
			aux_melee = false;
		}
		else if(status == "reset"){
			aux_grenade = false;
			aux_rifle = false;
			aux_run = false;
			aux_melee = false;
		}
	}
	void OnGUI(){
		if(!Script_HUD.pause){
			if(aux_run){
				GUI.Label(new Rect(Input.mousePosition.x,Screen.height-Input.mousePosition.y-50,0,0),consumeAP.ToString()+"pts",GUIStyle.none);
			}
		}
	}
	public void Animations(string anim){
		if(anim == "damage"){
			player_animation.SetTrigger("damage");
		}
		if(anim == "death"){
			player_animation.SetTrigger("dead");
			Destroy(player_gameObject.GetComponent<Script_Player>());
		}
	}

	public bool[] CurrentStatus(){
		bool[] aux = new bool[]{aux_grenade, aux_rifle, aux_run, aux_melee};
		return aux;
	}
}
