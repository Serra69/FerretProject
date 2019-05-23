﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : IState
{

	PlayerManager m_playerManager;
  float m_timer = 0;
  bool m_isJumpButtonHeld = false;

	// Constructor (CTOR)
  public PlayerJumpState (PlayerManager playerManager)
  {
    m_playerManager = playerManager;
  }

  public void Enter()
  {
    m_playerManager.Animator.SetTrigger("Jump");
    m_timer = 0;
  }

  public void Exit()
  {

  }

  public void FixedUpdate()
  {
    if (isJumpContinue() && m_timer < m_playerManager.m_states.m_jump.m_jumpTime)
    {   
      float proportionCompleted = m_timer / m_playerManager.m_states.m_jump.m_jumpTime;
      float yDirection = m_playerManager.m_states.m_jump.m_jumpHeightCurve.Evaluate(proportionCompleted);
      // Debug.Log("proportionCompleted=" + proportionCompleted);

      m_playerManager.m_timerOfPressSpace = proportionCompleted;
      
      if(m_playerManager.SwitchCamera.ThirdPersonMode){
        m_playerManager.MovePlayer(m_playerManager.LastStateMoveSpeed, yDirection, m_playerManager.m_states.m_jump.m_jumpForce);
      }else{
        m_playerManager.MoveFirstPersonPlayer(m_playerManager.LastStateMoveSpeed, yDirection, m_playerManager.m_states.m_jump.m_jumpForce);
      }

      m_timer += Time.deltaTime;
    }
    else
    {
      m_playerManager.ChangeState(4);
    }
    
    MoveCamera();
  }

  bool isJumpContinue(){
    return m_isJumpButtonHeld || m_timer < m_playerManager.m_states.m_jump.m_minJumpTime;
  }

  public void Update()
  {
    m_isJumpButtonHeld = m_playerManager.m_jumpHeldButton;
  }

  void MoveCamera(){
    
    if(m_playerManager.SwitchCamera.ThirdPersonMode){
      if(m_playerManager.PlayerInputIsMoving()){
        m_playerManager.RotatePlayer();
      }
    }else{
      if(!m_playerManager.SwitchCamera.CameraIsSwitching){
        m_playerManager.FirstPersonCamera.RotateCamera();
      }
    }

  }

}
