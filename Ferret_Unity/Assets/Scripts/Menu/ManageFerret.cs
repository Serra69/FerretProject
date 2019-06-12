using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageFerret : MonoBehaviour {

	[SerializeField] float m_timeToEndFade = 1;

	[Header("Time to iddle")]
	[SerializeField] float m_miniTimeToSecondIdle = 7.5f;
	[SerializeField] float m_maxTimeToSecondIdle = 15f;

	PlayerManager m_playerManager;
	Animator m_ferretAnimator;

	void Start(){
		m_playerManager = PlayerManager.Instance;
		m_ferretAnimator = GetComponentInChildren<Animator>();
		StartCoroutine(WaitIdle());
	}
	
	IEnumerator WaitIdle(){
		float random = Random.Range(m_miniTimeToSecondIdle, m_maxTimeToSecondIdle);
		yield return new WaitForSeconds(random);
		m_ferretAnimator.SetTrigger("Iddle");
		StartCoroutine(WaitIdle());
	}

	public void StartFerret(){
		StartCoroutine(StartFerretCorout());
	}
	IEnumerator StartFerretCorout(){
		yield return new WaitForSeconds(m_timeToEndFade);
		m_playerManager.ChangeState(0);
	}

}
