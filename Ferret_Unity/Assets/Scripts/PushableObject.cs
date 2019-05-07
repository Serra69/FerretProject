using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour {

	[SerializeField] Transform[] m_snapPositions;

	PlayerManager m_playerManager;

	float actualClosedDistance;
	Transform m_closedTransform = null;

	void Start(){
		m_playerManager = PlayerManager.Instance;
	}

	public Transform On_PlayerSnapToObject(){
		for(int i = 0, l = m_snapPositions.Length; i < l; ++i){
			float distance = Vector3.Distance(m_playerManager.transform.position, m_snapPositions[i].position);
			
			if(m_closedTransform != null){
				actualClosedDistance = Vector3.Distance(m_playerManager.transform.position, m_closedTransform.position);

				if(distance < actualClosedDistance){
					m_closedTransform = m_snapPositions[i];
				}

			}else{
				actualClosedDistance = distance;
				m_closedTransform = m_snapPositions[i];
			}
		}
		return m_closedTransform;
	}
	
}
