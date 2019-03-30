using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CrawlTrigger : MonoBehaviour {

	BoxCollider m_boxCollider;
    BoxCollider BoxCollider{
        get{
            return GetComponent<BoxCollider>();
        }
		set{
            m_boxCollider = value;
        }
    }
	
	void OnTriggerEnter(Collider col){
		print("ENTER");
		if(col.CompareTag("Player")){
			print("he enter");
			SwitchCamera.Instance.SwitchCameraType();
		}
	}
	void OnTriggerExit(Collider col){
		if(col.CompareTag("Player")){
			SwitchCamera.Instance.SwitchCameraType();
		}
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube(transform.position + BoxCollider.center, BoxCollider.size);
	}

}
