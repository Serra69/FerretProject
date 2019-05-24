using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixeCamera : MonoBehaviour {

	[Header("Object to follow")]
	public Transform m_objectToFollow;

	[Header("Parameters")]
	public Parameters m_parameters = new Parameters();
	[System.Serializable] public class Parameters {
		[Header("Speeds")]
		[Range(0, 1)] public float m_xSpeed = 0.5f;
		[Range(0, 1)] public float m_ySpeed = 0.5f;

		[Header("Z speed")]
		[Range(0, 5)] public float m_zPos = 3;
		[Range(0, 1)] public float m_zSpeed = 0.5f;
	}

	[Header("Positions")]
	public Positions m_positions = new Positions();
	[System.Serializable] public class Positions {
		public Vector2 m_maxTop;
		public Vector2 m_maxRight;
		public Vector2 m_maxBot;
		public Vector2 m_maxLeft;
	}	

	Vector3 m_offset;

	void Start(){
		m_offset = transform.position;
	}
	
	void LateUpdate(){

		float playerXRange = Mathf.InverseLerp(m_offset.x - m_positions.m_maxLeft.x, m_offset.x - m_positions.m_maxRight.x, m_objectToFollow.transform.position.x);
		float playerYRange = Mathf.InverseLerp(m_offset.y + m_positions.m_maxBot.y, m_offset.y + m_positions.m_maxTop.y, m_objectToFollow.transform.position.y);

		float desiredXPosition = Mathf.Lerp(m_offset.x - m_positions.m_maxLeft.x, m_offset.x - m_positions.m_maxRight.x, playerXRange);
		float desiredYPosition = Mathf.Lerp(m_offset.y + m_positions.m_maxBot.y, m_offset.y + m_positions.m_maxTop.y, playerYRange);

		float xCurve = Mathf.Lerp(transform.position.x, desiredXPosition, m_parameters.m_xSpeed);
		float yCurve = Mathf.Lerp(transform.position.y, desiredYPosition, m_parameters.m_ySpeed);
		float zCurve = Mathf.Lerp(transform.position.z, m_offset.z - m_parameters.m_zPos, m_parameters.m_zSpeed);

		transform.position =  new Vector3(xCurve, yCurve, zCurve);
	}
	
}
