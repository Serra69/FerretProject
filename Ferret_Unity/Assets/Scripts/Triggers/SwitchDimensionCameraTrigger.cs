using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchDimensionCameraTrigger : TriggerType {

    public TriggerTypes m_triggerType = TriggerTypes.Exit;

    FixeCamera m_fixeCamera;
    public FixeCamera FixeCamera{
        get{
            return m_fixeCamera;
        }
        set{
            m_fixeCamera = value;
        }
    }

	bool m_playerIsInTrigger = false;
    public bool PlayerIsInTrigger{
        get{
            return m_playerIsInTrigger;
        }
    }

    void OnTriggerEnter(Collider col){
        if(col.CompareTag("Player")){
            // m_playerIsInTrigger = true;
            // m_fixeCamera.CheckIfPlayerIsInTriggers();

            switch(m_triggerType){
                case TriggerTypes.Enter:
                    m_fixeCamera.OnPlayerEnterInEnterTrigger();
                break;
                case TriggerTypes.Exit:
                    m_playerIsInTrigger = true;
                break;
                case TriggerTypes.ChangeZ:
                    m_fixeCamera.On_ChangeZPosition();
                break;
            }
        }
    }

    void OnTriggerExit(Collider col){
        if(col.CompareTag("Player")){
            // m_playerIsInTrigger = false;
            // m_fixeCamera.CheckIfPlayerIsInTriggers();

            switch(m_triggerType){
                case TriggerTypes.Enter:
                break;
                case TriggerTypes.Exit:
                    m_playerIsInTrigger = false;
                    m_fixeCamera.CheckIfPlayerIsInTriggers();
                break;
                case TriggerTypes.ChangeZ:
                    m_fixeCamera.On_ChangeZPosition();
                break;
            }
        }   
    }

}
