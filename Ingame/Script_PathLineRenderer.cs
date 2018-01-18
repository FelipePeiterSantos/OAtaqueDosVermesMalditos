using UnityEngine;
using System.Collections;

public class Script_PathLineRenderer : MonoBehaviour {

	public Transform positionFather;
	public float lineRenderer_length;
	NavMeshAgent navMeshAgent;
	LineRenderer lineRenderer;

	void Start(){
		lineRenderer = GetComponent<LineRenderer>();
		navMeshAgent = GetComponent<NavMeshAgent>();
	}

	void Update(){
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit mousePointing;
		if (Physics.Raycast (ray.origin, ray.direction, out mousePointing, 100, 1 << LayerMask.NameToLayer("chao"))) {
			navMeshAgent.destination = mousePointing.point;
		}
		if(navMeshAgent.hasPath){
			lineRenderer.SetVertexCount(navMeshAgent.path.corners.Length);
			for (int i = 0; i < navMeshAgent.path.corners.Length; i++) {
				lineRenderer.SetPosition(i,navMeshAgent.path.corners[i]);
			}
			lineRenderer.SetPosition(0,new Vector3(positionFather.position.x,0,positionFather.position.z));
			lineRenderer_length = 0;
			for (int i = 0; i < navMeshAgent.path.corners.Length-1; i++) {
				lineRenderer_length += (navMeshAgent.path.corners[i] - navMeshAgent.path.corners[i+1]).magnitude;
			}
		}
	}
}
