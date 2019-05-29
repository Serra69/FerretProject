﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour {

	[Header("Object can fall?")]
	[SerializeField] bool m_objectAllowToFall = true;

	[Header("Snap positions")]
	[SerializeField] Transform[] m_snapPositions;

	[Header("Raycast Positions")]
	[SerializeField] float m_maxDistance = 0.25f;
	[SerializeField] Color m_rayColor = Color.green;
	[SerializeField] LayerMask m_whatIsGround;
	[SerializeField] Transform[] m_raycastPositions;

	[Header("Boxcast")]
	[SerializeField] LayerMask m_objectColliders;
	[Space]
	[SerializeField] float m_castDistanceForward = 2;
	[SerializeField] Vector3 m_boxSizeForward;
	[Space]
	[SerializeField] float m_castDistanceRight = 2;
	[SerializeField] Vector3 m_boxSizeRight;
	[SerializeField] Color m_boxColor = Color.magenta;
	
	bool[] m_raycastReturn;

	PlayerManager m_playerManager;
	float actualClosedDistance;
	Transform m_closedTransform = null;
	bool m_objectIsPushByTheFerret = false;
	Rigidbody m_rigidbody;

    bool m_canMove = true;
    public bool CanMove
    {
        get
        {
            return m_canMove;
        }

    }

    int m_closedPosition;
    public int ClosedPosition
    {
        get
        {
            return m_closedPosition;
        }
    }

    BoxCollider m_boxColl;
    public BoxCollider BoxColl{
        get{
            return GetComponent<BoxCollider>();
        }
        set{
            m_boxColl = value;
        }
    }



    void Start(){
		m_raycastReturn = new bool[m_raycastPositions.Length];
		m_rigidbody = GetComponent<Rigidbody>();
		m_playerManager = PlayerManager.Instance;
	}

	void Update(){
		if(!m_objectAllowToFall && m_objectIsPushByTheFerret){
			CheckGround();
		}
			BoxCast();
	}

	void CheckGround(){
		for(int i = 0, l = m_raycastPositions.Length; i < l; ++i){
			m_raycastReturn[i] = Physics.Raycast(m_raycastPositions[i].transform.position, - m_raycastPositions[i].transform.up, m_maxDistance, m_whatIsGround);
		}

		int falseNbs = 0;
		for(int i = 0, l = m_raycastReturn.Length; i < l; ++i){
			if(!m_raycastReturn[i]){
				m_canMove = false;
			}else{
				falseNbs ++;
			}
			if(falseNbs == l){
				m_canMove = true;
			}
		}
	}

	void OnDrawGizmos(){
		for(int i = 0, l = m_raycastPositions.Length; i < l; ++i){
			Debug.DrawRay(m_raycastPositions[i].transform.position, - m_raycastPositions[i].transform.up * m_maxDistance, m_rayColor, 0.1f);
		}

		Gizmos.color = m_boxColor;
		Vector3 boxSizeForward = new Vector3(m_boxSizeForward.x * transform.localScale.x, m_boxSizeForward.y * transform.localScale.y, m_boxSizeForward.z * transform.localScale.z);
		Vector3 boxSize = new Vector3(m_boxSizeRight.x * transform.localScale.x, m_boxSizeRight.y * transform.localScale.y, m_boxSizeRight.z * transform.localScale.z);

		// Gizmos.DrawCube(transform.position + On_PlayerSnapToObject().forward * m_boxCastDistance, boxSize);
		if(m_playerManager != null){
			/*if(On_PlayerSnapToObject() == m_snapPositions[0]){
				Gizmos.DrawCube(transform.position + Vector3.back * m_castDistanceForward, boxSizeForward);
			}else if(On_PlayerSnapToObject() == m_snapPositions[1]){
				Gizmos.DrawCube(transform.position + Vector3.left * m_castDistanceRight, boxSize);
			}else if(On_PlayerSnapToObject() == m_snapPositions[2]){
				Gizmos.DrawCube(transform.position + Vector3.forward * m_castDistanceForward, boxSizeForward);
			}else if(On_PlayerSnapToObject() == m_snapPositions[3]){
				Gizmos.DrawCube(transform.position + Vector3.right * m_castDistanceRight, boxSize);
			}*/

			Gizmos.DrawCube(transform.position + Vector3.forward * m_castDistanceForward, boxSizeForward);

		}

	}

	void BoxCast(){
		Vector3 boxSizeForward = new Vector3(m_boxSizeForward.x * transform.localScale.x, m_boxSizeForward.y * transform.localScale.y, m_boxSizeForward.z * transform.localScale.z);
		Vector3 boxSize = new Vector3(m_boxSizeRight.x * transform.localScale.x, m_boxSizeRight.y * transform.localScale.y, m_boxSizeRight.z * transform.localScale.z);
		
		Collider[] hitColliders = Physics.OverlapBox(transform.position + Vector3.forward * m_castDistanceForward, boxSizeForward, transform.rotation, m_objectColliders);

		int colliders = 0;
		for (int i = 0, l = hitColliders.Length; i < l; ++i){
			if(hitColliders[i] != BoxColl){
				Debug.Log("hitColliders = " + hitColliders[i].gameObject.name);
				colliders ++;
			}
		}

	}

	public Transform On_PlayerSnapToObject(){
		for(int i = 0, l = m_snapPositions.Length; i < l; ++i){
			float distance = Vector3.Distance(m_playerManager.transform.position, m_snapPositions[i].position);
			
			if(m_closedTransform != null){
				actualClosedDistance = Vector3.Distance(m_playerManager.transform.position, m_closedTransform.position);

				if(distance < actualClosedDistance){
					m_closedTransform = m_snapPositions[i];
					m_closedPosition = i;
				}

			}else{
				actualClosedDistance = distance;
				m_closedTransform = m_snapPositions[i];
			}
		}
		return m_closedTransform;
	}

	public void On_ObjectIsPushing(bool b){
		m_objectIsPushByTheFerret = b;

		if(b){
            m_rigidbody.isKinematic = false;
        }
        else
        {
            m_rigidbody.isKinematic = true;
        }
	}
	
}
