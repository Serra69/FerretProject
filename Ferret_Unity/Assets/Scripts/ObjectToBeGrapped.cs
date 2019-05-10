using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToBeGrapped : MonoBehaviour {

	[Header("Range")]
	[SerializeField] float m_takingRange = 1;

	[Header("Gizmos")]
	[SerializeField] bool m_showGizmos = true;
	[SerializeField] Color m_color = Color.white;

	Collider m_collider;
	Rigidbody m_rbody;

	PlayerManager m_playerManager;
	Transform m_posToFollow;
	bool m_followPlayer = false;

	bool m_canBeGrapped = true;
    public bool CanBeGrapped
    {
        get
        {
            return m_canBeGrapped;
        }

        set
        {
            m_canBeGrapped = value;
        }
    }

    void Start(){
		m_playerManager = PlayerManager.Instance;
		m_posToFollow = m_playerManager.m_states.m_takeObject.m_objectPosition;

		m_collider = GetComponent<Collider>();
		m_rbody = GetComponent<Rigidbody>();
	}

	public void On_ObjectIsTake(bool objectIsTake){
		// Debug.Log("My name is : " + gameObject.name + " : On_ObjectIsTake = " + objectIsTake);
		m_followPlayer = objectIsTake;
		On_SetGrappedObjectCharacteristic(objectIsTake);
	}

	void Update(){
		if(!m_followPlayer && m_canBeGrapped){
			CheckPlayerPosition();
		}
		if(!m_canBeGrapped){
			m_playerManager.SetClosedObjectToBeGrapped(false, this);
		}
	}

	void CheckPlayerPosition(){
		if(Vector3.Distance(transform.position, m_playerManager.transform.position) <= m_takingRange){
			m_playerManager.SetClosedObjectToBeGrapped(true, this);
		}else{
			m_playerManager.SetClosedObjectToBeGrapped(false, this);
		}
	}

	void On_SetGrappedObjectCharacteristic(bool b){
		if(b){
			if(m_collider != null){
				m_collider.enabled = false;
			}
			if(m_rbody != null){
				m_rbody.isKinematic = true;
			}
		}else{
			if(m_collider != null){
				m_collider.enabled = true;
			}
			if(m_rbody != null){
				m_rbody.isKinematic = false;
			}
		}
	}

	void LateUpdate(){
		if(m_followPlayer){
			transform.position = m_posToFollow.transform.position;
			transform.rotation = m_posToFollow.transform.rotation;
		}
	}

	void OnDrawGizmosSelected(){
		if(m_showGizmos){
			Gizmos.color = m_color;
			Gizmos.DrawWireSphere(transform.position, m_takingRange);
		}
	}

}
