using UnityEngine;
using System.Collections;

public class LineRendererPlayer : MonoBehaviour {

	public Transform positionFather;
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
		}
	}
}
