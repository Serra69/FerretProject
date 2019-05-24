using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CrawlTrigger : TriggerType {

	public TriggerTypes m_triggerType = TriggerTypes.Enter;

	BoxCollider m_boxCollider;
    BoxCollider BoxCollider{
        get{
            return GetComponent<BoxCollider>();
        }
    }
    CrawlController m_crawlController;
    public CrawlController CrawlController{
        get{
            return GetComponentInParent<CrawlController>();
        }
    }

    bool m_playerInTrigger = false;
    public bool PlayerInTrigger{
        get{
            return m_playerInTrigger;
        }
    }

	void Start(){

		BoxCollider.isTrigger = true;
		switch(m_triggerType){ 
			case TriggerTypes.Enter:
				CrawlController.EnterTrigger = this;
			break;
			case TriggerTypes.Exit:
				CrawlController.ExitTrigger = this;
			break;
		}
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			m_playerInTrigger = true;
			CrawlController.On_PlayerInTrigger();
		}
	}
	void OnTriggerExit(Collider col){
		if(col.CompareTag("Player")){
			m_playerInTrigger = false;
			CrawlController.On_PlayerInTrigger();
		}
	}

	void OnDrawGizmos(){
		if(!CrawlController.m_showGizmos){
			return;
		}

		switch(m_triggerType){ 
			case TriggerTypes.Enter:
				Gizmos.color = CrawlController.m_enterColorGizmos;
			break;
			case TriggerTypes.Exit:
				Gizmos.color = CrawlController.m_exitColorGizmos;
			break;
		}
		Gizmos.DrawWireCube(transform.position + BoxCollider.center, BoxCollider.size);
	}

}
