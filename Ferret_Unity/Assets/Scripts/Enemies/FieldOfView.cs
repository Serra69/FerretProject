using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour {

	public float m_viewRadius = 5f;

	[Range(0,360)]	public float m_viewAngle = 45f;

	public LayerMask m_targetMask;
	public LayerMask m_obstacleMask;

	[HideInInspector] public List<Transform> m_visibleTargets = new List<Transform>();

	void Start() {
		StartCoroutine(FindTargetsWithDelay(.2f));
	}

	IEnumerator FindTargetsWithDelay(float delay) {
		while (true) {
			yield return new WaitForSeconds (delay);
			FindVisibleTargets ();
		}
	}

	void FindVisibleTargets() {
		m_visibleTargets.Clear ();
		Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, m_viewRadius, m_targetMask);

		for (int i = 0, length = targetsInViewRadius.Length; i < length; ++i) {
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle (transform.forward, dirToTarget) < m_viewAngle / 2) {
				float dstToTarget = Vector3.Distance (transform.position, target.position);

				if (!Physics.Raycast (transform.position, dirToTarget, dstToTarget, m_obstacleMask)) {
					m_visibleTargets.Add (target);
				}
			}
		}
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

}
