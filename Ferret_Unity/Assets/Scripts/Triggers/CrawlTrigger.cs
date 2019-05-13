using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CrawlTrigger : MonoBehaviour {

	SwitchCamera m_switchCamera;

	BoxCollider m_boxCollider;
    BoxCollider BoxCollider{
        get
		{
            return GetComponent<BoxCollider>();
        }
    }
	
	void Start(){
		m_switchCamera = SwitchCamera.Instance;
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			m_switchCamera.SwitchCameraType();
		}
	}
	/*void OnTriggerExit(Collider col){
		if(col.CompareTag("Player")){
			m_switchCamera.SwitchCameraType();
		}
	}*/

	void OnDrawGizmos(){
		Gizmos.color = Color.magenta;
		Gizmos.DrawCube(transform.position + BoxCollider.center, BoxCollider.size);
	}

}
