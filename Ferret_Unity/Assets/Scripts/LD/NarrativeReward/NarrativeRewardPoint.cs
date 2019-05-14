using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class NarrativeRewardPoint : MonoBehaviour {

	[Header("Reward")]
	[SerializeField] int m_rewardNumber;

	[Header("Gizmos")]
	public ManageGizmos m_manageGizmos = new ManageGizmos();
	[System.Serializable] public class ManageGizmos {
		public Color m_color = Color.blue;
    }

	BoxCollider m_triggerCollider;
    BoxCollider TriggerCollider{
        get
		{
            return GetComponent<BoxCollider>();
        }
    }

	NarrativeRewardManager m_narrativeRewardManager;
	bool m_pointIsUsed = false;

	void Start(){
		m_narrativeRewardManager = NarrativeRewardManager.Instance;
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player") && !m_pointIsUsed){
			m_pointIsUsed = true;
			m_narrativeRewardManager.On_NarrativeRewardIsDiscovered(m_rewardNumber);
		}
	}

	void OnDrawGizmosSelected(){
		Gizmos.color = m_manageGizmos.m_color;
		if(TriggerCollider != null){
			Gizmos.DrawWireCube(transform.position + TriggerCollider.center, TriggerCollider.size);
		}
	}
	
}
