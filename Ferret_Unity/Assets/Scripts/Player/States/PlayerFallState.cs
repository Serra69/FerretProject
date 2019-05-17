using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : IState
{

	PlayerManager m_playerManager;

	// Constructor (CTOR)
  public PlayerFallState (PlayerManager playerManager)
  {
    m_playerManager = playerManager;
  }

  public void Enter()
  {
    //Debug.LogFormat("{0} : Enter()", GetType().Name);
    m_playerManager.Animator.SetTrigger("Fall");
  }

  public void Exit()
  {
    //Debug.LogFormat("{0} : Exit()", GetType().Name);
  }

  public void FixedUpdate()
  {
    Move();
  }

  public void Update()
  {
    if(m_playerManager.CheckCollider(false)){
      m_playerManager.ChangeState(0);
    }
    if(m_playerManager.RayCastForwardToStartClimbing() && m_playerManager.m_states.m_climb.m_canClimb){
      m_playerManager.ChangeState(6);
    }
  }

  void Move(){

    if(m_playerManager.SwitchCamera.ThirdPersonMode){
      m_playerManager.MovePlayer(m_playerManager.LastStateMoveSpeed);
      m_playerManager.RotatePlayer();
    }else{
      m_playerManager.MoveFirstPersonPlayer(m_playerManager.LastStateMoveSpeed);
      if(!m_playerManager.SwitchCamera.CameraIsSwitching){
        m_playerManager.FirstPersonCamera.RotateCamera();
      }
    }

  }

}
