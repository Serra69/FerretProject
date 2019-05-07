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
      m_playerManager.Animator.SetTrigger("Idle");
    }

    public void Exit()
    {
		  //Debug.LogFormat("{0} : Exit()", GetType().Name);
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
      if(Input.GetKeyDown(KeyCode.A)){
        SwitchCamera.Instance.SwitchCameraType();
      }

      if(m_playerManager.m_jumpButton){
        m_playerManager.ChangeState(3);
      }

      if(!m_playerManager.CheckCollider(false)){
        m_playerManager.ChangeState(4);
      }
      
      if(!SwitchCamera.Instance.ThirdPersonMode){
        FirstPersonCamera.Instance.RotateCamera();
      }

      if(m_playerManager.RayCastToCanPush()){
        if(m_playerManager.m_pushButton){
          m_playerManager.ChangeState(7);
        }
      }

    }

}
