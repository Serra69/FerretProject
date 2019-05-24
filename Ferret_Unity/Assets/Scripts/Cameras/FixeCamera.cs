﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixeCamera : MonoBehaviour {

	public Transform m_objectToFollow;

	[Header("Positions")]
	public Positions m_positions = new Positions();
	[System.Serializable] public class Positions {
		public Vector2 m_maxTop;
		public Vector2 m_maxRight;
		public Vector2 m_maxBot;
		public Vector2 m_maxLeft;
	}	

	[Range(0, 1)] public float m_leftAndRight = 0.5f;

	Vector3 m_offset;

	float m_maxRight;
	float m_maxLeft;

	void Start(){
		m_offset = transform.position;
	}
	
	void LateUpdate(){

		/*Vector3 desiredPos = new Vector3();

		if(transform.localPosition.x >= m_positions.m_maxRight.x){
			transform.position = new Vector3(m_offset.x - m_positions.m_maxRight.x, m_objectToFollow.position.y, transform.position.z);
		}else{
			desiredPos = new Vector3(m_objectToFollow.position.x, m_objectToFollow.position.y, transform.position.z);
			
				transform.position = desiredPos;
		}*/

		float desiredPosition = Mathf.Lerp(m_offset.x - m_positions.m_maxLeft.x, m_offset.x - m_positions.m_maxRight.x, m_leftAndRight);
		transform.position =  new Vector3(desiredPosition, transform.position.y, transform.position.z);

	}
	
}
