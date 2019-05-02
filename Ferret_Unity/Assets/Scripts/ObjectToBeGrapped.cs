using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToBeGrapped : MonoBehaviour {

	[Header("Range")]
	[SerializeField] float m_takingRange = 1;

	[Header("Gizmos")]
	[SerializeField] bool m_showGizmos = true;
	[SerializeField] Color m_color = Color.white;

	void FixedUpdate(){

	}

	void OnDrawGizmos(){
		if(m_showGizmos){
			Gizmos.color = m_color;
			Gizmos.DrawWireSphere(transform.position, m_takingRange);
		}
	}

}
