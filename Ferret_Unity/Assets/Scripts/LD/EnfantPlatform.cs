using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnfantPlatform : MonoBehaviour {

	[SerializeField] Transform m_newPlayerParent;

	PlayerManager m_playerManager;

    // public BoxCollider BoxCol{
    //     get{
    //         return GetComponent<BoxCollider>();
    //     }
    // }

	void Start(){
		m_playerManager = PlayerManager.Instance;
		// BoxCol.isTrigger = true;
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			if(m_newPlayerParent != null){
				m_playerManager.SetPlayerParent(m_newPlayerParent);
			}else{
				m_playerManager.SetPlayerParent(transform);
			}
		}
	}

	void OnTriggerExit(Collider col){
		if(col.CompareTag("Player")){
			m_playerManager.SetPlayerParent(m_newPlayerParent, true);
		}
	}

}
