using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class OnTriggerCollider : MonoBehaviour {

    [SerializeField] bool m_isActiveAtTheStart = true;

	[Header("Tag to trigger")]
	[SerializeField] string m_tagName = "Player";

	[Header("Triggers")]
	[SerializeField] UnityEvent m_onTriggerEnter;
	[SerializeField] UnityEvent m_onTriggerStay;
	[SerializeField] UnityEvent m_onTriggerExit;

    bool m_activeOnce = false;

    void OnTriggerEnter(Collider col){
		if(col.tag == m_tagName && !m_activeOnce && m_isActiveAtTheStart)
        {
            m_activeOnce = true;
            m_onTriggerEnter.Invoke();
		}
	}
	void OnTriggerStay(Collider col){
		if(col.tag == m_tagName && m_isActiveAtTheStart)
        {
			m_onTriggerStay.Invoke();
		}
	}
	void OnTriggerExit(Collider col){
		if(col.tag == m_tagName && m_isActiveAtTheStart)
        {
			m_onTriggerExit.Invoke();
		}
	}

    public void ActiveTrigger(bool b)
    {
        m_isActiveAtTheStart = b;
    }
	
}
