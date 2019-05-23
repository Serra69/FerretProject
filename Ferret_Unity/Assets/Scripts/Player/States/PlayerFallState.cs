using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : IState
{
  float m_currentime = 0;

  bool m_isFalling = false;
  float m_currentTimeOfFall = 0;

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
    m_currentime = 1 - m_playerManager.m_timerOfPressSpace;

    m_currentTimeOfFall = 0;
  }

  public void Exit()
  {
    //Debug.LogFormat("{0} : Exit()", GetType().Name);
  }

  public void FixedUpdate()
  {
    Move();
    m_currentime += Time.deltaTime;
    if(m_currentime > m_playerManager.m_states.m_fall.m_duration){
      m_currentime = m_playerManager.m_states.m_fall.m_duration;
    }

    if(m_isFalling){
      m_currentTimeOfFall += Time.deltaTime;
    }
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

      if(m_playerManager.m_states.m_jump.m_jumpHeightCurve.Evaluate(1 - m_currentime / m_playerManager.m_states.m_fall.m_duration) * Physics.gravity.y > 1){
        // Debug.Log("je suis au dessus de 1");
        m_playerManager.MovePlayer(m_playerManager.LastStateMoveSpeed, m_playerManager.m_states.m_jump.m_jumpHeightCurve.Evaluate(1 - m_currentime / m_playerManager.m_states.m_fall.m_duration) * Physics.gravity.y, 1);
      }else{
        // Debug.Log("je suis en dessous de 1");
        m_isFalling = true;
        m_playerManager.MovePlayer(m_playerManager.LastStateMoveSpeed, Physics.gravity.y * m_playerManager.m_states.m_fall.m_fallMultiplier, 1);
        // m_playerManager.MovePlayer(3, - Mathf.Exp(m_currentTimeOfFall * m_playerManager.m_states.m_fall.m_fallMultiplier * 9.81f), 1);

        // Debug.Log("Mathf.Exp(m_currentTimeOfFall) = " + - Mathf.Exp(m_currentTimeOfFall * m_playerManager.m_states.m_fall.m_fallMultiplier * 9.81f));
      }

      if(m_playerManager.PlayerInputIsMoving()){
        m_playerManager.RotatePlayer();
      }
    }else{
      m_playerManager.MoveFirstPersonPlayer(m_playerManager.LastStateMoveSpeed);
      if(!m_playerManager.SwitchCamera.CameraIsSwitching){
        m_playerManager.FirstPersonCamera.RotateCamera();
      }
    }

  }

}
