using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CheckPoint : MonoBehaviour {

	[Header("Respawn position")]
	[Tooltip("Reference a transform to respawn. If empty, CheckPoint transform is used")]
	[SerializeField] Transform m_savePosition;

	[Header("Gizmos")]
	[SerializeField] bool m_showGizmos = true;
	[SerializeField] Color m_gizmosColor = Color.blue;

    public BoxCollider BoxCol{
        get{
            return GetComponent<BoxCollider>();
        }
    }

	PlayerManager m_playerManager;
	bool m_playerPositionIsSave = false;

	void Start(){
		m_playerManager = PlayerManager.Instance;
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			if(!m_playerPositionIsSave){
				m_playerPositionIsSave = true;
				if(m_savePosition != null){
					m_playerManager.On_CheckPointIsTake(m_savePosition);
				}else{
					m_playerManager.On_CheckPointIsTake(transform);
				}
			}
		}
	}

    void OnDrawGizmos(){
		if(!m_showGizmos){
			return;
		}
		Gizmos.color = m_gizmosColor;
		Gizmos.DrawWireCube(transform.position + BoxCol.center, BoxCol.size);
	}

}
