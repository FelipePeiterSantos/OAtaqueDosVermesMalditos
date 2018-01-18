using UnityEngine;
using System.Collections;

public class Script_Bipede : MonoBehaviour {

	public LayerMask clickMouse_layermask;
	public Renderer[] bidepe_3Dmesh;
	public Animator bipede_animator;
	public Font _font;
	public AudioClip sd_attack;
	public AudioClip sd_dead;
	public AudioClip sd_agro;

	float move_cooldown;
	float move_cooldownRandom;
	float move_cooldownAgro;
	int status = 0;
	int life = 90;
	bool imDone = true;
	bool attackOnce = true;
	bool chooseSide = true;
	bool isEnemyTurn = false;
	bool grenadedInCombat;
	Vector3 grenadedTarget = new Vector3(0,0,0);
	GameObject HUD;
	GameObject player_gameObject;
	Transform attack_target;
	RaycastHit clickMouse;
	NavMeshAgent bidepe_navMeshAgent;
	Vector3 move_agro;
	Vector3 move_initialPosition;
	Vector3 initialPosition;

	void Start(){
		grenadedInCombat = false;
		imDone = true;
		chooseSide = true;
		isEnemyTurn = false;
		life = 90;
		player_gameObject = GameObject.FindWithTag ("Player");
		HUD = GameObject.Find ("HUD");
		bidepe_navMeshAgent = GetComponent<NavMeshAgent>();
		move_cooldownRandom = Random.Range (5f, 20f);
		move_cooldownAgro = 0;
		move_initialPosition = transform.position;
		Status_Idle ();
		GUIStyle.none.font = _font;
		GUIStyle.none.normal.textColor = Color.white;
	}

	void Update () {
		if(!Script_HUD.pause){
			if(Vector3.Distance(transform.position,player_gameObject.transform.position) > 15){
				isEnemyTurn = false;
			}
			if(Script_HUD.isCombatMode){
				if(status == 0 || status == 1 && bidepe_navMeshAgent.hasPath){
					status = 2;
					bidepe_navMeshAgent.destination = transform.position;
					bipede_animator.SetInteger("status",0);
				}
				bidepe_navMeshAgent.speed = 4;
				bidepe_navMeshAgent.stoppingDistance = 2;
				if(isEnemyTurn && Vector3.Distance(transform.position,player_gameObject.transform.position) < 20){
					if(Vector3.Distance(player_gameObject.transform.position,transform.position) < 8 && attackOnce && !grenadedInCombat){
						imDone = false;
						chooseSide = false;
						bidepe_navMeshAgent.destination = player_gameObject.transform.position;
						if (bidepe_navMeshAgent.hasPath){
							if(bidepe_navMeshAgent.remainingDistance < bidepe_navMeshAgent.stoppingDistance){
								bidepe_navMeshAgent.ResetPath();
								bipede_animator.SetTrigger("attack");
								bipede_animator.SetInteger("status",0);
								StartCoroutine("DealDamage",1);
								StartCoroutine("IalreadyPlayed",2f);
								attackOnce = false;
							}
							else{
								bipede_animator.SetInteger("status",2);
							}
						}
						else if(imDone){
							attackOnce = true;
						}
					}
					else if(grenadedInCombat){
						imDone = false;
						attackOnce = false;
						chooseSide = false;
						bidepe_navMeshAgent.destination = grenadedTarget;
						if(bidepe_navMeshAgent.hasPath){
							bipede_animator.SetInteger("status",2);
							if(bidepe_navMeshAgent.remainingDistance < bidepe_navMeshAgent.stoppingDistance){
								bidepe_navMeshAgent.ResetPath();
								grenadedInCombat = false;
							}
						}
					}
					else{
						if (chooseSide){
							imDone = false;
							attackOnce = false;
							chooseSide = false;
							Vector3 aux = player_gameObject.transform.position;
							int rnd = Random.Range(0,4);
							if(rnd == 0){
								aux += transform.right * 6;
							}
							else if(rnd == 1){
								aux -= transform.right * 6;
							}
							else if(rnd == 2){
								aux -= transform.forward * 6;
							}
							else if(rnd == 3){
								aux -= transform.forward * 6;
							}
							transform.LookAt(aux);
							bidepe_navMeshAgent.destination = aux;
							initialPosition = transform.position;
						}
						else if(bidepe_navMeshAgent.hasPath){
							bipede_animator.SetInteger("status",2);
							if(bidepe_navMeshAgent.remainingDistance < bidepe_navMeshAgent.stoppingDistance){
								bidepe_navMeshAgent.ResetPath();
							}
						}
						else if (!imDone){
							bipede_animator.SetInteger("status",0);
							StartCoroutine("IalreadyPlayed",2f);
						}
						/*else{
							isEnemyTurn = false;
						}*/
					}
				}
			}	

			else{
				if(status != 0 && status != 1){
					status = 0;
				}
				bidepe_navMeshAgent.stoppingDistance = 2;
				if(status == 0){
					move_cooldown += Time.deltaTime;
					if(move_cooldown > move_cooldownRandom){
						move_cooldown = 0;
						move_cooldownRandom = Random.Range (2f, 5f);
						bidepe_navMeshAgent.destination = new Vector3(move_initialPosition.x + (6f * Random.Range(-1f,1f)),0,move_initialPosition.z + (6f * Random.Range(-1f,1f)));
					}
					if(bidepe_navMeshAgent.hasPath){
						bidepe_navMeshAgent.speed = 1;
						bipede_animator.SetInteger("status",1);
						if(bidepe_navMeshAgent.remainingDistance < bidepe_navMeshAgent.stoppingDistance){
							bidepe_navMeshAgent.ResetPath();
						}
					}
					else{
						bipede_animator.SetInteger("status",0);
					}
				}
				else if(status == 1){
					if(move_cooldownAgro < 5f){
						bipede_animator.SetInteger("status",0);
						move_cooldownAgro += Time.deltaTime;
						bidepe_navMeshAgent.destination = move_agro;
						bidepe_navMeshAgent.speed = 0;
						Quaternion lookAtAgro = Quaternion.LookRotation(-transform.position + move_agro);
						transform.rotation = Quaternion.Slerp(transform.rotation,lookAtAgro,Time.deltaTime * 5f);
					}
					else if(bidepe_navMeshAgent.hasPath){
						if(bidepe_navMeshAgent.remainingDistance < bidepe_navMeshAgent.stoppingDistance){
							bidepe_navMeshAgent.ResetPath();
						}
						bidepe_navMeshAgent.speed = 4;
						bipede_animator.SetInteger("status",2);
					}
					else if(move_cooldownAgro < 8f){
						move_cooldownAgro += Time.deltaTime;
						bipede_animator.SetTrigger("attack");
					}
					else{
						Status_Idle();
					}
				}
			}
		}
	}
	IEnumerator IalreadyPlayed(int delay){
		yield return new WaitForSeconds (delay);
		bipede_animator.SetInteger("status",0);	
		imDone = true;
		chooseSide = true;
		attackOnce = true;
		bidepe_navMeshAgent.ResetPath();
		isEnemyTurn = false;
		status = 0;
	}
	IEnumerator DealDamage(int delay){
		yield return new WaitForSeconds (0.5f);
		AudioSource.PlayClipAtPoint(sd_attack,new Vector3(transform.position.x,11.33f,transform.position.z));
		yield return new WaitForSeconds (delay-0.5f);
		HUD.SendMessage("Damaged",15);
		transform.LookAt(new Vector3(player_gameObject.transform.position.x,transform.position.y,player_gameObject.transform.position.z));
	}

