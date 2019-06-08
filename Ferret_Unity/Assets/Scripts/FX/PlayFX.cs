using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFX : MonoBehaviour {

	[Header("FX")]
	[SerializeField] GameObject m_moveFx;

	[Header("Positions")]
	[SerializeField] Transform m_frontRightPos;
	[SerializeField] Transform m_frontLeftPos;
	[SerializeField] Transform m_backRightPos;
	[SerializeField] Transform m_backLeftPos;

	[Header("ParticleSystem")]
	public ParticleSystem m_frontRightPs;
	public ParticleSystem m_frontLeftPs;
	public ParticleSystem m_backRightPs;
	public ParticleSystem m_backLeftPs;

	public void SpawnFrontRightParticles(){
		// m_frontRightPs.Play();
		Level.AddFX(m_moveFx, m_frontRightPos.position, m_moveFx.transform.rotation);
	}
	public void SpawnFrontLeftParticles(){
		// m_frontLeftPs.Play();
		Level.AddFX(m_moveFx, m_frontLeftPos.position, m_moveFx.transform.rotation);
	}
	public void SpawnBackRightParticles(){
		// m_backRightPs.Play();
		Level.AddFX(m_moveFx, m_backRightPos.position, m_moveFx.transform.rotation);
	}
	public void SpawnBackLeftParticles(){
		// m_backLeftPs.Play();
		Level.AddFX(m_moveFx, m_backLeftPos.position, m_moveFx.transform.rotation);
	}

	public void SpawnAllParticles(){
		// m_frontRightPs.Play();
		// m_frontLeftPs.Play();
		// m_backRightPs.Play();
		// m_backLeftPs.Play();

		Level.AddFX(m_moveFx, m_frontRightPos.position, m_moveFx.transform.rotation);
		Level.AddFX(m_moveFx, m_frontLeftPos.position, m_moveFx.transform.rotation);
		Level.AddFX(m_moveFx, m_backRightPos.position, m_moveFx.transform.rotation);
		Level.AddFX(m_moveFx, m_backLeftPos.position, m_moveFx.transform.rotation);
	}
	
}
