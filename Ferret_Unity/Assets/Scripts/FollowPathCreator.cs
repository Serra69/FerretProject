using UnityEngine;
using System.Collections;
using System;
using PathCreation;

public class FollowPathCreator : MonoBehaviour {

	[Header("PathCreator variables")]
	[SerializeField] EndOfPathInstruction m_endOfPathInstruction = EndOfPathInstruction.Stop;

	[Header("Manage path variables")]
	[SerializeField] bool m_usePathAtStart = true;
	[SerializeField] float m_delayToStartUsePath = 0;
	[SerializeField] Transform m_followTransform;

	[Header("Move speeds")]
	[SerializeField] float m_startMoveSpeed = 1;
	public MoveSpeeds[] m_moveSpeeds = new MoveSpeeds[0];
	[Serializable] public class MoveSpeeds {
		[Tooltip("You can not modify the variable.")]
		public int m_positionNumber;
		[Tooltip("Timer from last moveSpeeds timer.")]
		public float m_timer = 1;
		public float m_moveSpeed = 1;
	}

	[Header("Gizmos")]
	public GizmosFollow m_gizmos;
	[Serializable] public class GizmosFollow {
		public float m_radius = 0.25f;
		public Color m_color = Color.white;
	}
	
	bool m_usePath;

	PathCreator m_pathCreator;
	float m_distanceTravelled;

	float m_timer = 0;
	float m_actualLastTimer = 0;
	int m_moveSpeedNumber = 0;

	float m_moveSpeed = 0;

	void Start(){
		m_pathCreator = GetComponent<PathCreator>();
		m_moveSpeed = m_startMoveSpeed;
		StartCoroutine(WaitTimeToUsePath(m_usePathAtStart));
	}

	void Update(){
		if(!m_usePath){
			return;
		}
		m_timer += Time.deltaTime;
		if(m_moveSpeedNumber < m_moveSpeeds.Length){
			if(m_timer >= m_actualLastTimer + m_moveSpeeds[m_moveSpeedNumber].m_timer){
				m_moveSpeed = m_moveSpeeds[m_moveSpeedNumber].m_moveSpeed;
				m_actualLastTimer += m_moveSpeeds[m_moveSpeedNumber].m_timer;
				m_moveSpeedNumber ++;
			}
		}

		m_distanceTravelled += m_moveSpeed * Time.deltaTime;
		m_followTransform.position = m_pathCreator.path.GetPointAtDistance(m_distanceTravelled, m_endOfPathInstruction);
		m_followTransform.rotation = m_pathCreator.path.GetRotationAtDistance(m_distanceTravelled, m_endOfPathInstruction);
	}

	IEnumerator WaitTimeToUsePath(bool usePath){
		yield return new WaitForSeconds(m_delayToStartUsePath);
		m_usePath = usePath;
	}

	public void UsePath(bool usePath){
		StartCoroutine(WaitTimeToUsePath(usePath));
	}

	void OnDrawGizmos(){
		for (int i = 0, l = m_moveSpeeds.Length; i < l; ++i) {
			m_moveSpeeds[i].m_positionNumber = i;
		}
		Gizmos.color = m_gizmos.m_color;
		if(m_followTransform != null){
			Gizmos.DrawSphere(m_followTransform.position, m_gizmos.m_radius);
		}
	}
	
}