	public void Status_Agro(Vector3 agro_target){
		Vector2 aux = Camera.main.WorldToViewportPoint (transform.position);
		if(Vector3.Distance(agro_target,transform.position) < 20){
			AudioSource.PlayClipAtPoint(sd_agro,new Vector3(transform.position.x,11.33f,transform.position.z),0.5f);
			status = 1;
			move_cooldownAgro = 0;
			move_agro = agro_target;
		}
	}

	public bool AmIdone(){
		return isEnemyTurn//imDone
			;
	}

	public void EnemyTurn(){
		if(Vector3.Distance(transform.position,player_gameObject.transform.position) < 15){
			isEnemyTurn = true;
		}
		if(grenadedInCombat && !isEnemyTurn){
			grenadedInCombat = false;
		}
	}

	public void GrenadedInCombat(Vector3 target){
		Vector2 aux = Camera.main.WorldToViewportPoint (transform.position);
		if (Vector3.Distance(target,transform.position) < 20) {
			grenadedTarget = target;
			grenadedInCombat = true;	
		}
	}

	void Status_Idle(){
		bidepe_navMeshAgent.speed = 1;
		status = 0;
	}

	void Damaged(int dano){
		bipede_animator.SetTrigger ("damaged");
		life -= dano;
		AudioSource.PlayClipAtPoint(sd_attack,new Vector3(transform.position.x,11.33f,transform.position.z));
		if(life <= 0){
			AudioSource.PlayClipAtPoint(sd_dead,new Vector3(transform.position.x,11.33f,transform.position.z));
			bidepe_navMeshAgent.enabled = false;
			bipede_animator.SetBool ("death",true);
			Destroy(this.gameObject.GetComponent<Script_Bipede> ());
			this.gameObject.name = "dead";
			this.gameObject.layer = 1 << 0;
		}
	}
	IEnumerator DeadAsHell(float delaySec){
		yield return new WaitForSeconds (delaySec);

	}
	void OnGUI(){
		if(Script_HUD.isCombatMode && !Script_HUD.pause){
			Vector2 hudLife = new Vector2(
				Camera.main.WorldToScreenPoint(transform.position).x,
				Screen.height - Camera.main.WorldToScreenPoint(transform.position).y - 50);
			GUI.Label (new Rect(hudLife.x,hudLife.y,100,100),life+" / 90",GUIStyle.none);
		}
	}
}
