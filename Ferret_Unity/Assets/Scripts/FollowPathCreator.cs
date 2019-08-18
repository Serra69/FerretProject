using UnityEngine;
using System;
using PathCreation;

public class FollowPathCreator : MonoBehaviour {

	[Header("Path Creator variables")]
	[SerializeField] EndOfPathInstruction m_endOfPathInstruction = EndOfPathInstruction.Stop;

	[Header("Transform who follows the path")]
	[SerializeField] Transform m_followTransform;

	[Header("Speeds")]
	[SerializeField] float m_startMoveSpeed = 1;
	public Speeds[] m_speeds = new Speeds[0];
	[Serializable] public class Speeds {
		public float m_timer = 1;
		public float m_moveSpeed = 1;
		[HideInInspector] public bool m_timerIsPassed = false;
	}

	[Header("Gizmos")]
	public GizmosFollow m_gizmos;
	[Serializable] public class GizmosFollow {
		public float m_radius = 0.25f;
		public Color m_color = Color.white;
	}
	
	PathCreator m_pathCreator;
	float m_distanceTravelled;

	float m_timer = 0;
	float m_moveSpeed = 0;

	void Start(){
		m_pathCreator = GetComponent<PathCreator>();
		m_moveSpeed = m_startMoveSpeed;
	}

	void Update(){
		m_timer += Time.deltaTime;
		for (int i = 0, l = m_speeds.Length; i < l; ++i) {
			if(!m_speeds[i].m_timerIsPassed){
				if(m_timer >= m_speeds[i].m_timer){
					m_moveSpeed = m_speeds[i].m_moveSpeed;
					m_speeds[i].m_timerIsPassed = true;
				}
			}
		}

		m_distanceTravelled += m_moveSpeed * Time.deltaTime;
		m_followTransform.position = m_pathCreator.path.GetPointAtDistance(m_distanceTravelled, m_endOfPathInstruction);
		m_followTransform.rotation = m_pathCreator.path.GetRotationAtDistance(m_distanceTravelled, m_endOfPathInstruction);
	}

	void OnDrawGizmos(){
		Gizmos.color = m_gizmos.m_color;
		if(m_followTransform != null){
			Gizmos.DrawSphere(m_followTransform.position, m_gizmos.m_radius);
		}
	}
	
}
