using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlController : MonoBehaviour {
	
	[Header("Gizmos")]
	public bool m_showGizmos = true;
	public Color m_enterColorGizmos = Color.magenta;
	public Color m_exitColorGizmos = Color.white;
	
	CrawlTrigger m_enterTrigger;
    public CrawlTrigger EnterTrigger{
        get{
            return m_enterTrigger;
        }
        set{
            m_enterTrigger = value;
        }
    }

    CrawlTrigger m_exitTrigger;
    public CrawlTrigger ExitTrigger{
        get{
            return m_exitTrigger;
        }
        set{
            m_exitTrigger = value;
        }
    }

	SwitchCamera m_switchCamera;

	bool m_isInside = false;

    void Start(){
		m_switchCamera = SwitchCamera.Instance;
	}

	public void On_PlayerInTrigger(){

		// On enter
		if(!m_exitTrigger.PlayerInTrigger && m_enterTrigger.PlayerInTrigger){
			if(!m_isInside){
				m_isInside = true;
				m_switchCamera.SwitchCameraType();
			}
		}

		// On exit
		if(m_exitTrigger.PlayerInTrigger && !m_enterTrigger.PlayerInTrigger){
			if(m_isInside){
				m_isInside = false;
				m_switchCamera.SwitchCameraType();
			}
		}

	}

}
