using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class OnTriggerCollider : MonoBehaviour {

	[Header("Tag to trigger")]
	[SerializeField] string m_tagName = "Player";

	[Header("Triggers")]
	[SerializeField] UnityEvent m_onTriggerEnter;
	[SerializeField] UnityEvent m_onTriggerStay;
	[SerializeField] UnityEvent m_onTriggerExit;

	void OnTriggerEnter(Collider col){
		if(col.tag == m_tagName){
			m_onTriggerEnter.Invoke();
		}
	}
	void OnTriggerStay(Collider col){
		if(col.tag == m_tagName){
			m_onTriggerStay.Invoke();
		}
	}
	void OnTriggerExit(Collider col){
		if(col.tag == m_tagName){
			m_onTriggerExit.Invoke();
		}
	}
	
}
