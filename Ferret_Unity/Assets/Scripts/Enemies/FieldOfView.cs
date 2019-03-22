﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour {

	public float m_viewRadius = 5f;
	
	[Range(0,360)] public float m_viewAngle = 90f;

	public LayerMask m_targetMask;
	public LayerMask m_obstacleMask;

	[HideInInspector] public GameObject m_target;

	void Start() {
		StartCoroutine ("FindTargetsWithDelay", 0.2f);
	}

	IEnumerator FindTargetsWithDelay(float delay) {
		while (true) {
			yield return new WaitForSeconds (delay);
			FindVisibleTargets ();
		}
	}

	void FindVisibleTargets() {
		m_target = null;
		Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, m_viewRadius, m_targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++) {
			Transform target = targetsInViewRadius [i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle (transform.forward, dirToTarget) < m_viewAngle / 2) {
				float dstToTarget = Vector3.Distance (transform.position, target.position);

				if (!Physics.Raycast (transform.position, dirToTarget, dstToTarget, m_obstacleMask)) {
					m_target = target.GetComponentInParent<PlayerManager>().gameObject;
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

	void Update(){	
		if(GetComponent<RobotTurret>() || GetComponent<RobotDoctor>()){
			LookAtTarget();
		}
	}

	void LookAtTarget(){
		if(m_target != null){
			transform.LookAt(m_target.transform);
		}
	}

}
