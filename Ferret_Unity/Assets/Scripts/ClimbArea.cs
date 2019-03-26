using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbArea : MonoBehaviour {

	[SerializeField] Color m_areaColor = Color.white;
	[SerializeField] BoxCollider m_col;

	void OnDrawGizmos(){

		if(m_col == null){
			return;
		}

		Gizmos.color = m_areaColor;
		Gizmos.DrawWireCube(transform.position + m_col.center, m_col.size);
	}

}
