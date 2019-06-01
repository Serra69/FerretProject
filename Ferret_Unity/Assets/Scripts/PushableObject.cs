using System.Collections;
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
	[Space]
	[SerializeField] float m_minusY = 0.1f;

	bool[] m_raycastReturn;
	PlayerManager m_playerManager;
	float actualClosedDistance;
	Transform m_closedTransform = null;
	bool m_objectIsPushByTheFerret = false;
	Rigidbody m_rigidbody;

	Vector3 sizeForAndBack;
	Vector3 centerForward;
	Vector3 centerBackward;

	Vector3 sizeRAndL;
	Vector3 centerRight;
	Vector3 centerLeft;

    bool m_canMove = true;
    public bool CanMove{
        get{
            return m_canMove;
        }
    }

	bool m_boxCollForward = false;
	public bool BoxCollForward{
        get{
            return m_boxCollForward;
        }
    }
	bool m_boxCollBackward = false;
	public bool BoxCollBackward{
        get{
            return m_boxCollBackward;
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

			if(m_closedTransform == m_snapPositions[0] || m_closedTransform == m_snapPositions[2]){
				BoxCastForward(true);
			}else if(m_closedTransform == m_snapPositions[1] || m_closedTransform == m_snapPositions[3]){
				BoxCastForward(false);
			}
		}
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

	void BoxCastForward(bool isForward){

		if(isForward){

			Vector3 boxSizeForward = new Vector3(m_boxSizeForward.x * transform.lossyScale.x, m_boxSizeForward.y * transform.lossyScale.y, m_boxSizeForward.z * transform.lossyScale.z);
			
			Collider[] forwardHitColliders = Physics.OverlapBox(transform.position + m_playerManager.transform.forward * m_castDistanceForward, boxSizeForward / 2, transform.rotation, m_objectColliders);
			int forwardColliders = 0;
			for (int i = 0, l = forwardHitColliders.Length; i < l; ++i){
				if(forwardHitColliders[i] != BoxColl){
					// Debug.Log("forwardColliders = " + forwardHitColliders[i].gameObject.name);
					forwardColliders ++;
				}
			}
			m_boxCollForward = forwardColliders > 0 ? true : false;

			Collider[] backwardHitColliders = Physics.OverlapBox(transform.position - m_playerManager.transform.forward * m_castDistanceForward, boxSizeForward / 2, transform.rotation, m_objectColliders);
			int backwardColliders = 0;
			for (int i = 0, l = backwardHitColliders.Length; i < l; ++i){
				if(backwardHitColliders[i] != BoxColl){
					// Debug.Log("backwardColliders = " + backwardHitColliders[i].gameObject.name);
					backwardColliders ++;
				}
			}
			m_boxCollBackward = backwardColliders > 0 ? true : false;

		}else{
			Vector3 boxSizeRight = new Vector3(m_boxSizeRight.x * transform.localScale.x, m_boxSizeRight.y * transform.localScale.y, m_boxSizeRight.z * transform.localScale.z);
			
			Collider[] rightHitColliders = Physics.OverlapBox(transform.position + m_playerManager.transform.forward * m_castDistanceForward, boxSizeRight / 2, transform.rotation, m_objectColliders);
			int rightColliders = 0;
			for (int i = 0, l = rightHitColliders.Length; i < l; ++i){
				if(rightHitColliders[i] != BoxColl){
					// Debug.Log("rightColliders = " + rightHitColliders[i].gameObject.name);
					rightColliders ++;
				}
			}
			m_boxCollForward = rightColliders > 0 ? true : false;

			Collider[] leftHitColliders = Physics.OverlapBox(transform.position - m_playerManager.transform.forward * m_castDistanceForward, boxSizeRight / 2, transform.rotation, m_objectColliders);
			int leftColliders = 0;
			for (int i = 0, l = leftHitColliders.Length; i < l; ++i){
				if(leftHitColliders[i] != BoxColl){
					// Debug.Log("leftColliders = " + leftHitColliders[i].gameObject.name);
					leftColliders ++;
				}
			}
			m_boxCollBackward = leftColliders > 0 ? true : false;
			
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

	void InitializeBoxCastToSnap(){
		// FORWARD & BACKWARD
		sizeForAndBack = new Vector3(m_playerManager.m_colliders.m_baseCollider.m_radius * 2,
									m_playerManager.m_colliders.m_baseCollider.m_radius * 2 - m_minusY,
									m_playerManager.m_colliders.m_baseCollider.m_height);
		// FORWARD
		centerForward = new Vector3(m_snapPositions[0].position.x + m_playerManager.m_colliders.m_baseCollider.m_center.x,
									m_snapPositions[0].position.y + m_playerManager.m_colliders.m_baseCollider.m_center.y, 
									m_snapPositions[0].position.z - m_playerManager.m_colliders.m_baseCollider.m_center.z);
		// BACKWARD
		centerBackward = new Vector3(m_snapPositions[2].position.x + m_playerManager.m_colliders.m_baseCollider.m_center.x,
									m_snapPositions[2].position.y + m_playerManager.m_colliders.m_baseCollider.m_center.y, 
									m_snapPositions[2].position.z + m_playerManager.m_colliders.m_baseCollider.m_center.z);

		// RIGHT & LEFT
		sizeRAndL = new Vector3(m_playerManager.m_colliders.m_baseCollider.m_height,
									m_playerManager.m_colliders.m_baseCollider.m_radius * 2 - m_minusY,
									m_playerManager.m_colliders.m_baseCollider.m_radius * 2);
		// RIGHT
		centerRight = new Vector3(m_snapPositions[1].position.x - m_playerManager.m_colliders.m_baseCollider.m_center.z,
									m_snapPositions[1].position.y + m_playerManager.m_colliders.m_baseCollider.m_center.y, 
									m_snapPositions[1].position.z - m_playerManager.m_colliders.m_baseCollider.m_center.x);
		// LEFT
		centerLeft = new Vector3(m_snapPositions[3].position.x + m_playerManager.m_colliders.m_baseCollider.m_center.z,
									m_snapPositions[3].position.y + m_playerManager.m_colliders.m_baseCollider.m_center.y, 
									m_snapPositions[3].position.z - m_playerManager.m_colliders.m_baseCollider.m_center.x);
	}

	public bool CanSnapToThisPoint(int point){

		InitializeBoxCastToSnap();

		if(point == 0){
			
			Collider[] hitColliders = Physics.OverlapBox(centerForward, sizeForAndBack / 2, Quaternion.identity, m_objectColliders);
			return !CollidersInHitColliders(hitColliders);

		}else if(point == 1){

			Collider[] hitColliders = Physics.OverlapBox(centerRight, sizeRAndL / 2, Quaternion.identity, m_objectColliders);
			return !CollidersInHitColliders(hitColliders);

		}else if(point == 2){
			
			Collider[] hitColliders = Physics.OverlapBox(centerBackward, sizeForAndBack / 2, Quaternion.identity, m_objectColliders);
			return !CollidersInHitColliders(hitColliders);

		}else if(point == 3){
			
			Collider[] hitColliders = Physics.OverlapBox(centerLeft, sizeRAndL / 2, Quaternion.identity, m_objectColliders);
			return !CollidersInHitColliders(hitColliders);

		}

		return false;
	} 

	bool CollidersInHitColliders(Collider[] hitColliders){
		int colliders = 0;
		for (int i = 0, l = hitColliders.Length; i < l; ++i){
			if(hitColliders[i] != BoxColl){
				Debug.Log("Colliders = " + hitColliders[i].gameObject.name);
				colliders ++;
			}
		}
		return colliders > 0 ? true : false;
	}

	void OnDrawGizmos(){
		/*for(int i = 0, l = m_raycastPositions.Length; i < l; ++i){
			Debug.DrawRay(m_raycastPositions[i].transform.position, - m_raycastPositions[i].transform.up * m_maxDistance, m_rayColor, 0.1f);
		}*/

		/*Gizmos.color = m_boxColor;
		Vector3 boxSizeForward = new Vector3(m_boxSizeForward.x * transform.lossyScale.x, m_boxSizeForward.y * transform.lossyScale.y, m_boxSizeForward.z * transform.lossyScale.z);
		Vector3 boxSizeRight = new Vector3(m_boxSizeRight.x * transform.localScale.x, m_boxSizeRight.y * transform.localScale.y, m_boxSizeRight.z * transform.localScale.z);*/

		// Gizmos.DrawCube(transform.position + On_PlayerSnapToObject().forward * m_boxCastDistance, boxSize);
		/*if(m_playerManager != null){
			/*if(On_PlayerSnapToObject() == m_snapPositions[0]){
				Gizmos.DrawCube(transform.position + Vector3.back * m_castDistanceForward, boxSizeForward);
			}else if(On_PlayerSnapToObject() == m_snapPositions[1]){
				Gizmos.DrawCube(transform.position + Vector3.left * m_castDistanceRight, boxSize);
			}else if(On_PlayerSnapToObject() == m_snapPositions[2]){
				Gizmos.DrawCube(transform.position + Vector3.forward * m_castDistanceForward, boxSizeForward);
			}else if(On_PlayerSnapToObject() == m_snapPositions[3]){
				Gizmos.DrawCube(transform.position + Vector3.right * m_castDistanceRight, boxSize);
			}*

			Gizmos.DrawCube(transform.position + Vector3.forward * m_castDistanceForward, boxSizeForward);

		}

		Gizmos.DrawCube(transform.position + Vector3.forward * m_castDistanceForward, boxSizeForward);*/

		if(m_playerManager != null){
			// FORWARD
			Gizmos.DrawWireCube(centerForward, sizeForAndBack);

			// BackWARD
			Gizmos.DrawWireCube(centerBackward, sizeForAndBack);

			// RIGHT
			Gizmos.DrawWireCube(centerRight, sizeRAndL);

			// LEFT
			Gizmos.DrawWireCube(centerLeft, sizeRAndL);
		}

	}

	
}
