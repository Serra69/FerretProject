﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : IState
{
	PlayerManager m_playerManager;

	// Constructor (CTOR)
    public PlayerRunState (PlayerManager playerManager)
    {
		m_playerManager = playerManager;
    }

    public void Enter()
    {
		  //Debug.LogFormat("{0} : Enter()", GetType().Name);
      m_playerManager.Animator.SetTrigger("Run");
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
      if(!m_playerManager.PlayerInputIsMoving()){
        m_playerManager.ChangeState(0);
      }

      if(!m_playerManager.m_runButton || !m_playerManager.PlayerInputIsRuning()){
        m_playerManager.ChangeState(1);
      }

      if(m_playerManager.m_jumpButton){
        m_playerManager.ChangeState(3);
      }

      if(!m_playerManager.CheckCollider(false)){
        m_playerManager.ChangeState(4);
      }

      if(m_playerManager.m_pushButton){
        if(m_playerManager.RayCastToCanPush()){
          m_playerManager.ChangeState(7);
        }
      }

    }

    public void LateUpdate(){
      
    }

  void Move(){

    if(m_playerManager.SwitchCamera.ThirdPersonMode){
      m_playerManager.MovePlayer(m_playerManager.m_states.m_run.m_speed);
      m_playerManager.RotatePlayer();
    }else{
      m_playerManager.MoveFirstPersonPlayer(m_playerManager.m_states.m_run.m_speed);
      if(!m_playerManager.SwitchCamera.CameraIsSwitching){
        m_playerManager.FirstPersonCamera.RotateCamera();
      }
    }

  }

}
