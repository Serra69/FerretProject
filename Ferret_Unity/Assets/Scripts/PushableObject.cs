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
	[SerializeField] Color m_gizmosColor = Color.black;
	[SerializeField] LayerMask m_whatIsGround;
	[SerializeField] Transform[] m_raycastPositions;
	
	bool[] m_raycastReturn;

	PlayerManager m_playerManager;
	float actualClosedDistance;
	Transform m_closedTransform = null;
	bool m_objectIsPushByTheFerret = false;

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


    void Start(){
		m_raycastReturn = new bool[m_raycastPositions.Length];
		m_playerManager = PlayerManager.Instance;
	}

	void Update(){
		if(!m_objectAllowToFall && m_objectIsPushByTheFerret){
			CheckGround();
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

	void OnDrawGizmos(){
		for(int i = 0, l = m_raycastPositions.Length; i < l; ++i){
			Debug.DrawRay(m_raycastPositions[i].transform.position, - m_raycastPositions[i].transform.up * m_maxDistance, m_gizmosColor, 0.1f);
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
	}
	
}
