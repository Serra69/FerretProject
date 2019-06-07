using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : IState
{
	PlayerManager m_playerManager;

	// Constructor (CTOR)
    public PlayerIdleState (PlayerManager playerManager)
    {
		  m_playerManager = playerManager;
    }

    public void Enter()
    {
		  //Debug.LogFormat("{0} : Enter()", GetType().Name);
      m_playerManager.Animator.SetBool("Move", false);
      
      m_playerManager.m_isInIddle = true;
      m_playerManager.StartIddleTimer();

      if(m_playerManager.m_sM.IsLastStateIndex(4)){
        m_playerManager.OnPlayerLanding();
      }

    }

    public void Exit()
    {
		  //Debug.LogFormat("{0} : Exit()", GetType().Name);
      m_playerManager.m_isInIddle = false;
    }

    public void FixedUpdate()
    {
      
    }

    public void Update()
    {
      if(m_playerManager.PlayerInputIsMoving()){
        m_playerManager.ChangeState(1);
      }

      if(m_playerManager.m_crawlButton){
        if(!m_playerManager.CheckCollider(true)){
          m_playerManager.ChangeState(5);
        }
      }

      if(m_playerManager.m_jumpButton){
        m_playerManager.ChangeState(3);
      }

      if(!m_playerManager.CheckCollider(false)){
        m_playerManager.ChangeState(4);
      }
      
      if(!m_playerManager.SwitchCamera.ThirdPersonMode && !m_playerManager.SwitchCamera.CameraIsSwitching){
        m_playerManager.FirstPersonCamera.RotateCamera();
      }

      if(m_playerManager.m_pushButton){
        if(m_playerManager.RayCastToCanPush() && !m_playerManager.m_states.m_takeObject.m_iHaveAnObject){
          m_playerManager.ChangeState(7);
        }
      }

    }

    public void LateUpdate(){
      
    }

}
