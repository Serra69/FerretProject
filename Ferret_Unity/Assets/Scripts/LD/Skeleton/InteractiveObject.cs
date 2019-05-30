using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class InteractiveObject : MonoBehaviour {

	[Header("Gizmos")]
	[SerializeField] bool m_showGizmos = true;
	[SerializeField] Color m_color = Color.white;

	public BoxCollider BoxColl{
        get{
            return GetComponent<BoxCollider>();
        }
    }

	PlayerManager m_playerManager;
	SkeletonController m_skeletonController;
	bool m_isUsed = false;

    void Start(){
		m_playerManager = PlayerManager.Instance;
		BoxColl.isTrigger = true;
	}

	public void SetSkeletonController(SkeletonController skeletonController){
		m_skeletonController = skeletonController;
	}

	public void On_ObjectIsTake(){
		m_isUsed = true;
		m_skeletonController.On_PikeIsDisable();
		m_playerManager.SetClosedInteractiveObject(null, false);
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player") && !m_isUsed){
			m_playerManager.SetClosedInteractiveObject(this, true);
		}
	}
	void OnTriggerExit(Collider col){
		if(col.CompareTag("Player")){
			m_playerManager.SetClosedInteractiveObject(this, false);
		}
	}


	void OnDrawGizmos(){
		if(m_showGizmos){
			Gizmos.color = m_color;
			Gizmos.DrawWireCube(transform.position + BoxColl.center, BoxColl.size);
		}
	}

}
