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
	public Triggers[] m_triggerEnter;
	// public Triggers[] m_triggerStay;
	// public Triggers[] m_triggerExit;
	[System.Serializable] public class Triggers {
		public float m_timer = 0;
		public UnityEvent m_event;
	}

	[Header("Gizmos")]
	[SerializeField] bool m_showGizmos = true;
	[SerializeField] Color m_gizmosColor = Color.magenta;

    public BoxCollider BoxColl{
        get{
            return GetComponent<BoxCollider>();
        }
    }

    bool m_activeOnce = false;
	float m_timer = 0;

    void OnTriggerEnter(Collider col){
		if(col.tag == m_tagName && !m_activeOnce && m_isActiveAtTheStart)
        {
            m_activeOnce = true;

			for(int i = 0, l = m_triggerEnter.Length; i < l; ++i){
				StartCoroutine(PlayerInTrigger(m_triggerEnter[i]));
        	}
		}
	}
	/*void OnTriggerStay(Collider col){
		if(col.tag == m_tagName && m_isActiveAtTheStart)
        {
			// m_onTriggerStay.Invoke();
		}
	}
	void OnTriggerExit(Collider col){
		if(col.tag == m_tagName && m_isActiveAtTheStart)
        {
			// m_onTriggerExit.Invoke();
		}
	}*/

	IEnumerator PlayerInTrigger(Triggers triggers){
		yield return new WaitForSeconds(triggers.m_timer);
		triggers.m_event.Invoke();
	}

    public void ActiveTrigger(bool b)
    {
        m_isActiveAtTheStart = b;
    }

	void Update(){
		if(m_activeOnce){
			m_timer += Time.deltaTime;
		}
	}
	
	void OnDrawGizmos(){
		if(!m_showGizmos){
			return;
		}
		Gizmos.color = m_gizmosColor;
		Gizmos.DrawWireCube(transform.position + BoxColl.center, BoxColl.size);
	}

}
