using UnityEngine;
using System.Collections;

public class Script_Worm : MonoBehaviour {
	
	public Font _font;
	public AudioClip sd_ataque;
	public AudioClip sd_saindo;
	public AudioClip sd_dead;
	public AudioClip sd_damaged;
	GameObject player_gameObject;
	Animator worm_animator;
	Transform worm_target;
	GameObject HUD;
	int life = 150;
	int status = 0;
	bool justDoIt = false;
	bool isEnemyTurn = false;
	bool imDone = false;
	
	void Start () {
		transform.localRotation = Quaternion.Euler(0,Random.Range (0f,360f),0);
		life = 150;
		status = 0;
		justDoIt = true;
		imDone = true;
		isEnemyTurn = false;
		player_gameObject = GameObject.FindWithTag("Player");
		HUD = GameObject.Find ("HUD");
		worm_animator = GetComponentInChildren<Animator>();
		GUIStyle.none.font = _font;
		GUIStyle.none.normal.textColor = Color.white;
	}

	void Update () {
		if(!Script_HUD.pause){
			if(Script_HUD.isCombatMode){
				if(isEnemyTurn){
					if(justDoIt){
						imDone = false;
						//isEnemyTurn = false;
						justDoIt = false;
						switch (status){
						case 0:
							int rnd = Random.Range(0,2);
							if(rnd == 0){
								rnd = 4;
							}
							else{
								rnd = 6;
							}
							transform.position = player_gameObject.transform.position + (transform.forward*rnd);
							transform.LookAt(player_gameObject.transform.position);
							worm_animator.SetTrigger("sair");
							AudioSource.PlayClipAtPoint(sd_saindo,new Vector3(transform.position.x,11.33f,transform.position.z));
							status = 1;
							if(rnd == 4){
								StartCoroutine(Damaged(10,1));
								transform.LookAt(player_gameObject.transform.position);
							}
							StartCoroutine("IamDone",2);
							break;
						case 1:
							if(Vector3.Distance(player_gameObject.transform.position,transform.position) < 6){
								worm_animator.SetTrigger("attack");
								AudioSource.PlayClipAtPoint(sd_ataque,new Vector3(transform.position.x,11.33f,transform.position.z));
								StartCoroutine(Damaged(40,1));
								transform.LookAt(player_gameObject.transform.position);
							}
							else{
								worm_animator.SetTrigger("entrar");
							}
							status = 0;
							StartCoroutine("IamDone",2);
							break;
						case 2:
							worm_animator.SetTrigger("entrar");
							status = 0;
							StartCoroutine("IamDone",2);
							break;
						case 3:
							status = 0;
							StartCoroutine("IamDone",2);
							break;
						}
					}
				}
			}
		}
	}
	void OnGUI(){
		if(Script_HUD.isCombatMode && !Script_HUD.pause){
			if(worm_animator.GetCurrentAnimatorStateInfo(0).IsName("idle")){
				Vector2 hudLife = new Vector2(
					Camera.main.WorldToScreenPoint(transform.position).x,
					Screen.height - Camera.main.WorldToScreenPoint(transform.position).y - 80);
				GUI.Label (new Rect(hudLife.x,hudLife.y,100,100),life+" / 150",GUIStyle.none);
			}
		}
	}

	void Damaged(int dano){
		worm_animator.SetTrigger ("damage");
		AudioSource.PlayClipAtPoint(sd_damaged,new Vector3(transform.position.x,11.33f,transform.position.z));
		life -= dano;
		if(life <= 0){
			AudioSource.PlayClipAtPoint(sd_dead,new Vector3(transform.position.x,11.33f,transform.position.z));
			worm_animator.SetBool ("death",true);
			Destroy(this.gameObject.GetComponent<Script_Worm> ());
			this.gameObject.name = "dead";
			this.gameObject.layer = 1 << 0;
		}
	}

	public void GrenadedInCombat(){

		if(status == 1){
			status = 2;
		}
		else if(status == 0){
			status = 3;
		}
	}

	public bool AmIdone(){
		return isEnemyTurn//imDone
			;
	}

	IEnumerator IamDone(float delaySec){
		yield return new WaitForSeconds (delaySec);
		imDone = true;
		isEnemyTurn = false;
	}
	IEnumerator Damaged(int dano,int delay){
		yield return new WaitForSeconds (delay);
		HUD.SendMessage("Damaged",dano);
	}
	public void EnemyTurn(){
		if(Vector3.Distance(transform.position,player_gameObject.transform.position) < 20){
			isEnemyTurn = true;
			justDoIt = true;
		}
	}
}
