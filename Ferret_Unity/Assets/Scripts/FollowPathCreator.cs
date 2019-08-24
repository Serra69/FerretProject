﻿using UnityEngine;
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
	[Space]
	[SerializeField] bool m_usePathRotation = true;

	[Header("Move speeds")]
	[SerializeField] float m_startMoveSpeed = 1;
	public MoveSpeeds[] m_moveSpeeds = new MoveSpeeds[0];
	[Serializable] public class MoveSpeeds {
		[Header("CREATION ASSISTANCE")]
		[Tooltip("You can not modify the variable.")]
		public int m_positionNumber;

		[Tooltip("Change the name to make it easier for you to find yourself!")]
		public string m_levelPart;
		[Space]

		[Header("VARIABLES")]
		[Tooltip("Timer from last moveSpeeds timer.")]
		public float m_timer = 1;
		public float m_moveSpeed = 1;
		public float m_timeToReachNewSpeed = 1;
		public AnimationCurve m_changeSpeedCurve;
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

	float m_actualMoveSpeed = 0;

	IEnumerator m_changeSpeedCorout;
	bool m_changeSpeedCoroutIsRunning = false;

	void Start(){
		m_pathCreator = GetComponent<PathCreator>();
		m_actualMoveSpeed = m_startMoveSpeed;
		StartCoroutine(WaitTimeToUsePath(m_usePathAtStart));
	}

	void Update(){
		if(!m_usePath){
			return;
		}
		m_timer += Time.deltaTime;
		if(m_moveSpeedNumber < m_moveSpeeds.Length){
			if(m_timer >= m_actualLastTimer + m_moveSpeeds[m_moveSpeedNumber].m_timer){
				// m_actualMoveSpeed = m_moveSpeeds[m_moveSpeedNumber].m_moveSpeed;

				if(m_changeSpeedCorout != null){
					StopCoroutine(m_changeSpeedCorout);
					if(m_changeSpeedCoroutIsRunning){
						Debug.LogWarning("Caution! The ChangeSpeedCorout was overridden by the other after her!");
					}
				}
				m_changeSpeedCorout = ChangeMoveSpeedValue(m_moveSpeeds[m_moveSpeedNumber].m_moveSpeed, m_moveSpeeds[m_moveSpeedNumber].m_timeToReachNewSpeed, m_moveSpeeds[m_moveSpeedNumber].m_changeSpeedCurve);
				StartCoroutine(m_changeSpeedCorout);

				m_actualLastTimer += m_moveSpeeds[m_moveSpeedNumber].m_timer;
				m_moveSpeedNumber ++;
			}
		}

		m_distanceTravelled += m_actualMoveSpeed * Time.deltaTime;
		m_followTransform.position = m_pathCreator.path.GetPointAtDistance(m_distanceTravelled, m_endOfPathInstruction);
		if(m_usePathRotation){
			m_followTransform.rotation = m_pathCreator.path.GetRotationAtDistance(m_distanceTravelled, m_endOfPathInstruction);
		}
	}

	public void UsePath(bool usePath){
		StartCoroutine(WaitTimeToUsePath(usePath));
	}

	IEnumerator WaitTimeToUsePath(bool usePath){
		yield return new WaitForSeconds(m_delayToStartUsePath);
		m_usePath = usePath;
	}

	IEnumerator ChangeMoveSpeedValue(float newSpeedValue, float timeToReachNewSpeed, AnimationCurve changeSpeedCurve)
	{
		m_changeSpeedCoroutIsRunning = true;
		float distance = Mathf.Abs(m_actualMoveSpeed - newSpeedValue);
		float vitesse = distance / timeToReachNewSpeed;
		float moveFracJourney = new float();

		while(m_actualMoveSpeed != newSpeedValue){
			moveFracJourney += (Time.deltaTime) * vitesse / distance;
			m_actualMoveSpeed = Mathf.Lerp(m_actualMoveSpeed, newSpeedValue, changeSpeedCurve.Evaluate(moveFracJourney));
			yield return null;
		}
		m_changeSpeedCoroutIsRunning = false;
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
