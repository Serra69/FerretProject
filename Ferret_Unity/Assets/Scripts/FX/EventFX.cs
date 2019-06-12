using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFX : MonoBehaviour {

	[SerializeField] GameObject m_objectFX;
	[SerializeField] Transform m_fxTransform;

	public void AddFX(){
		if(m_fxTransform != null){
			Level.AddFX(m_objectFX, m_fxTransform.position, m_fxTransform.rotation);
		}else{
			Level.AddFX(m_objectFX, transform.position, transform.rotation);
		}
	}
	
}
